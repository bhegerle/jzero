using System;
using static System.Math;

namespace JZero.Model.Impl {
    internal class ScalarArrayModel<T> : ArrayModel<ScalarModel<T>>, IArray<T> {
        private int n;

        int IArray<T>.Count => n;

        T IArray<T>.this[int index] {
            get {
                var s = this[index];
                if (s != null)
                    return s.Value;
                else if (Nullable)
                    return (T)(object)null;
                else
                    throw new Exception("cannot return null array element as primitive");
            }

            set {
                var s = this[index];
                n = Max(index + 1, n);

                if (s == null)
                    this[index] = Factory.NewScalar(value);
                else
                    s.Value = value;
            }
        }

        private static readonly bool Nullable;

        static ScalarArrayModel() {
            try {
                Nullable = (T)(object)null == null;
            } catch {
                // ignored
            }
        }
    }
}
