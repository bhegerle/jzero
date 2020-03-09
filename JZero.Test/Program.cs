using System;
using System.Collections.Generic;
using System.Reflection;
using JZero;
using JZero.Model;
using JZero.Model.Impl;

namespace JZero.Test {
    class Program {
        static void Main(string[] args) {
            var f = BindingFlags.Static | BindingFlags.NonPublic;
            foreach (var m in typeof(Program).GetMethods(f)) {
                if (m.Name.EndsWith("Test")) {
                    Console.WriteLine(m.Name);
                    m.Invoke(null, null);
                }
            }
        }

        static void JsonTest() {
            var json = @"{""X"":[true, false, 9, null,[[{}]]],""Y"":{},""Z"":[]}";

            Console.WriteLine($"JSON: {json}");
            Console.WriteLine("tokens:");
            foreach (var t in new JsonTokenizer(json)) {
                Console.WriteLine($"    {t}");
            }

            var rdr = new JsonReader(json);
            rdr.ReadObjectStart();
            {
                AssertEqual(rdr.ReadPropertyName(), "X");
                rdr.ReadArrayStart();
                {
                    AssertEqual(rdr.ReadBool(), true);
                    AssertEqual(rdr.NextElement(), true);
                    AssertEqual(rdr.ReadBool(), false);
                    AssertEqual(rdr.NextElement(), true);
                    AssertEqual(rdr.ReadInt(), 9);
                    AssertEqual(rdr.NextElement(), true);
                    rdr.ReadNull();
                    AssertEqual(rdr.NextElement(), true);

                    rdr.ReadArrayStart();
                    rdr.ReadArrayStart();
                    rdr.ReadObjectStart();
                    AssertEqual(rdr.NextProperty(), false);
                    AssertEqual(rdr.NextElement(), false);
                    AssertEqual(rdr.NextElement(), false);
                }
                AssertEqual(rdr.NextElement(), false);

                AssertEqual(rdr.NextProperty(), true);
                AssertEqual(rdr.ReadPropertyName(), "Y");
                rdr.ReadObjectStart();
                AssertEqual(rdr.NextProperty(), false);

                AssertEqual(rdr.NextProperty(), true);
                AssertEqual(rdr.ReadPropertyName(), "Z");
                rdr.ReadArrayStart();
                AssertEqual(rdr.NextElement(), false);
            }
            AssertEqual(rdr.NextProperty(), false);
            rdr.ReadEof();

            var wrtr = new JsonWriter(new char[json.Length]);
            wrtr.WriteObjectStart();
            {
                wrtr.WritePropertyName("X");
                wrtr.WriteArrayStart();
                {
                    wrtr.Write(true);
                    wrtr.Write(false);
                    wrtr.Write(9);
                    wrtr.WriteNull();
                    wrtr.WriteArrayStart();
                    wrtr.WriteArrayStart();
                    wrtr.WriteObjectStart();
                    wrtr.WriteObjectEnd();
                    wrtr.WriteArrayEnd();
                    wrtr.WriteArrayEnd();
                }
                wrtr.WriteArrayEnd();

                wrtr.WritePropertyName("Y");
                wrtr.WriteObjectStart();
                wrtr.WriteObjectEnd();

                wrtr.WritePropertyName("Z");
                wrtr.WriteArrayStart();
                wrtr.WriteArrayEnd();
            }
            wrtr.WriteObjectEnd();
            Console.WriteLine($"wrote {wrtr.WrittenString}");

            var quotingTests = new[]{
                ("\"\"", "", true),
                (@"""xy""", "xy", true),
                (@"""\\\""""", @"\""", true),
                (@"""\u0033""", "3", false)
            };

            foreach (var (qstring, ustring, doQuote) in quotingTests) {
                AssertEqual(Quoting.Unquote(qstring), ustring);
                if (doQuote)
                    AssertEqual(Quoting.Quote(ustring), qstring);
            }

            foreach (var s in NumberStringTestCases()) {
                var tokEnum = new JsonTokenizer(s).GetEnumerator();
                tokEnum.MoveNext();
                AssertEqual(JsonToken.Number, tokEnum.Current);
                var cons = tokEnum.CurrentSegment.Count;
                if (cons != s.Length)
                    throw new Exception($"only consumed {cons} chars of {s}");
            }
        }

        static void SymbolTableTest() {
            var syms = new Dictionary<string, int> {
                { "Name", 0 },
                { "Event", 1 },
                { "Id", 2 },
                { "Score", 3 },
                { "Rank", 4 }
            };

            var tbl = new SymbolTable<int>(syms);
            AssertEqual(tbl.Fallback, false);
            foreach (var k in syms.Keys)
                AssertEqual(tbl[k], syms[k]);
        }

        static IEnumerable<string> NumberStringTestCases() {
            foreach (var sign in new[] { "", "-" })
                foreach (var intPart in new[] { 0, 1, 33 })
                    foreach (var fracPart in new[] { "", ".0", ".00", ".999" })
                        foreach (var exp in new[] { "", "e", "E" })
                            foreach (var expSign in new[] { "", "+", "-" })
                                foreach (var expPow in new[] { "0", "00", "88" }) {
                                    var s = sign + intPart + fracPart;
                                    if (exp.Length > 0)
                                        s += exp + expSign + expPow;
                                    yield return s;
                                }
        }

        static void Write(ModelBase model, bool hasValue) {
            var w = new JsonWriter(new char[10000]);
            w.WriteObjectStart();
            w.WritePropertyName("Key");
            model.WritePath(ref w);
            w.WritePropertyName("Value");
            if (hasValue)
                model.WriteValue(ref w);
            else
                w.WriteNull();
            w.WriteObjectEnd();
            Console.WriteLine(w.WrittenString);
        }

        static void ModelTest() {
            var a = new Root<IArray<IArray<int>>>();
            a.Changed += Write;
            a.Model[0] = New.Array<int>();
            for (var i = 0; i < 300; i++)
                a.Model[0][i] = i;
            Console.WriteLine(a);

            var b = new Root<IDict<IDict<double>>>();
            b.Changed += Write;
            b.Model["x"] = New.Dict<double>();
            b.Model["x"]["y"] = 1;
            b.Model["x"]["z"] = 1.2;
            Console.WriteLine(b);

            var sm = new Root<ISubModel>();
            sm.Changed += Write;
            sm.Model.X = 88;
            Console.WriteLine(sm);

            var foo = new Root<IModelFoo>();
            foo.Changed += Write;
            foo.Model.Name = "a";
            foo.Model.Flags = New.Dict<bool>();
            foo.Model.Flags["Flag0"] = true;
            foo.Model.Flags["Flag1"] = false;
            foo.Model.Floats = New.Array<float>();
            foo.Model.Floats[2] = 2.3f;
            foo.Model.SubArray = New.Array<ISubModel>();
            foo.Model.SubArray[1] = New.Model<ISubModel>();
            Console.WriteLine(foo);
        }

        static void TypeBuilderTest() {
            var foo = New.Model<IFoo>();
            foo.X = 9;
            Console.WriteLine(foo.X);
            foo.SubModel = New.Model<ISubModel>();
            foo.SubModel.X = 7;
            Console.WriteLine(foo);
        }

        static void AssertEqual<T>(T actual, T expected) {
            if (!actual.Equals(expected))
                throw new Exception($"expected {actual} == {expected}");
        }

        static void AssertEqual(Span<char> actual, string expected) {
            AssertEqual(actual.ToString(), expected);
        }
    }
}
