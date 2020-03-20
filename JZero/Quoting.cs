using System;

namespace JZero {
    /// <summary>
    /// Helper class to quote and unquote strings.
    /// </summary>
    public static class Quoting {
        static void Quote() { }

        /// <summary>
        /// Return a new, quoted representation of a string.
        /// </summary>
        public static string Quote(string s) {
            var c = new char[2 * s.Length + 2];
            var qchars = Quote(s, c);
            return new string(c, 0, qchars);
        }

        /// <summary>
        /// Write a quoted representation of the read-only span into segment, 
        /// returning the number of characters written.
        /// </summary>
        public static int Quote(ReadOnlySpan<char> s, ArraySegment<char> seg) {
            var i = 0;

            if (i >= seg.Count)
                throw new JsonException(seg, 0, "no space for initial quote character");
            seg[i++] = '"';

            foreach (var c in s) {
                if (i >= seg.Count)
                    throw new JsonException(seg, 0, "no space for character");

                if (c == '\\' || c == '"') {
                    seg[i++] = '\\';
                    if (i >= seg.Count)
                        throw new JsonException(seg, 0, "no space for character escape");
                }

                seg[i++] = c;
            }

            if (i >= seg.Count)
                throw new JsonException(seg, 0, "no space for final quote character");
            seg[i++] = '"';

            return i;
        }

        /// <summary>
        /// Return an unquoted string.
        /// </summary>
        public static string Unquote(string s) {
            var c = s.ToCharArray();
            var (qchars, uchars) = Unquote(c);
            return new string(c, 0, uchars);
        }

        internal static (int, int) Unquote(ArraySegment<char> seg) {
            int i = 1, j = 0;
            while (i < seg.Count) {
                var c = seg[i];

                if (char.IsControl(c))
                    throw new JsonException(seg, i, "illegal control character");

                if (c == '"') {
                    i++;
                    for (var k = j; k < i; k++)
                        seg[k] = ' ';
                    return (i, j);
                } else if (c == '\\') {
                    i++;
                    if (i >= seg.Count)
                        throw new JsonException(seg, i, "incomplete character escape");

                    char u;
                    switch (seg[i]) {
                        case '"': u = '"'; break;
                        case '\\': u = '\\'; break;
                        case '/': u = '/'; break;
                        case 'b': u = '\b'; break;
                        case 'f': u = '\f'; break;
                        case 'n': u = '\n'; break;
                        case 'r': u = '\r'; break;
                        case 't': u = '\t'; break;
                        case 'u':
                            if (i + 4 >= seg.Count)
                                throw new JsonException(seg, i, "incomplete hex escape");

                            ushort codeUnit = 0;
                            for (var k = 0; k < 4; k++) {
                                codeUnit <<= 4;
                                codeUnit |= HexDigit(seg, i + k + 1);
                            }

                            u = (char)codeUnit;
                            i += 4;

                            break;
                        default:
                            throw new JsonException(seg, i, "unknown character escape");
                    }

                    seg[j++] = u;
                    i++;
                } else {
                    seg[j++] = seg[i++];
                }
            }

            throw new JsonException(seg, 0, "runaway string");
        }

        static ushort HexDigit(ArraySegment<char> buffer, int offset) {
            var c = buffer[offset];
            if ('0' <= c && c <= '9')
                return (ushort)(c - '0');
            if ('a' <= c && c <= 'F')
                return (ushort)(c - 'a' + 10);
            if ('A' <= c && c <= 'F')
                return (ushort)(c - 'A' + 10);
            throw new JsonException(buffer, offset, "invalid hex digit");
        }
    }
}