using System;

namespace JZero {
    public class JsonException : Exception {
        private readonly ArraySegment<char> segment;
        private readonly int offset;

        public JsonException(ArraySegment<char> segment, int offset, string message)
            : this(segment, offset, true, message) {
        }

        public JsonException(ArraySegment<char> segment, int offset, bool offsetRelToSeg, string message)
            : base(message) {
            this.segment = segment;
            this.offset = offsetRelToSeg ? offset : offset - segment.Offset;
        }

        public ReadOnlySpan<char> ContextBeforeError {
            get {
                return segment.Slice(0, offset).AsSpan();
            }
        }

        public ReadOnlySpan<char> ContextAfterError {
            get {
                var n = segment.Count - offset;
                if (n > 8) n = 8;
                return segment.Slice(offset, n).AsSpan();
            }
        }
    }
}