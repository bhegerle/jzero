using System;

namespace JZero.Model.Impl {
    /// <summary>
    /// Delegate type of the event raised when a property of a ModelBase changes.
    /// </summary>
    public delegate void OnChanged(ModelBase model, bool hasValue);

    /// <summary>
    /// An observable, serializable class.
    /// </summary>
    public abstract class ModelBase {
        /// <summary>
        /// The parent model base.
        /// </summary>
        public ModelBase Parent { get; private set; }

        /// <summary>
        /// The index of this in the parent, when that is an IArray.
        /// </summary>
        public int? Index { get; private set; }

        /// <summary>
        /// The property name of this on the parent, when that is an IDict or arbitrary model.
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// Event raised when a property of this ModelBase, or one of its children, changes.
        /// </summary>
        public event OnChanged Changed;

        /// <summary>
        /// Writes an array of strings and integers corresponding to the properties
        /// which lead from the root to this object.
        /// </summary>
        public void WritePath(ref JsonWriter writer) {
            writer.WriteArrayStart();
            WritePathElements(ref writer);
            writer.WriteArrayEnd();
        }

        /// <summary>
        /// Write this object as JSON to the writer.
        /// </summary>
        public abstract void WriteValue(ref JsonWriter writer);

        /// <summary>
        /// Associate this object with its new parent at the array <c>index</c>.
        /// </summary>
        protected void Connect(ModelBase model, int index) {
            if (model != null) {
                if (model.Parent != null)
                    throw new Exception("cannot replace parent");
                model.Parent = this;
                model.Index = index;
            }
        }

        /// <summary>
        /// Associate this object with its new parent under the property name <c>key</c>.
        /// </summary>
        protected void Connect(ModelBase model, string key) {
            if (model != null) {
                if (model.Parent != null)
                    throw new Exception("cannot replace parent");
                model.Parent = this;
                model.Key = key;
            }
        }

        /// <summary>
        /// Disassociate this object from its parent.
        /// </summary>
        protected void Disconnect(ModelBase p) {
            if (p != null) {
                p.Parent = null;
                p.Index = null;
                p.Key = null;
            }
        }

        /// <summary>
        /// Raise the Changed event on this object, and recursively on its parents.
        /// </summary>
        protected void TriggerChangedEvents(ModelBase model, bool hasValue) {
            if (model != null) {
                var c = Changed;
                if (c != null) c(model, hasValue);

                var p = Parent;
                if (p != null) p.TriggerChangedEvents(model, hasValue);
            }
        }

        /// <summary>
        /// Assign a new object to property named <c>name</c>.
        /// </summary>
        protected void Assigned(string name, object value, object prev) {
            var pmod = prev as ModelBase;
            var vmod = value as ModelBase;
            Connect(vmod, name);
            TriggerChangedEvents(vmod ?? pmod, vmod != null);
            Disconnect(pmod);
        }

        /// <summary>
        /// Return this object as JSON, truncating the output if its gets too long.
        /// </summary>
        public override string ToString() {
            var buffer = new char[200];
            var w = new JsonWriter(buffer);
            try {
                WriteValue(ref w);
            } catch (JsonException) {
                var seg = w.Written;
                var n = seg.Count;
                if (n > 3)
                    seg[n - 1] = seg[n - 2] = seg[n - 3] = '.';
            }
            return w.WrittenString;
        }

        private void WritePathElements(ref JsonWriter writer) {
            if (Index != null || Key != null) {
                Parent.WritePathElements(ref writer);
                if (Index != null)
                    writer.Write(Index.Value);
                else if (Key != null)
                    writer.Write(Key);
            }
        }
    }
}
