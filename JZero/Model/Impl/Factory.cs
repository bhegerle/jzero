using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.RegularExpressions;

namespace JZero.Model.Impl {
    internal static class Factory {
        internal static T NewModel<T>() {
            var impl = ImplementingType(typeof(T));
            return (T)Activator.CreateInstance(impl);
        }

        internal static ScalarModel<T> NewScalar<T>(T value = default(T)) {
            if (ScalarTypeMap.TryGetValue(typeof(T), out var stype))
                return (ScalarModel<T>)Activator.CreateInstance(stype, value);
            else
                throw new Exception("not a scalar type");
        }

        private static readonly Dictionary<Type, Type> ModelTypeMap;
        private static readonly Dictionary<Type, Type> ScalarTypeMap;
        private static readonly ModuleBuilder mod;

        private static Type ImplementingType(Type iface) {
            lock (ModelTypeMap) {
                if (!ModelTypeMap.TryGetValue(iface, out var impl)) {
                    if (!iface.IsInterface)
                        throw new Exception("not an interface");

                    ModelTypeMap[iface] = null;

                    if (iface.IsGenericType && iface.GenericTypeArguments.Length == 1) {
                        var genDef = iface.GetGenericTypeDefinition();
                        var genArg = iface.GenericTypeArguments[0];
                        if (genDef == typeof(IArray<>)) {
                            if (ScalarTypeMap.ContainsKey(genArg))
                                impl = typeof(ScalarArrayModel<>).MakeGenericType(genArg);
                            else
                                impl = typeof(ArrayModel<>).MakeGenericType(genArg);
                        } else if (genDef == typeof(IDict<>)) {
                            if (ScalarTypeMap.ContainsKey(genArg))
                                impl = typeof(ScalarDictModel<>).MakeGenericType(genArg);
                            else
                                impl = typeof(DictModel<>).MakeGenericType(genArg);
                        }
                    }

                    if (impl == null)
                        impl = BuildImplementingType(iface);

                    ModelTypeMap[iface] = impl;
                }

                return impl;
            }
        }

        private static readonly MethodInfo WriteObjectStart, WriteObjectEnd, WritePropertyName, WriteValue;
        private static readonly MethodInfo Assigned;

        private static Type BuildImplementingType(Type iface) {
            var methodSet = new HashSet<MethodInfo>(iface.GetMethods());

            var n = Regex.Replace(iface.Name, "^I([A-Z])", "$1");

            var tgen = mod.DefineType(n, TypeAttributes.Public, typeof(ModelBase), null);
            tgen.AddInterfaceImplementation(iface);

            var fieldMap = new Dictionary<PropertyInfo, FieldInfo>();

            foreach (var prop in iface.GetProperties(BindingFlags.Public | BindingFlags.Instance)) {
                if (prop.GetIndexParameters().Length > 0)
                    throw new Exception("indexer not supported");

                var pget = prop.GetGetMethod();
                var pset = prop.GetSetMethod();

                if (pget == null || pset == null)
                    throw new Exception("property must be gettable and settable");

                methodSet.Remove(pget);
                methodSet.Remove(pset);

                if (prop.PropertyType.IsInterface) {
                    ImplementingType(prop.PropertyType);
                    var fld = tgen.DefineField('$' + prop.Name, prop.PropertyType, FieldAttributes.Private);
                    fieldMap[prop] = fld;
                } else if (ScalarTypeMap.TryGetValue(prop.PropertyType, out var stype)) {
                    var fld = tgen.DefineField('$' + prop.Name, stype, FieldAttributes.Private);
                    fieldMap[prop] = fld;
                } else {
                    throw new Exception("property type not supported");
                }
            }

            if (methodSet.Count > 0)
                throw new Exception("methods are not supported");

            var cons = tgen.DefineConstructor(MethodAttributes.Public,
                CallingConventions.Standard, null);
            var cgen = cons.GetILGenerator();

            foreach (var prop in fieldMap.Keys) {
                var fld = fieldMap[prop];
                if (fld.FieldType != prop.PropertyType) {
                    // this.$ = new Scalar<>
                    cgen.Emit(OpCodes.Ldarg_0);
                    var c = fld.FieldType.GetConstructor(new Type[0]);
                    cgen.Emit(OpCodes.Newobj, c);
                    cgen.Emit(OpCodes.Stfld, fld);

                    // this.Assigned("Property", this.$, null)
                    cgen.Emit(OpCodes.Ldarg_0);
                    cgen.Emit(OpCodes.Ldstr, prop.Name);
                    cgen.Emit(OpCodes.Ldarg_0);
                    cgen.Emit(OpCodes.Ldfld, fld);
                    cgen.Emit(OpCodes.Ldnull);
                    cgen.EmitCall(OpCodes.Callvirt, Assigned, null);
                }
            }

            cgen.Emit(OpCodes.Ret);

            var mattr = MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.SpecialName | MethodAttributes.HideBySig;
            foreach (var prop in fieldMap.Keys) {
                var fld = fieldMap[prop];

                var pget = prop.GetGetMethod();
                var pset = prop.GetSetMethod();

                var pgen = tgen.DefineProperty(prop.Name, PropertyAttributes.None, prop.PropertyType, null);

                var get = tgen.DefineMethod(pget.Name, mattr, prop.PropertyType, null);
                var set = tgen.DefineMethod(pset.Name, mattr, null, new[] { prop.PropertyType });
                var getGen = get.GetILGenerator();
                var setGen = set.GetILGenerator();

                if (fld.FieldType == prop.PropertyType) {
                    // get => this.$
                    getGen.Emit(OpCodes.Ldarg_0);
                    getGen.Emit(OpCodes.Ldfld, fld);

                    // set =>
                    // prev = this.$
                    var prev = setGen.DeclareLocal(fld.FieldType);
                    setGen.Emit(OpCodes.Ldarg_0);
                    setGen.Emit(OpCodes.Ldfld, fld);
                    setGen.Emit(OpCodes.Stloc, prev);

                    // this.$ = value
                    setGen.Emit(OpCodes.Ldarg_0);
                    setGen.Emit(OpCodes.Ldarg_1);
                    setGen.Emit(OpCodes.Stfld, fld);

                    // this.Assigned("Property", value, prev)
                    setGen.Emit(OpCodes.Ldarg_0);
                    setGen.Emit(OpCodes.Ldstr, prop.Name);
                    setGen.Emit(OpCodes.Ldarg_1);
                    setGen.Emit(OpCodes.Ldloc, prev);
                    setGen.EmitCall(OpCodes.Callvirt, Assigned, null);
                } else {
                    var scalarVal = fld.FieldType.GetProperty("Value");

                    // get => this.$.Value
                    getGen.Emit(OpCodes.Ldarg_0);
                    getGen.Emit(OpCodes.Ldfld, fld);
                    getGen.EmitCall(OpCodes.Call, scalarVal.GetGetMethod(), null);

                    // set => this.$.Value = value
                    setGen.Emit(OpCodes.Ldarg_0);
                    setGen.Emit(OpCodes.Ldfld, fld);
                    setGen.Emit(OpCodes.Ldarg_1);
                    setGen.EmitCall(OpCodes.Call, scalarVal.GetSetMethod(), null);
                }

                getGen.Emit(OpCodes.Ret);
                setGen.Emit(OpCodes.Ret);

                pgen.SetGetMethod(get);
                pgen.SetSetMethod(set);
                tgen.DefineMethodOverride(get, pget);
                tgen.DefineMethodOverride(set, pset);
            }

            var write = tgen.DefineMethod(WriteValue.Name, WriteValue.Attributes & ~MethodAttributes.Abstract,
                WriteValue.CallingConvention,
                WriteValue.ReturnType,
                new[] { WriteValue.GetParameters()[0].ParameterType });
            write.DefineParameter(0, ParameterAttributes.None, "w");

            var wgen = write.GetILGenerator();
            wgen.Emit(OpCodes.Ldarg_1);
            wgen.EmitCall(OpCodes.Call, WriteObjectStart, null);

            foreach (var prop in fieldMap.Keys) {
                var fld = fieldMap[prop];

                if (fld.FieldType == prop.PropertyType) {
                    // if(this.$ is ModelBase m) {
                    var loc = wgen.DeclareLocal(typeof(ModelBase));
                    var isinstFalse = wgen.DefineLabel();

                    wgen.Emit(OpCodes.Ldarg_0);
                    wgen.Emit(OpCodes.Ldfld, fld);
                    wgen.Emit(OpCodes.Isinst, typeof(ModelBase));
                    wgen.Emit(OpCodes.Stloc, loc);
                    wgen.Emit(OpCodes.Ldloc, loc);
                    wgen.Emit(OpCodes.Brfalse_S, isinstFalse);

                    //     writer.WritePropertyName("Property")
                    wgen.Emit(OpCodes.Ldarg_1);
                    wgen.Emit(OpCodes.Ldstr, prop.Name);
                    wgen.EmitCall(OpCodes.Call, WritePropertyName, null);

                    //     m.WriteValue(ref writer)
                    wgen.Emit(OpCodes.Ldloc, loc);
                    wgen.Emit(OpCodes.Ldarg_1);
                    wgen.EmitCall(OpCodes.Callvirt, WriteValue, null);

                    // }
                    wgen.MarkLabel(isinstFalse);
                } else {
                    // writer.WritePropertyName("Property")
                    wgen.Emit(OpCodes.Ldarg_1);
                    wgen.Emit(OpCodes.Ldstr, prop.Name);
                    wgen.EmitCall(OpCodes.Call, WritePropertyName, null);

                    // this.$.WriteValue(ref writer)
                    wgen.Emit(OpCodes.Ldarg_0);
                    wgen.Emit(OpCodes.Ldfld, fld);
                    wgen.Emit(OpCodes.Ldarg_1);
                    wgen.EmitCall(OpCodes.Callvirt, WriteValue, null);
                }
            }

            wgen.Emit(OpCodes.Ldarg_1);
            wgen.EmitCall(OpCodes.Call, WriteObjectEnd, null);
            wgen.Emit(OpCodes.Ret);

            tgen.DefineMethodOverride(write, WriteValue);

            return tgen.CreateType();
        }

        static Factory() {
            ModelTypeMap = new Dictionary<Type, Type>();
            ScalarTypeMap = new Dictionary<Type, Type> {
                { typeof(bool), typeof(ScalarBool) },
                { typeof(sbyte), typeof(ScalarSByte) },
                { typeof(byte), typeof(ScalarByte) },
                { typeof(ushort), typeof(ScalarUShort) },
                { typeof(short), typeof(ScalarShort) },
                { typeof(uint), typeof(ScalarUInt) },
                { typeof(int), typeof(ScalarInt) },
                { typeof(ulong), typeof(ScalarULong) },
                { typeof(long), typeof(ScalarLong) },
                { typeof(float), typeof(ScalarFloat) },
                { typeof(double), typeof(ScalarDouble) },
                { typeof(bool?), typeof(ScalarNBool) },
                { typeof(sbyte?), typeof(ScalarNSByte) },
                { typeof(byte?), typeof(ScalarNByte) },
                { typeof(ushort?), typeof(ScalarNUShort) },
                { typeof(short?), typeof(ScalarNShort) },
                { typeof(uint?), typeof(ScalarNUInt) },
                { typeof(int?), typeof(ScalarNInt) },
                { typeof(ulong?), typeof(ScalarNULong) },
                { typeof(long?), typeof(ScalarNLong) },
                { typeof(float?), typeof(ScalarNFloat) },
                { typeof(double?), typeof(ScalarNDouble) },
                { typeof(string), typeof(ScalarString) }
            };

            var asmName = new AssemblyName("FactoryImpls");
            var asm = AssemblyBuilder.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.Run);
            mod = asm.DefineDynamicModule(asmName.Name);

            WriteObjectStart = typeof(JsonWriter).GetMethod("WriteObjectStart");
            WriteObjectEnd = typeof(JsonWriter).GetMethod("WriteObjectEnd");
            WritePropertyName = typeof(JsonWriter).GetMethod("WritePropertyName",
                new[] { typeof(string) });
            WriteValue = typeof(ModelBase).GetMethod("WriteValue");
            Assigned = typeof(ModelBase).GetMethod("Assigned",
                BindingFlags.NonPublic | BindingFlags.Instance);
        }
    }
}