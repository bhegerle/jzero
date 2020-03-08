using System;

namespace JZero.Model.Impl {
    internal class ScalarDictModel<T> : DictModel<ScalarModel<T>>, IDict<T> {
        T IDict<T>.this[string key] {
            get {
                var s = this[key];
                if (s != null)
                    return s.Value;
                else if (Nullable)
                    return (T)(object)null;
                else
                    throw new Exception("cannot return null dict value as primitive");
            }

            set {
                var s = this[key];
                if (s == null)
                    this[key] = Factory.NewScalar(value);
                else
                    s.Value = value;
            }
        }

        private static readonly bool Nullable;

        static ScalarDictModel() {
            try {
                Nullable = (T)(object)null == null;
            } catch {
                // ignored
            }
        }
    }
}
