using System;
using System.Collections.Generic;

namespace JZero {
    /// <summary>
    /// Writer of JSON content.
    /// </summary>
    public struct JsonWriter {
        private JsonFormatter jfmt;
        private bool needSep;

        /// <summary>
        /// Construct a writer for the whole of a char array.
        /// </summary>
        public JsonWriter(char[] buffer) : this(new ArraySegment<char>(buffer)) { }

        /// <summary>
        /// Construct a writer for the whole of a sugment.
        /// </summary>
        public JsonWriter(ArraySegment<char> segment) {
            jfmt = new JsonFormatter(segment);
            needSep = false;
        }

        /// <summary>
        /// Slice of the buffer which has been written.
        /// </summary>
        public ArraySegment<char> Written => jfmt.Written;

        /// <summary>
        /// Slice of the buffer which has not been written.
        /// </summary>
        public ArraySegment<char> Remaining => jfmt.Remaining;

        /// <summary>
        /// New string from this.Written.
        /// </summary>
        public string WrittenString => new string(Written.Array, Written.Offset, Written.Count);

        /// <summary>
        /// Write a left-bracket: the array-start character. 
        /// </summary>
        public void WriteArrayStart() { Sep(); jfmt.WriteArrayStart(); needSep = false; }

        /// <summary>
        /// Write a right-bracket: the array-start character.
        /// </summary>
        public void WriteArrayEnd() { jfmt.WriteArrayEnd(); needSep = true; }

        /// <summary>
        /// Write a left-brace: the object-start character.
        /// </summary>
        public void WriteObjectStart() { Sep(); jfmt.WriteObjectStart(); needSep = false; }

        /// <summary>
        /// Write a right-brace: the object-start character.
        /// </summary>
        public void WriteObjectEnd() { jfmt.WriteObjectEnd(); needSep = true; }

        /// <summary>
        /// Write a property name from an enum value.
        /// </summary>
        public void WritePropertyName<T>(T e) where T : Enum { }

        /// <summary>
        /// Write a property name.
        /// </summary>
        public void WritePropertyName(string s) {
            WritePropertyName(s.AsSpan());
        }

        /// <summary>
        /// Write a property name.
        /// </summary>
        public void WritePropertyName(ReadOnlySpan<char> s) {
            Sep();
            jfmt.Write(s);
            jfmt.WriteColon();
            needSep = false;
        }

        /// <summary>
        /// Write a null value.
        /// </summary>
        public void WriteNull() { Sep(); jfmt.WriteNull(); }

        /// <summary>
        /// Write a string value.
        /// </summary>
        public void Write(ReadOnlySpan<char> s) {
            Sep();
            if (s != null) jfmt.Write(s);
            else jfmt.WriteNull();
        }

        /// <summary>
        /// Write a bool value.
        /// </summary>
        public void Write(bool b) { Sep(); jfmt.Write(b); }

        /// <summary>
        /// Write a byte value.
        /// </summary>
        public void Write(byte i) { Sep(); jfmt.Write(i); }

        /// <summary>
        /// Write an sbyte value.
        /// </summary>
        public void Write(sbyte i) { Sep(); jfmt.Write(i); }

        /// <summary>
        /// Write a short value.
        /// </summary>
        public void Write(short i) { Sep(); jfmt.Write(i); }

        /// <summary>
        /// Write a ushort value.
        /// </summary>
        public void Write(ushort i) { Sep(); jfmt.Write(i); }

        /// <summary>
        /// Write an int value.
        /// </summary>
        public void Write(int i) { Sep(); jfmt.Write(i); }

        /// <summary>
        /// Write a uint value.
        /// </summary>
        public void Write(uint i) { Sep(); jfmt.Write(i); }

        /// <summary>
        /// Write a long value.
        /// </summary>
        public void Write(long i) { Sep(); jfmt.Write(i); }

        /// <summary>
        /// Write a ulong value.
        /// </summary>
        public void Write(ulong i) { Sep(); jfmt.Write(i); }

        /// <summary>
        /// Write a float value.
        /// </summary>
        public void Write(float i) { Sep(); jfmt.Write(i); }

        /// <summary>
        /// Write a double value.
        /// </summary>
        public void Write(double i) { Sep(); jfmt.Write(i); }

        /// <summary>
        /// Write a DateTime value.
        /// </summary>
        public void Write(DateTime i) {
            Sep();
            var u=i.ToUniversalTime();
            jfmt.Write(u.ToString("yyyy-MM-ddTHH:mm:ssZ"));
        }

        /// <summary>
        /// Write a bool? value.
        /// </summary>
        public void Write(bool? b) { if (b != null) Write(b.Value); else WriteNull(); }

        /// <summary>
        /// Write a byte? value.
        /// </summary>
        public void Write(byte? i) { if (i != null) Write(i.Value); else WriteNull(); }

        /// <summary>
        /// Write an sbyte? value.
        /// </summary>
        public void Write(sbyte? i) { if (i != null) Write(i.Value); else WriteNull(); }

        /// <summary>
        /// Write a short? value.
        /// </summary>
        public void Write(short? i) { if (i != null) Write(i.Value); else WriteNull(); }

        /// <summary>
        /// Write a ushort? value.
        /// </summary>
        public void Write(ushort? i) { if (i != null) Write(i.Value); else WriteNull(); }

        /// <summary>
        /// Write a int? value.
        /// </summary>
        public void Write(int? i) { if (i != null) Write(i.Value); else WriteNull(); }

        /// <summary>
        /// Write a uint? value.
        /// </summary>
        public void Write(uint? i) { if (i != null) Write(i.Value); else WriteNull(); }

        /// <summary>
        /// Write a long? value.
        /// </summary>
        public void Write(long? i) { if (i != null) Write(i.Value); else WriteNull(); }

        /// <summary>
        /// Write a ulong? value.
        /// </summary>
        public void Write(ulong? i) { if (i != null) Write(i.Value); else WriteNull(); }

        /// <summary>
        /// Write a float? value.
        /// </summary>
        public void Write(float? i) { if (i != null) Write(i.Value); else WriteNull(); }

        /// <summary>
        /// Write a double? value.
        /// </summary>
        public void Write(double? i) { if (i != null) Write(i.Value); else WriteNull(); }

        /// <summary>
        /// Write a DateTime? value.
        /// </summary>
        public void Write(DateTime? i) { if (i != null) Write(i.Value); else WriteNull(); }

        #region Write(IEnumerable)
        /// <summary>
        /// Write each element in seq, separated by commas, within brackets. 
        /// </summary>
        public void Write(IEnumerable<string> seq) {
            WriteArrayStart(); foreach (var e in seq) Write(e); WriteArrayEnd();
        }

        /// <summary>
        /// Write each element in seq, separated by commas, within brackets. 
        /// </summary>
        public void Write(IEnumerable<bool> seq) {
            WriteArrayStart(); foreach (var e in seq) Write(e); WriteArrayEnd();
        }

        /// <summary>
        /// Write each element in seq, separated by commas, within brackets. 
        /// </summary>
        public void Write(IEnumerable<byte> seq) {
            WriteArrayStart(); foreach (var e in seq) Write(e); WriteArrayEnd();
        }

        /// <summary>
        /// Write each element in seq, separated by commas, within brackets. 
        /// </summary>
        public void Write(IEnumerable<sbyte> seq) {
            WriteArrayStart(); foreach (var e in seq) Write(e); WriteArrayEnd();
        }

        /// <summary>
        /// Write each element in seq, separated by commas, within brackets. 
        /// </summary>
        public void Write(IEnumerable<short> seq) {
            WriteArrayStart(); foreach (var e in seq) Write(e); WriteArrayEnd();
        }

        /// <summary>
        /// Write each element in seq, separated by commas, within brackets. 
        /// </summary>
        public void Write(IEnumerable<ushort> seq) {
            WriteArrayStart(); foreach (var e in seq) Write(e); WriteArrayEnd();
        }

        /// <summary>
        /// Write each element in seq, separated by commas, within brackets. 
        /// </summary>
        public void Write(IEnumerable<int> seq) {
            WriteArrayStart(); foreach (var e in seq) Write(e); WriteArrayEnd();
        }

        /// <summary>
        /// Write each element in seq, separated by commas, within brackets. 
        /// </summary>
        public void Write(IEnumerable<uint> seq) {
            WriteArrayStart(); foreach (var e in seq) Write(e); WriteArrayEnd();
        }

        /// <summary>
        /// Write each element in seq, separated by commas, within brackets. 
        /// </summary>
        public void Write(IEnumerable<long> seq) {
            WriteArrayStart(); foreach (var e in seq) Write(e); WriteArrayEnd();
        }

        /// <summary>
        /// Write each element in seq, separated by commas, within brackets. 
        /// </summary>
        public void Write(IEnumerable<ulong> seq) {
            WriteArrayStart(); foreach (var e in seq) Write(e); WriteArrayEnd();
        }

        /// <summary>
        /// Write each element in seq, separated by commas, within brackets. 
        /// </summary>
        public void Write(IEnumerable<float> seq) {
            WriteArrayStart(); foreach (var e in seq) Write(e); WriteArrayEnd();
        }

        /// <summary>
        /// Write each element in seq, separated by commas, within brackets. 
        /// </summary>
        public void Write(IEnumerable<double> seq) {
            WriteArrayStart(); foreach (var e in seq) Write(e); WriteArrayEnd();
        }

        /// <summary>
        /// Write each element in seq, separated by commas, within brackets. 
        /// </summary>
        public void Write(IEnumerable<bool?> seq) {
            WriteArrayStart(); foreach (var e in seq) Write(e); WriteArrayEnd();
        }

        /// <summary>
        /// Write each element in seq, separated by commas, within brackets. 
        /// </summary>
        public void Write(IEnumerable<byte?> seq) {
            WriteArrayStart(); foreach (var e in seq) Write(e); WriteArrayEnd();
        }

        /// <summary>
        /// Write each element in seq, separated by commas, within brackets. 
        /// </summary>
        public void Write(IEnumerable<sbyte?> seq) {
            WriteArrayStart(); foreach (var e in seq) Write(e); WriteArrayEnd();
        }

        /// <summary>
        /// Write each element in seq, separated by commas, within brackets. 
        /// </summary>
        public void Write(IEnumerable<short?> seq) {
            WriteArrayStart(); foreach (var e in seq) Write(e); WriteArrayEnd();
        }

        /// <summary>
        /// Write each element in seq, separated by commas, within brackets. 
        /// </summary>
        public void Write(IEnumerable<ushort?> seq) {
            WriteArrayStart(); foreach (var e in seq) Write(e); WriteArrayEnd();
        }

        /// <summary>
        /// Write each element in seq, separated by commas, within brackets. 
        /// </summary>
        public void Write(IEnumerable<int?> seq) {
            WriteArrayStart(); foreach (var e in seq) Write(e); WriteArrayEnd();
        }

        /// <summary>
        /// Write each element in seq, separated by commas, within brackets. 
        /// </summary>
        public void Write(IEnumerable<uint?> seq) {
            WriteArrayStart(); foreach (var e in seq) Write(e); WriteArrayEnd();
        }

        /// <summary>
        /// Write each element in seq, separated by commas, within brackets. 
        /// </summary>
        public void Write(IEnumerable<long?> seq) {
            WriteArrayStart(); foreach (var e in seq) Write(e); WriteArrayEnd();
        }

        /// <summary>
        /// Write each element in seq, separated by commas, within brackets. 
        /// </summary>
        public void Write(IEnumerable<ulong?> seq) {
            WriteArrayStart(); foreach (var e in seq) Write(e); WriteArrayEnd();
        }

        /// <summary>
        /// Write each element in seq, separated by commas, within brackets. 
        /// </summary>
        public void Write(IEnumerable<float?> seq) {
            WriteArrayStart(); foreach (var e in seq) Write(e); WriteArrayEnd();
        }

        /// <summary>
        /// Write each element in seq, separated by commas, within brackets. 
        /// </summary>
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