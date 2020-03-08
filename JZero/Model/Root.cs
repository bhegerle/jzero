using JZero.Model.Impl;

namespace JZero.Model {
    public class Root<T> : ModelBase {
        private readonly ModelBase ModelBase;

        public Root() {
            Model = Factory.NewModel<T>();
            ModelBase = (ModelBase)(object)Model;
            Connect(ModelBase, null);
        }

        public T Model { get; }

        public override void WriteValue(ref JsonWriter writer) {
            ModelBase.WriteValue(ref writer);
        }
    }
}
