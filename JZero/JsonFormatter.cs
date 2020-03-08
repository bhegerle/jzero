using System;

namespace JZero {
    internal struct JsonFormatter {
        private readonly ArraySegment<char> segment;
        private readonly char[] buffer;
        private readonly int last;
        private int offset;

        internal JsonFormatter(ArraySegment<char> segment) {
            this.segment = segment;
            this.buffer = segment.Array;
            offset = segment.Offset;
            last = offset + segment.Count;
        }

        internal ArraySegment<char> Written => segment.Slice(0, offset - segment.Offset);
        internal ArraySegment<char> Remaining => segment.Slice(offset - segment.Offset);

        internal void WriteObjectStart() { WriteChar('{', "no space for object start"); }
        internal void WriteArrayStart() { WriteChar('[', "no space for array start"); }
        internal void WriteColon() { WriteChar(':', "no space for colon"); }

        internal void WriteObjectEnd() { WriteChar('}', "no space for object end"); }
        internal void WriteArrayEnd() { WriteChar(']', "no space for array end"); }
        internal void WriteComma() { WriteChar(',', "no space for comma"); }

        internal void WriteNull() { WriteKeyword("null", "no space for null"); }

        internal void Write(ReadOnlySpan<char> s) {
            offset += Quoting.Quote(s, Remaining);
        }

        internal void Write(bool b) {
            WriteKeyword(b ? "true" : "false", "no space for boolean");
        }

        internal void Write(byte i) {
            if (i.TryFormat(Remaining, out var n)) offset += n;
            else throw JsonEx();
        }

        internal void Write(sbyte i) {
            if (i.TryFormat(Remaining, out var n)) offset += n;
            else throw JsonEx();
        }

        internal void Write(short i) {
            if (i.TryFormat(Remaining, out var n)) offset += n;
            else throw JsonEx();
        }

        internal void Write(ushort i) {
            if (i.TryFormat(Remaining, out var n)) offset += n;
            else throw JsonEx();
        }

        internal void Write(int i) {
            if (i.TryFormat(Remaining, out var n)) offset += n;
            else throw JsonEx();
        }

        internal void Write(uint i) {
            if (i.TryFormat(Remaining, out var n)) offset += n;
            else throw JsonEx();
        }

        internal void Write(long i) {
            if (i.TryFormat(Remaining, out var n)) offset += n;
            else throw JsonEx();
        }

        internal void Write(ulong i) {
            if (i.TryFormat(Remaining, out var n)) offset += n;
            else throw JsonEx();
        }

        internal void Write(float i) {
            if (i.TryFormat(Remaining, out var n)) offset += n;
            else throw JsonEx();
        }

        internal void Write(double i) {
            if (i.TryFormat(Remaining, out var n)) offset += n;
            else throw JsonEx();
        }

        private void WriteChar(char c, string msg) {
            if (offset >= last)
                throw new JsonException(segment, offset, false, msg);
            buffer[offset++] = c;
        }

        private void WriteKeyword(string s, string msg) {
            if (offset + s.Length > last)
                throw new JsonException(segment, offset, false, msg);
            s.CopyTo(0, buffer, offset, s.Length);
            offset += s.Length;
        }

        private readonly JsonException JsonEx() {
            return new JsonException(segment, offset, false, "failed to write int");
        }
    }
}