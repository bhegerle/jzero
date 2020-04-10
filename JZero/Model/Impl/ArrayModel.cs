using System;
using static System.Math;

namespace JZero.Model.Impl {
    internal class ArrayModel<T> : ModelBase, IArray<T> where T : class {
        private T[] array;

        public ArrayModel() {
            array = new T[4];
        }

        public int Count { get; private set; }

        public T this[int index] {
            get {
                return index < array.Length ? array[index] : null;
            }

            set {
                while (array.Length <= index)
                    Array.Resize(ref array, 2 * array.Length);

                var m = value as ModelBase;
                var p = array[index] as ModelBase;

                array[index] = value;
                Count = Max(index + 1, Count);

                Connect(m, index);
                TriggerChangedEvents(m ?? p, m != null);

                Disconnect(p);
            }
        }

        public override void WriteValue(ref JsonWriter writer) {
            writer.WriteArrayStart();
            for (var i = 0; i < Count; i++)
                if (this[i] is ModelBase m)
                    m.WriteValue(ref writer);
                else
                    writer.WriteNull();
            writer.WriteArrayEnd();
        }
    }
}
