using System;

namespace JZero {
    internal class JsonException : Exception {
        private readonly ArraySegment<char> segment;
        private readonly int offset;

        internal JsonException(ArraySegment<char> segment, int offset, string message)
            : this(segment, offset, true, message) {
        }

        internal JsonException(ArraySegment<char> segment, int offset, bool offsetRelToSeg, string message)
            : base(message) {
            this.segment = segment;
            this.offset = offsetRelToSeg ? offset : offset - segment.Offset;
        }

        internal ReadOnlySpan<char> ContextBeforeError {
            get {
                return segment.Slice(0, offset).AsSpan();
            }
        }

        internal ReadOnlySpan<char> ContextAfterError {
            get {
                var n = segment.Count - offset;
                if (n > 8) n = 8;
                return segment.Slice(offset, n).AsSpan();
            }
        }
    }
}