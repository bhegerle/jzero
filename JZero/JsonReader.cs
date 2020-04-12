using System;

namespace JZero {
    /// <summary>
    /// Parser for JSON content. Example: 
    ///     var json = @"{""Foo"":9}";
    ///     var rdr = new JsonReader(json);
    ///     rdr.ReadObjectStart();
    ///     rdr.NextProperty(); // == true
    ///     rdr.ReadPropertyName(); // == "Foo"
    ///     rdr.ReadInt(); // == 9
    ///     rdr.NextProperty(); // == false
    ///     rdr.ReadEof();
    /// </summary>
    public struct JsonReader {
        private JsonTokenEnumerator jsonEnum;
        private readonly ArraySegment<char> segment;
        private bool start;

        /// <summary>
        /// Parse string containing JSON.
        /// </summary>
        public JsonReader(string s) : this(s.ToCharArray()) { }

        /// <summary>
        /// Parse a buffer in-place. Modifies the buffer when unquoting a string.
        /// </summary>
        public JsonReader(ArraySegment<char> segment) {
            start = false;
            jsonEnum = new JsonTokenEnumerator(segment);
            this.segment = segment;
        }

        /// <summary>
        /// Type of token at current position.
        /// </summary>
        public JsonToken Token => jsonEnum.Current;

        /// <summary>
        /// Segment containing the token at current position.
        /// </summary>
        public ArraySegment<char> TokenSegment => jsonEnum.CurrentSegment;

        /// <summary>
        /// Consume a left-bracket: the array start character.
        /// </summary>
        public void ReadArrayStart() {
            Expect(JsonToken.ArrayStart, "expected array start");
            start = true;
        }

        /// <summary>
        /// Consume a right-bracket: the array end character.
        /// </summary>
        public void ReadArrayEnd() {
            Expect(JsonToken.ArrayEnd, "expected array end");
        }

        /// <summary>
        /// Iterate through an array: return true while an element can be read.
        /// </summary>
        public bool NextElement() {
            if (start) {
                start = false;
                if (!jsonEnum.NextIsArrayEnd())
                    return true;
            }

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

        /// <summary>
        /// Consume a left-brace: the object start character.
        /// </summary>
        public void ReadObjectStart() {
            Expect(JsonToken.ObjectStart, "expected object start");
            start = true;
        }

        /// <summary>
        /// Consume a right-brace: the object start character.
        /// </summary>
        public void ReadObjectEnd() {
            Expect(JsonToken.ObjectEnd, "expected object end");
        }

        /// <summary>
        /// Consume and unquote a property name; return its segment.
        /// </summary>
        public ArraySegment<char> ReadPropertyName() {
            Expect(JsonToken.String, "expected property name");
            var n = jsonEnum.CurrentSegment;
            Expect(JsonToken.Colon, "expected colon");
            return n;
        }

        /// <summary>
        /// Iterate through an object: return true while an element can be read.
        /// </summary>
        public bool NextProperty() {
            if (start) {
                start = false;
                if (!jsonEnum.NextIsObjectEnd())
                    return true;
            }

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

        /// <summary>
        /// Consume a null value.
        /// </summary>
        public object ReadNull() {
            Expect(JsonToken.Null, "expected null");
            return null;
        }

        /// <summary>
        /// Consume and return a boolean value.
        /// </summary>
        public bool ReadBool() {
            Expect(JsonToken.Bool, "expected true or false");
            return jsonEnum.CurrentSegment[0] == 't';
        }

        /// <summary>
        /// Consume and unquote a string; return its segment.
        /// </summary>
        public ArraySegment<char> ReadString() {
            if (SkipNull())
                return null;
            Expect(JsonToken.String, "expected string");
            return jsonEnum.CurrentSegment;
        }

        /// <summary>
        /// Consume and return an sbyte value.
        /// </summary>
        public sbyte ReadSbyte() { ExpectNum(); return sbyte.Parse(jsonEnum.CurrentSpan); }

        /// <summary>
        /// Consume and return a byte value.
        /// </summary>
        public byte ReadByte() { ExpectNum(); return byte.Parse(jsonEnum.CurrentSpan); }

        /// <summary>
        /// Consume and return a short value.
        /// </summary>
        public short ReadShort() { ExpectNum(); return short.Parse(jsonEnum.CurrentSpan); }

        /// <summary>
        /// Consume and return a ushort value.
        /// </summary>
        public ushort ReadUShort() { ExpectNum(); return ushort.Parse(jsonEnum.CurrentSpan); }

        /// <summary>
        /// Consume and return an int value.
        /// </summary>
        public int ReadInt() { ExpectNum(); return int.Parse(jsonEnum.CurrentSpan); }

        /// <summary>
        /// Consume and return a uint value.
        /// </summary>
        public uint ReadUInt() { ExpectNum(); return uint.Parse(jsonEnum.CurrentSpan); }

        /// <summary>
        /// Consume and return a long value.
        /// </summary>
        public long ReadLong() { ExpectNum(); return long.Parse(jsonEnum.CurrentSpan); }

        /// <summary>
        /// Consume and return a ulong value.
        /// </summary>
        public ulong ReadULong() { ExpectNum(); return ulong.Parse(jsonEnum.CurrentSpan); }

        /// <summary>
        /// Consume and return a float value.
        /// </summary>
        public float ReadFloat() { ExpectNum(); return float.Parse(jsonEnum.CurrentSpan); }

        /// <summary>
        /// Consume and return a double value.
        /// </summary>
        public double ReadDouble() { ExpectNum(); return double.Parse(jsonEnum.CurrentSpan); }

        /// <summary>
        /// Consume and return a DateTime value.
        /// </summary>
        public DateTime ReadDateTime() { return DateTime.Parse(ReadString()); }

        /// <summary>
        /// Consume and return a bool? value.
        /// </summary>
        public bool? ReadNBool() { if (SkipNull()) return null; return ReadBool(); }

        /// <summary>
        /// Consume and return an sbyte? value.
        /// </summary>
        public sbyte? ReadNSbyte() { if (SkipNull()) return null; return ReadSbyte(); }

        /// <summary>
        /// Consume and return a byte? value.
        /// </summary>
        public byte? ReadNByte() { if (SkipNull()) return null; return ReadByte(); }

        /// <summary>
        /// Consume and return a short? value.
        /// </summary>
        public short? ReadNShort() { if (SkipNull()) return null; return ReadShort(); }

        /// <summary>
        /// Consume and return a ushort? value.
        /// </summary>
        public ushort? ReadNUShort() { if (SkipNull()) return null; return ReadUShort(); }

        /// <summary>
        /// Consume and return an int? value.
        /// </summary>
        public int? ReadNInt() { if (SkipNull()) return null; return ReadInt(); }

        /// <summary>
        /// Consume and return a uint? value.
        /// </summary>
        public uint? ReadNUInt() { if (SkipNull()) return null; return ReadUInt(); }

        /// <summary>
        /// Consume and return a long? value.
        /// </summary>
        public long? ReadNLong() { if (SkipNull()) return null; return ReadLong(); }

        /// <summary>
        /// Consume and return a ulong? value.
        /// </summary>
        public ulong? ReadNULong() { if (SkipNull()) return null; return ReadULong(); }

        /// <summary>
        /// Consume and return a float? value.
        /// </summary>
        public float? ReadNFloat() { if (SkipNull()) return null; return ReadFloat(); }

        /// <summary>
        /// Consume and return a double? value.
        /// </summary>
        public double? ReadNDouble() { if (SkipNull()) return null; return ReadDouble(); }

        /// <summary>
        /// Consume and return a DateTime? value.
        /// </summary>
        public DateTime? ReadNDateTime() {
            var s = ReadString();
            if (s == null) return null;
            return DateTime.Parse(s);
        }

        /// <summary>
        /// Read any token.
        /// </summary>
        public ArraySegment<char> ReadArraySegment() {
            if (!jsonEnum.MoveNext())
                throw JsonEx("expected token");
            return jsonEnum.CurrentSegment;
        }

        /// <summary>
        /// Consume the end-of-buffer sentinel.
        /// </summary>
        public void ReadEof() {
            Expect(JsonToken.Eof, "expected EOF");
        }

        private bool SkipNull() {
            if (jsonEnum.NextIsNull()) {
                ReadNull();
                return true;
            }
            return false;
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