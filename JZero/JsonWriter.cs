using System;
using System.Collections.Generic;

namespace JZero {
    public struct JsonWriter {
        private JsonFormatter jfmt;
        private bool needSep;

        public JsonWriter(char[] buffer) : this(new ArraySegment<char>(buffer)) { }

        public JsonWriter(ArraySegment<char> segment) {
            jfmt = new JsonFormatter(segment);
            needSep = false;
        }

        public ArraySegment<char> Written => jfmt.Written;
        public ArraySegment<char> Remaining => jfmt.Remaining;

        public string WrittenString
            => new string(Written.Array, Written.Offset, Written.Count);

        public void WriteObjectStart() { Sep(); jfmt.WriteObjectStart(); needSep = false; }
        public void WriteArrayStart() { Sep(); jfmt.WriteArrayStart(); needSep = false; }

        public void WriteObjectEnd() { jfmt.WriteObjectEnd(); needSep = true; }
        public void WriteArrayEnd() { jfmt.WriteArrayEnd(); needSep = true; }

        public void WritePropertyName<T>(T e) where T : Enum { }

        public void WritePropertyName(string s) {
            WritePropertyName(s.AsSpan());
        }

        public void WritePropertyName(ReadOnlySpan<char> s) {
            Sep();
            jfmt.Write(s);
            jfmt.WriteColon();
            needSep = false;
        }

        public void WriteNull() { Sep(); jfmt.WriteNull(); }

        public void Write(ReadOnlySpan<char> s) {
            Sep();
            if (s != null) jfmt.Write(s);
            else jfmt.WriteNull();
        }

        public void Write(bool b) { Sep(); jfmt.Write(b); }
        public void Write(byte i) { Sep(); jfmt.Write(i); }
        public void Write(sbyte i) { Sep(); jfmt.Write(i); }
        public void Write(short i) { Sep(); jfmt.Write(i); }
        public void Write(ushort i) { Sep(); jfmt.Write(i); }
        public void Write(int i) { Sep(); jfmt.Write(i); }
        public void Write(uint i) { Sep(); jfmt.Write(i); }
        public void Write(long i) { Sep(); jfmt.Write(i); }
        public void Write(ulong i) { Sep(); jfmt.Write(i); }
        public void Write(float i) { Sep(); jfmt.Write(i); }
        public void Write(double i) { Sep(); jfmt.Write(i); }

        public void Write(bool? b) { if (b != null) Write(b.Value); else WriteNull(); }
        public void Write(byte? i) { if (i != null) Write(i.Value); else WriteNull(); }
        public void Write(sbyte? i) { if (i != null) Write(i.Value); else WriteNull(); }
        public void Write(short? i) { if (i != null) Write(i.Value); else WriteNull(); }
        public void Write(ushort? i) { if (i != null) Write(i.Value); else WriteNull(); }
        public void Write(int? i) { if (i != null) Write(i.Value); else WriteNull(); }
        public void Write(uint? i) { if (i != null) Write(i.Value); else WriteNull(); }
        public void Write(long? i) { if (i != null) Write(i.Value); else WriteNull(); }
        public void Write(ulong? i) { if (i != null) Write(i.Value); else WriteNull(); }
        public void Write(float? i) { if (i != null) Write(i.Value); else WriteNull(); }
        public void Write(double? i) { if (i != null) Write(i.Value); else WriteNull(); }

        #region Write(IEnumerable)
        public void Write(IEnumerable<string> seq) {
            WriteArrayStart(); foreach (var e in seq) Write(e); WriteArrayEnd();
        }

        public void Write(IEnumerable<bool> seq) {
            WriteArrayStart(); foreach (var e in seq) Write(e); WriteArrayEnd();
        }
        public void Write(IEnumerable<byte> seq) {
            WriteArrayStart(); foreach (var e in seq) Write(e); WriteArrayEnd();
        }
        public void Write(IEnumerable<sbyte> seq) {
            WriteArrayStart(); foreach (var e in seq) Write(e); WriteArrayEnd();
        }
        public void Write(IEnumerable<short> seq) {
            WriteArrayStart(); foreach (var e in seq) Write(e); WriteArrayEnd();
        }
        public void Write(IEnumerable<ushort> seq) {
            WriteArrayStart(); foreach (var e in seq) Write(e); WriteArrayEnd();
        }
        public void Write(IEnumerable<int> seq) {
            WriteArrayStart(); foreach (var e in seq) Write(e); WriteArrayEnd();
        }
        public void Write(IEnumerable<uint> seq) {
            WriteArrayStart(); foreach (var e in seq) Write(e); WriteArrayEnd();
        }
        public void Write(IEnumerable<long> seq) {
            WriteArrayStart(); foreach (var e in seq) Write(e); WriteArrayEnd();
        }
        public void Write(IEnumerable<ulong> seq) {
            WriteArrayStart(); foreach (var e in seq) Write(e); WriteArrayEnd();
        }
        public void Write(IEnumerable<float> seq) {
            WriteArrayStart(); foreach (var e in seq) Write(e); WriteArrayEnd();
        }
        public void Write(IEnumerable<double> seq) {
            WriteArrayStart(); foreach (var e in seq) Write(e); WriteArrayEnd();
        }

        public void Write(IEnumerable<bool?> seq) {
            WriteArrayStart(); foreach (var e in seq) Write(e); WriteArrayEnd();
        }
        public void Write(IEnumerable<byte?> seq) {
            WriteArrayStart(); foreach (var e in seq) Write(e); WriteArrayEnd();
        }
        public void Write(IEnumerable<sbyte?> seq) {
            WriteArrayStart(); foreach (var e in seq) Write(e); WriteArrayEnd();
        }
        public void Write(IEnumerable<short?> seq) {
            WriteArrayStart(); foreach (var e in seq) Write(e); WriteArrayEnd();
        }
        public void Write(IEnumerable<ushort?> seq) {
            WriteArrayStart(); foreach (var e in seq) Write(e); WriteArrayEnd();
        }
        public void Write(IEnumerable<int?> seq) {
            WriteArrayStart(); foreach (var e in seq) Write(e); WriteArrayEnd();
        }
        public void Write(IEnumerable<uint?> seq) {
            WriteArrayStart(); foreach (var e in seq) Write(e); WriteArrayEnd();
        }
        public void Write(IEnumerable<long?> seq) {
            WriteArrayStart(); foreach (var e in seq) Write(e); WriteArrayEnd();
        }
        public void Write(IEnumerable<ulong?> seq) {
            WriteArrayStart(); foreach (var e in seq) Write(e); WriteArrayEnd();
        }
        public void Write(IEnumerable<float?> seq) {
            WriteArrayStart(); foreach (var e in seq) Write(e); WriteArrayEnd();
        }
        public void Write(IEnumerable<double?> seq) {
            WriteArrayStart(); foreach (var e in seq) Write(e); WriteArrayEnd();
        }
        #endregion

        private void Sep() {
            if (needSep) jfmt.WriteComma();
            else needSep = true;
        }
    }
}