using System;
using System.Collections;
using System.Collections.Generic;

namespace JZero {
    /// <summary>
    /// JSON token-type enumeration.
    /// </summary>
    public enum JsonToken {
        /// <summary>
        /// Default value.
        /// </summary>
        Invalid,

        /// <summary>
        /// The left-brace character.
        /// </summary>
        ObjectStart,

        /// <summary>
        /// The right-brace character.
        /// </summary>
        ObjectEnd,

        /// <summary>
        /// The left-bracket character.
        /// </summary>
        ArrayStart,

        /// <summary>
        /// The right-bracket character.
        /// </summary>
        ArrayEnd,

        /// <summary>
        /// The colon character.
        /// </summary>
        Colon,

        /// <summary>
        /// The comma delimiter.
        /// </summary>
        Comma,

        /// <summary>
        /// A string value--not a property name.
        /// </summary>
        String,

        /// <summary>
        /// A numeric value, not tied to a .Net number type.
        /// </summary>
        Number,

        /// <summary>
        /// A true/false value.
        /// </summary>
        Bool,

        /// <summary>
        /// A null value.
        /// </summary>
        Null,

        /// <summary>
        /// End-of-buffer sentinel.
        /// </summary>
        Eof,
    }

    public struct JsonTokenizer : IEnumerable<JsonToken> {
        private readonly ArraySegment<char> buffer;

        public JsonTokenizer(ArraySegment<char> buffer) {
            this.buffer = buffer;
        }

        public JsonTokenizer(string s) : this(s.ToCharArray()) { }

        public JsonTokenEnumerator GetEnumerator() {
            return new JsonTokenEnumerator(buffer);
        }

        IEnumerator<JsonToken> IEnumerable<JsonToken>.GetEnumerator() {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }

    public struct JsonTokenEnumerator : IEnumerator<JsonToken> {
        private readonly ArraySegment<char> segment;
        private readonly char[] buffer;
        private readonly int last;
        private int begin, end;

        public JsonTokenEnumerator(ArraySegment<char> segment) {
            this.segment = segment;
            buffer = segment.Array;
            last = segment.Offset + segment.Count;
            begin = end = segment.Offset;
            Current = JsonToken.Invalid;
            CurrentSegment = null;
        }

        public ArraySegment<char> CurrentSegment { get; private set; }
        public Span<char> CurrentSpan => CurrentSegment.AsSpan();

        public JsonToken Current { get; private set; }
        object IEnumerator.Current => Current;

        public void Dispose() { }

        public bool MoveNext() {
            if (Current == JsonToken.Eof)
                return false;

            begin = end;

            while (begin < last && char.IsWhiteSpace(buffer[begin]))
                begin++;

            if (begin == last) {
                end = begin;
                Current = JsonToken.Eof;
            } else {
                switch (buffer[begin]) {
                    case '{':
                        Current = JsonToken.ObjectStart;
                        end = begin + 1;
                        break;
                    case '}':
                        Current = JsonToken.ObjectEnd;
                        end = begin + 1;
                        break;
                    case '[':
                        Current = JsonToken.ArrayStart;
                        end = begin + 1;
                        break;
                    case ']':
                        Current = JsonToken.ArrayEnd;
                        end = begin + 1;
                        break;
                    case ':':
                        Current = JsonToken.Colon;
                        end = begin + 1;
                        break;
                    case ',':
                        Current = JsonToken.Comma;
                        end = begin + 1;
                        break;
                    case '"':
                        var (qchars, uchars) = Quoting.Unquote(Slice());
                        Current = JsonToken.String;
                        end = begin + qchars;
                        CurrentSegment = Slice(uchars);
                        return true;
                    case 't':
                        if (MatchTrue()) {
                            Current = JsonToken.Bool;
                            end = begin + 4;
                        } else {
                            goto default;
                        }
                        break;
                    case 'f':
                        if (MatchFalse()) {
                            Current = JsonToken.Bool;
                            end = begin + 5;
                        } else {
                            goto default;
                        }
                        break;
                    case 'n':
                        if (MatchNull()) {
                            Current = JsonToken.Null;
                            end = begin + 4;
                        } else {
                            goto default;
                        }
                        break;
                    case '-':
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        var nchars = MatchNumber();
                        if (nchars > 0) {
                            Current = JsonToken.Number;
                            end = begin + nchars;
                        } else {
                            goto default;
                        }
                        break;
                    default:
                        throw new JsonException(segment, begin, false, "unexpected character");
                }
            }

            CurrentSegment = Slice(end - begin);

            return true;
        }

        public void Reset() {
            begin = end = segment.Offset;
            Current = JsonToken.Invalid;
            CurrentSegment = null;
        }

        public bool NextIsNull() {
            for (var i = end; i < last; i++)
                if (!char.IsWhiteSpace(buffer[i]))
                    return buffer[i] == 'n';
            return false;
        }

        public bool NextIsObjectEnd() {
            for (var i = end; i < last; i++)
                if (!char.IsWhiteSpace(buffer[i]))
                    return buffer[i] == '}';
            return false;
        }

        public bool NextIsArrayEnd() {
            for (var i = end; i < last; i++)
                if (!char.IsWhiteSpace(buffer[i]))
                    return buffer[i] == ']';
            return false;
        }

        private readonly bool MatchNull() {
            return begin + 3 < last &&
                buffer[begin + 1] == 'u' && buffer[begin + 2] == 'l' &&
                buffer[begin + 3] == 'l';
        }

        private readonly bool MatchTrue() {
            return begin + 3 < last &&
                buffer[begin + 1] == 'r' && buffer[begin + 2] == 'u' &&
                buffer[begin + 3] == 'e';
        }

        private readonly bool MatchFalse() {
            return begin + 4 < last &&
                buffer[begin + 1] == 'a' && buffer[begin + 2] == 'l' &&
                buffer[begin + 3] == 's' && buffer[begin + 4] == 'e';
        }

        private readonly int MatchNumber() {
            var i = begin;

            if (buffer[i] == '-')
                if (++i >= last)
                    return 0;

            if (buffer[i] == '0')
                i++;
            else if ('1' <= buffer[i] && buffer[i] <= '9')
                do
                    i++;
                while (i < last && '0' <= buffer[i] && buffer[i] <= '9');
            else
                return 0;

            if (i < last && buffer[i] == '.') {
                if (++i >= last)
                    return 0;

                while (i < last && '0' <= buffer[i] && buffer[i] <= '9')
                    i++;
            }

            if (i < last) {
                if ((buffer[i] == 'e' || buffer[i] == 'E')) {
                    if (++i >= last)
                        return 0;

                    if (buffer[i] == '+' || buffer[i] == '-')
                        if (++i >= last)
                            return 0;

                    while (i < last && '0' <= buffer[i] && buffer[i] <= '9')
                        i++;
                }
            }

            return i - begin;
        }

        private ArraySegment<char> Slice() {
            return new ArraySegment<char>(buffer, begin, last - begin);
        }

        private ArraySegment<char> Slice(int count) {
            return new ArraySegment<char>(buffer, begin, count);
        }
    }
}