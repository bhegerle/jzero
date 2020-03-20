using System;
using System.Collections.Generic;

namespace JZero {
    /// <summary>
    /// A readonly dictionary data structure which accepts readonly spans as keys.
    /// </summary>
    public class SymbolTable<T> where T : struct {
        private readonly string[] keyTable;
        private readonly T[] valTable;
        private readonly int maxHash, hashMult;
        private readonly Dictionary<string, T> symMap;

        /// <summary>
        /// Construct a symbol table by adding the strings in the array.
        /// </summary>
        public SymbolTable((string, T)[] symbols) : this(ToDictionary(symbols)) { }

        /// <summary>
        /// Construct a symbol table from a dictionary.
        /// </summary>
        public SymbolTable(Dictionary<string, T> symMap) {
            this.symMap = new Dictionary<string, T>(symMap);

            keyTable = new string[2 * symMap.Count];
            valTable = new T[2 * symMap.Count];

            for (maxHash = 1; maxHash < 6; maxHash++) {
                for (hashMult = 2; hashMult < 255; hashMult++) {
                    Array.Clear(keyTable, 0, keyTable.Length);

                    var collision = false;
                    foreach (var sym in symMap) {
                        var h = Hash(sym.Key.AsSpan());
                        if (keyTable[h] == null) {
                            keyTable[h] = sym.Key;
                            valTable[h] = sym.Value;
                        } else {
                            collision = true;
                            break;
                        }
                    }

                    if (!collision)
                        return;
                }
            }

            keyTable = null;
            valTable = null;
        }

        /// <summary>
        /// Returns true if, during construction, a perfect hash could be found.
        /// </summary>
        public bool Fallback => keyTable == null;

        /// <summary>
        /// Returns the value associated with the symbol, or null.
        /// </summary>
        public T? this[string sym] => this[sym.AsSpan()];

        /// <summary>
        /// Returns the value associated with the symbol, or null.
        /// </summary>
        public T? this[ReadOnlySpan<char> sym] {
            get {
                if (keyTable != null) {
                    var h = Hash(sym);
                    var k = keyTable[h];
                    if (k != null && StrEq(k, sym))
                        return valTable[h];
                } else if (symMap.TryGetValue(sym.ToString(), out var val)) {
                    return val;
                }
                return null;
            }
        }

        /// <summary>
        /// Construct a symbol table mapping strings back to their enum values.
        /// </summary>
        public static SymbolTable<T> FromEnumValues() {
            var symMap = new Dictionary<string, T>();
            foreach (var v in Enum.GetValues(typeof(T)))
                symMap.Add(v.ToString(), (T)v);
            return new SymbolTable<T>(symMap);
        }

        private int Hash(ReadOnlySpan<char> sym) {
            var h = 5381;
            for (var i = 0; i < maxHash && i < sym.Length; i++)
                h = hashMult * h + sym[i];
            return (h < 0 ? -h : h) % keyTable.Length;
        }

        private static bool StrEq(string s0, ReadOnlySpan<char> s1) {
            if (s0.Length != s1.Length)
                return false;
            for (var i = 0; i < s0.Length; i++)
                if (s0[i] != s1[i])
                    return false;
            return true;
        }

        private static Dictionary<string, T> ToDictionary((string, T)[] symbols) {
            var d = new Dictionary<string, T>();
            foreach (var (key, value) in symbols)
                d.Add(key, value);
            return d;
        }
    }
}