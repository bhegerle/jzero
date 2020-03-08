using System;
using System.Collections.Generic;

namespace JZero.Model.Impl {
    internal class DictModel<T> : ModelBase, IDict<T> where T : class {
        private Dictionary<Entry, T> set;

        public DictModel() {
            set = new Dictionary<Entry, T>();
        }

        public T this[string key] {
            get { return this[key, null]; }
            set { this[key, null] = value; }
        }

        public T this[ArraySegment<char> key] {
            get { return this[null, key]; }
            set { this[null, key] = value; }
        }

        public override void WriteValue(ref JsonWriter writer) {
            writer.WriteObjectStart();
            foreach (var e in set) {
                writer.WritePropertyName(e.Key.Str);
                if (e.Value is ModelBase m)
                    m.WriteValue(ref writer);
                else
                    writer.WriteNull();
            }
            writer.WriteObjectEnd();
        }

        struct Entry {
            internal Entry(string str, ArraySegment<char> seg) {
                Str = str;
                Seg = seg;
            }

            internal string Str;
            internal ArraySegment<char> Seg;

            internal ReadOnlySpan<char> Span => Str != null ? Str.AsSpan() : Seg.AsSpan();

            internal void Freeze() {
                if (Str == null)
                    Str = new string(Seg.AsSpan());
                Seg = null;
            }

            public override bool Equals(object obj) {
                if (obj.GetType() == typeof(Entry)) {
                    var e = (Entry)obj;
                    return Equals(Span, e.Span);
                }
                return false;
            }

            public override int GetHashCode() {
                int hash = 5381;
                foreach (var c in Span)
                    hash = (hash << 5) + hash + c;
                return hash;
            }

            public override string ToString() {
                if (Str == null)
                    Str = new string(Seg.AsSpan());
                return Str;
            }

            private static bool Equals(ReadOnlySpan<char> a, ReadOnlySpan<char> b) {
                if (a.Length != b.Length)
                    return false;

                for (var i = 0; i < a.Length; i++)
                    if (a[i] != b[i])
                        return false;

                return true;
            }
        }

        private T this[string str, ArraySegment<char> seg] {
            get {
                return set.TryGetValue(new Entry(str, seg), out var val) ? val : null;
            }

            set {
                var e = new Entry(str, seg);
                e.Freeze();

                var m = value as ModelBase;

                set.TryGetValue(e, out var prev);
                var p = prev as ModelBase;

                set[e] = value;
                Connect(m, e.Str);
                TriggerChangedEvents(m ?? p, m != null);
                Disconnect(p);
            }
        }
    }
}