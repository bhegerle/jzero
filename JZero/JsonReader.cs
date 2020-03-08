using System;

namespace JZero {
    public struct JsonReader {
        private JsonTokenEnumerator jsonEnum;
        private readonly ArraySegment<char> segment;

        public JsonReader(string s) : this(s.ToCharArray()) { }

        public JsonReader(ArraySegment<char> segment) {
            jsonEnum = new JsonTokenEnumerator(segment);
            this.segment = segment;
        }

        public JsonToken Token => jsonEnum.Current;

        public bool SkipNull() {
            if (jsonEnum.NextIsNull()) {
                ReadNull();
                return true;
            }
            return false;
        }

        public void ReadObjectStart() {
            Expect(JsonToken.ObjectStart, "expected object start");
        }

        public void ReadObjectEnd() {
            Expect(JsonToken.ObjectEnd, "expected object end");
        }

        public ArraySegment<char> ReadPropertyName() {
            Expect(JsonToken.String, "expected property name");
            var n = jsonEnum.CurrentSegment;
            Expect(JsonToken.Colon, "expected colon");
            return n;
        }

        public void ReadArrayStart() {
            Expect(JsonToken.ArrayStart, "expected array start");
        }

        public void ReadArrayEnd() {
            Expect(JsonToken.ArrayEnd, "expected array end");
        }

        public bool NextElement() {
            if (!jsonEnum.MoveNext())
                throw JsonEx("runaway array");

            switch (jsonEnum.Current) {
                case JsonToken.Comma:
                    return true;
                case JsonToken.ArrayEnd:
                    return false;
                default:
                    throw JsonEx("expected comma or array end");
            }
        }

        public bool NextProperty() {
            if (!jsonEnum.MoveNext())
                throw JsonEx("runaway object");

            switch (jsonEnum.Current) {
                case JsonToken.Comma:
                    return true;
                case JsonToken.ObjectEnd:
                    return false;
                default:
                    throw JsonEx("expected comma or object end");
            }
        }

        public object ReadNull() {
            Expect(JsonToken.Null, "expected null");
            return null;
        }

        public bool ReadBool() {
            Expect(JsonToken.Bool, "expected true or false");
            return jsonEnum.CurrentSegment[0] == 't';
        }

        public ArraySegment<char> ReadString() {
            if (SkipNull())
                return null;
            Expect(JsonToken.String, "expected string");
            return jsonEnum.CurrentSegment;
        }

        public sbyte ReadSbyte() { ExpectNum(); return sbyte.Parse(jsonEnum.CurrentSpan); }
        public byte ReadByte() { ExpectNum(); return byte.Parse(jsonEnum.CurrentSpan); }
        public short ReadShort() { ExpectNum(); return short.Parse(jsonEnum.CurrentSpan); }
        public ushort ReadUShort() { ExpectNum(); return ushort.Parse(jsonEnum.CurrentSpan); }
        public int ReadInt() { ExpectNum(); return int.Parse(jsonEnum.CurrentSpan); }
        public uint ReadUInt() { ExpectNum(); return uint.Parse(jsonEnum.CurrentSpan); }
        public long ReadLong() { ExpectNum(); return long.Parse(jsonEnum.CurrentSpan); }
        public ulong ReadULong() { ExpectNum(); return ulong.Parse(jsonEnum.CurrentSpan); }
        public float ReadFloat() { ExpectNum(); return float.Parse(jsonEnum.CurrentSpan); }
        public double ReadDouble() { ExpectNum(); return double.Parse(jsonEnum.CurrentSpan); }

        public bool? ReadNBool() { if (SkipNull()) return null; return ReadBool(); }
        public sbyte? ReadNSbyte() { if (SkipNull()) return null; return ReadSbyte(); }
        public byte? ReadNByte() { if (SkipNull()) return null; return ReadByte(); }
        public short? ReadNShort() { if (SkipNull()) return null; return ReadShort(); }
        public ushort? ReadNUShort() { if (SkipNull()) return null; return ReadUShort(); }
        public int? ReadNInt() { if (SkipNull()) return null; return ReadInt(); }
        public uint? ReadNUInt() { if (SkipNull()) return null; return ReadUInt(); }
        public long? ReadNLong() { if (SkipNull()) return null; return ReadLong(); }
        public ulong? ReadNULong() { if (SkipNull()) return null; return ReadULong(); }
        public float? ReadNFloat() { if (SkipNull()) return null; return ReadFloat(); }
        public double? ReadNDouble() { if (SkipNull()) return null; return ReadDouble(); }

        public void ReadEof() {
            Expect(JsonToken.Eof, "expected EOF");
        }

        private void ExpectNum() { Expect(JsonToken.Number, "expected number"); }

        private void Expect(JsonToken expect, string errorMsg) {
            if (!(jsonEnum.MoveNext() && jsonEnum.Current == expect))
                throw JsonEx(errorMsg);
        }

        private JsonException JsonEx(string errorMsg) {
            return new JsonException(segment, jsonEnum.CurrentSegment.Offset, false, errorMsg);
        }
    }
}