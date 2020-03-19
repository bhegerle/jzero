using System;

namespace JZero {
    /// <summary>
    /// Class for JSON parse errors, or buffer errors on formatting.
    /// </summary>
    public class JsonException : Exception {
        private readonly ArraySegment<char> segment;
        private readonly int offset;

        /// <summary>
        /// Represent parse error at the specific location.
        /// </summary>
        public JsonException(ArraySegment<char> segment, int offset, string message)
            : this(segment, offset, true, message) {
        }

        /// <summary>
        /// Represent parse error at the specific location.
        /// </summary>
        public JsonException(ArraySegment<char> segment, int offset, bool offsetRelToSeg, string message)
            : base(message) {
            this.segment = segment;
            this.offset = offsetRelToSeg ? offset : offset - segment.Offset;
        }

        /// <summary>
        /// Slice the successfully-parsed input leading up to the parse error.
        /// </summary>
        public ReadOnlySpan<char> ContextBeforeError {
            get {
                return segment.Slice(0, offset).AsSpan();
            }
        }

        /// <summary>
        /// Slice the unparsed input following the parse error.
        /// </summary>
        public ReadOnlySpan<char> ContextAfterError {
            get {
                var n = segment.Count - offset;
                if (n > 8) n = 8;
                return segment.Slice(offset, n).AsSpan();
            }
        }
    }
}