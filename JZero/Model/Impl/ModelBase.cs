using System;

namespace JZero.Model.Impl {
    public delegate void OnChanged(ModelBase model, bool hasValue);

    public abstract class ModelBase {
        private int? index;
        private string key;

        internal ModelBase Parent { get; private set; }

        public event OnChanged Changed;

        public void WritePath(ref JsonWriter writer) {
            writer.WriteArrayStart();
            WritePathElements(ref writer);
            writer.WriteArrayEnd();
        }

        public abstract void WriteValue(ref JsonWriter writer);

        protected void Connect(ModelBase model, int index) {
            if (model != null) {
                if (model.Parent != null)
                    throw new Exception("cannot replace parent");
                model.Parent = this;
                model.index = index;
            }
        }

        protected void Connect(ModelBase model, string key) {
            if (model != null) {
                if (model.Parent != null)
                    throw new Exception("cannot replace parent");
                model.Parent = this;
                model.key = key;
            }
        }

        protected void Disconnect(ModelBase p) {
            if (p != null) {
                p.Parent = null;
                p.index = null;
                p.key = null;
            }
        }

        protected void TriggerChangedEvents(ModelBase model, bool hasValue) {
            if (model != null) {
                var c = Changed;
                if (c != null) c(model, hasValue);

                var p = Parent;
                if (p != null) p.TriggerChangedEvents(model, hasValue);
            }
        }

        protected void Assigned(string name, object value, object prev) {
            var pmod = prev as ModelBase;
            var vmod = value as ModelBase;
            Connect(vmod, name);
            TriggerChangedEvents(vmod ?? pmod, vmod != null);
            Disconnect(pmod);
        }

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
            if (index != null || key != null) {
                Parent.WritePathElements(ref writer);
                if (index != null)
                    writer.Write(index.Value);
                else if (key != null)
                    writer.Write(key);
            }
        }
    }
}
