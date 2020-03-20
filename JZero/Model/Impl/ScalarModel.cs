namespace JZero.Model.Impl {
    /// <summary>
    /// Container for a single scalar which can trigger changed events. 
    /// </summary>
    public abstract class ScalarModel<T> : ModelBase {
        private T val;

        /// <summary>
        /// Gets the current value, or set it and trigger any changed events.
        /// </summary>
        public T Value {
            get => val;
            set {
                val = value;
                TriggerChangedEvents(this, value != null);
            }
        }

        /// <summary>
        /// Construct model with specified initial value. 
        /// </summary>
        protected ScalarModel(T val) { this.val = val; }
    }

    /// <summary>
    /// Container for a single bool.
    /// </summary>
    public sealed class ScalarBool : ScalarModel<bool> {
        /// <summary>
        /// Construct a model which contains a false.
        /// </summary>
        public ScalarBool() : base(default(bool)) { }

        /// <summary>
        /// Construct a model which contains the bool value.
        /// </summary>
        public ScalarBool(bool value) : base(value) { }

        /// <summary>
        /// Write the current model value to the JsonWriter.
        /// </summary>
        public override void WriteValue(ref JsonWriter w) { w.Write(Value); }
    }

    /// <summary>
    /// Container for a single sbyte.
    /// </summary>
    public sealed class ScalarSByte : ScalarModel<sbyte> {
        /// <summary>
        /// Construct a model which contains a 0 sbyte.
        /// </summary>
        public ScalarSByte() : base(default(sbyte)) { }

        /// <summary>
        /// Construct a model which contains the sbyte value.
        /// </summary>
        public ScalarSByte(sbyte value) : base(value) { }

        /// <summary>
        /// Write the current model value to the JsonWriter.
        /// </summary>
        public override void WriteValue(ref JsonWriter w) { w.Write(Value); }
    }

    /// <summary>
    /// Container for a single byte.
    /// </summary>
    public sealed class ScalarByte : ScalarModel<byte> {
        /// <summary>
        /// Construct a model which contains a 0 byte.
        /// </summary>
        public ScalarByte() : base(default(byte)) { }

        /// <summary>
        /// Construct a model which contains the byte value.
        /// </summary>
        public ScalarByte(byte value) : base(value) { }

        /// <summary>
        /// Write the current model value to the JsonWriter.
        /// </summary>
        public override void WriteValue(ref JsonWriter w) { w.Write(Value); }
        
    }

    /// <summary>
    /// Container for a single ushort.
    /// </summary>
    public sealed class ScalarUShort : ScalarModel<ushort> {
        /// <summary>
        /// Construct a model which contains a 0 ushort.
        /// </summary>
        public ScalarUShort() : base(default(ushort)) { }

        /// <summary>
        /// Construct a model which contains the ushort value.
        /// </summary>
        public ScalarUShort(ushort value) : base(value) { }

        /// <summary>
        /// Write the current model value to the JsonWriter.
        /// </summary>
        public override void WriteValue(ref JsonWriter w) { w.Write(Value); }
    }

    /// <summary>
    /// Container for a single short.
    /// </summary>
    public sealed class ScalarShort : ScalarModel<short> {
        /// <summary>
        /// Construct a model which contains a 0 short.
        /// </summary>
        public ScalarShort() : base(default(short)) { }

        /// <summary>
        /// Construct a model which contains the short value.
        /// </summary>
        public ScalarShort(short value) : base(value) { }

        /// <summary>
        /// Write the current model value to the JsonWriter.
        /// </summary>
        public override void WriteValue(ref JsonWriter w) { w.Write(Value); }
    }

    /// <summary>
    /// Container for a single uint.
    /// </summary>
    public sealed class ScalarUInt : ScalarModel<uint> {
        /// <summary>
        /// Construct a model which contains a 0 uint.
        /// </summary>
        public ScalarUInt() : base(default(uint)) { }

        /// <summary>
        /// Construct a model which contains the uint value.
        /// </summary>
        public ScalarUInt(uint value) : base(value) { }

        /// <summary>
        /// Write the current model value to the JsonWriter.
        /// </summary>
        public override void WriteValue(ref JsonWriter w) { w.Write(Value); }
    }

    /// <summary>
    /// Container for a single int.
    /// </summary>
    public sealed class ScalarInt : ScalarModel<int> {
        /// <summary>
        /// Construct a model which contains a 0 int.
        /// </summary>
        public ScalarInt() : base(default(int)) { }

        /// <summary>
        /// Construct a model which contains the int value.
        /// </summary>
        public ScalarInt(int value) : base(value) { }

        /// <summary>
        /// Write the current model value to the JsonWriter.
        /// </summary>
        public override void WriteValue(ref JsonWriter w) { w.Write(Value); }
    }

    /// <summary>
    /// Container for a single ulong.
    /// </summary>
    public sealed class ScalarULong : ScalarModel<ulong> {
        /// <summary>
        /// Construct a model which contains a 0 ulong.
        /// </summary>
        public ScalarULong() : base(default(ulong)) { }

        /// <summary>
        /// Construct a model which contains the ulong value.
        /// </summary>
        public ScalarULong(ulong value) : base(value) { }

        /// <summary>
        /// Write the current model value to the JsonWriter.
        /// </summary>
        public override void WriteValue(ref JsonWriter w) { w.Write(Value); }
    }

    /// <summary>
    /// Container for a single long.
    /// </summary>
    public sealed class ScalarLong : ScalarModel<long> {
        /// <summary>
        /// Construct a model which contains a 0 long.
        /// </summary>
        public ScalarLong() : base(default(long)) { }

        /// <summary>
        /// Construct a model which contains the long value.
        /// </summary>
        public ScalarLong(long value) : base(value) { }

        /// <summary>
        /// Write the current model value to the JsonWriter.
        /// </summary>
        public override void WriteValue(ref JsonWriter w) { w.Write(Value); }
    }

    /// <summary>
    /// Container for a single float.
    /// </summary>
    public sealed class ScalarFloat : ScalarModel<float> {
        /// <summary>
        /// Construct a model which contains a 0 float.
        /// </summary>
        public ScalarFloat() : base(default(float)) { }

        /// <summary>
        /// Construct a model which contains the float value.
        /// </summary>
        public ScalarFloat(float value) : base(value) { }

        /// <summary>
        /// Write the current model value to the JsonWriter.
        /// </summary>
        public override void WriteValue(ref JsonWriter w) { w.Write(Value); }
    }

    /// <summary>
    /// Container for a single double.
    /// </summary>
    public sealed class ScalarDouble : ScalarModel<double> {
        /// <summary>
        /// Construct a model which contains a 0 double.
        /// </summary>
        public ScalarDouble() : base(default(double)) { }

        /// <summary>
        /// Construct a model which contains the double value.
        /// </summary>
        public ScalarDouble(double value) : base(value) { }

        /// <summary>
        /// Write the current model value to the JsonWriter.
        /// </summary>
        public override void WriteValue(ref JsonWriter w) { w.Write(Value); }
    }

    /// <summary>
    /// Container for a single bool?.
    /// </summary>
    public sealed class ScalarNBool : ScalarModel<bool?> {
        /// <summary>
        /// Construct a model which contains a null bool?.
        /// </summary>
        public ScalarNBool() : base(default(bool?)) { }

        /// <summary>
        /// Construct a model which contains the bool? value.
        /// </summary>
        public ScalarNBool(bool? value) : base(value) { }

        /// <summary>
        /// Write the current model value to the JsonWriter.
        /// </summary>
        public override void WriteValue(ref JsonWriter w) { w.Write(Value); }
    }

    /// <summary>
    /// Container for a single sbyte?.
    /// </summary>
    public sealed class ScalarNSByte : ScalarModel<sbyte?> {
        /// <summary>
        /// Construct a model which contains a null sbyte?.
        /// </summary>
        public ScalarNSByte() : base(default(sbyte?)) { }

        /// <summary>
        /// Construct a model which contains the sbyte? value.
        /// </summary>
        public ScalarNSByte(sbyte? value) : base(value) { }

        /// <summary>
        /// Write the current model value to the JsonWriter.
        /// </summary>
        public override void WriteValue(ref JsonWriter w) { w.Write(Value); }
    }

    /// <summary>
    /// Container for a single byte?.
    /// </summary>
    public sealed class ScalarNByte : ScalarModel<byte?> {
        /// <summary>
        /// Construct a model which contains a null byte?.
        /// </summary>
        public ScalarNByte() : base(default(byte?)) { }

        /// <summary>
        /// Construct a model which contains the byte? value.
        /// </summary>
        public ScalarNByte(byte? value) : base(value) { }

        /// <summary>
        /// Write the current model value to the JsonWriter.
        /// </summary>
        public override void WriteValue(ref JsonWriter w) { w.Write(Value); }
    }

    /// <summary>
    /// Container for a single ushort?.
    /// </summary>
    public sealed class ScalarNUShort : ScalarModel<ushort?> {
        /// <summary>
        /// Construct a model which contains a null ushort?.
        /// </summary>
        public ScalarNUShort() : base(default(ushort?)) { }

        /// <summary>
        /// Construct a model which contains the ushort? value.
        /// </summary>
        public ScalarNUShort(ushort? value) : base(value) { }

        /// <summary>
        /// Write the current model value to the JsonWriter.
        /// </summary>
        public override void WriteValue(ref JsonWriter w) { w.Write(Value); }
    }

    /// <summary>
    /// Container for a single short?.
    /// </summary>
    public sealed class ScalarNShort : ScalarModel<short?> {
        /// <summary>
        /// Construct a model which contains a null short?.
        /// </summary>
        public ScalarNShort() : base(default(short?)) { }

        /// <summary>
        /// Construct a model which contains the short? value.
        /// </summary>
        public ScalarNShort(short? value) : base(value) { }

        /// <summary>
        /// Write the current model value to the JsonWriter.
        /// </summary>
        public override void WriteValue(ref JsonWriter w) { w.Write(Value); }
    }

    /// <summary>
    /// Container for a single uint?.
    /// </summary>
    public sealed class ScalarNUInt : ScalarModel<uint?> {
        /// <summary>
        /// Construct a model which contains a null uint?.
        /// </summary>
        public ScalarNUInt() : base(default(uint?)) { }

        /// <summary>
        /// Construct a model which contains the uint? value.
        /// </summary>
        public ScalarNUInt(uint? value) : base(value) { }

        /// <summary>
        /// Write the current model value to the JsonWriter.
        /// </summary>
        public override void WriteValue(ref JsonWriter w) { w.Write(Value); }
    }

    /// <summary>
    /// Container for a single int?.
    /// </summary>
    public sealed class ScalarNInt : ScalarModel<int?> {
        /// <summary>
        /// Construct a model which contains a null int?.
        /// </summary>
        public ScalarNInt() : base(default(int?)) { }

        /// <summary>
        /// Construct a model which contains the int? value.
        /// </summary>
        public ScalarNInt(int? value) : base(value) { }

        /// <summary>
        /// Write the current model value to the JsonWriter.
        /// </summary>
        public override void WriteValue(ref JsonWriter w) { w.Write(Value); }
    }

    /// <summary>
    /// Container for a single ulong?.
    /// </summary>
    public sealed class ScalarNULong : ScalarModel<ulong?> {
        /// <summary>
        /// Construct a model which contains a null ulong?.
        /// </summary>
        public ScalarNULong() : base(default(ulong?)) { }

        /// <summary>
        /// Construct a model which contains the ulong? value.
        /// </summary>
        public ScalarNULong(ulong? value) : base(value) { }

        /// <summary>
        /// Write the current model value to the JsonWriter.
        /// </summary>
        public override void WriteValue(ref JsonWriter w) { w.Write(Value); }
    }

    /// <summary>
    /// Container for a single long?.
    /// </summary>
    public sealed class ScalarNLong : ScalarModel<long?> {
        /// <summary>
        /// Construct a model which contains a null long?.
        /// </summary>
        public ScalarNLong() : base(default(long?)) { }

        /// <summary>
        /// Construct a model which contains the long? value.
        /// </summary>
        public ScalarNLong(long? value) : base(value) { }

        /// <summary>
        /// Write the current model value to the JsonWriter.
        /// </summary>
        public override void WriteValue(ref JsonWriter w) { w.Write(Value); }
    }

    /// <summary>
    /// Container for a single float?.
    /// </summary>
    public sealed class ScalarNFloat : ScalarModel<float?> {
        /// <summary>
        /// Construct a model which contains a null float?.
        /// </summary>
        public ScalarNFloat() : base(default(float?)) { }

        /// <summary>
        /// Construct a model which contains the float? value.
        /// </summary>
        public ScalarNFloat(float? value) : base(value) { }

        /// <summary>
        /// Write the current model value to the JsonWriter.
        /// </summary>
        public override void WriteValue(ref JsonWriter w) { w.Write(Value); }
    }

    /// <summary>
    /// Container for a single double?.
    /// </summary>
    public sealed class ScalarNDouble : ScalarModel<double?> {
        /// <summary>
        /// Construct a model which contains a null double?.
        /// </summary>
        public ScalarNDouble() : base(default(double?)) { }

        /// <summary>
        /// Construct a model which contains the double? value.
        /// </summary>
        public ScalarNDouble(double? value) : base(value) { }

        /// <summary>
        /// Write the current model value to the JsonWriter.
        /// </summary>
        public override void WriteValue(ref JsonWriter w) { w.Write(Value); }
    }

    /// <summary>
    /// Container for a single string.
    /// </summary>
    public sealed class ScalarString : ScalarModel<string> {
        /// <summary>
        /// Construct a model which contains a null string.
        /// </summary>
        public ScalarString() : base(default(string)) { }

        /// <summary>
        /// Construct a model which contains the string value.
        /// </summary>
        public ScalarString(string value) : base(value) { }

        /// <summary>
        /// Write the current model value to the JsonWriter.
        /// </summary>
        public override void WriteValue(ref JsonWriter w) { w.Write(Value); }
    }
}
