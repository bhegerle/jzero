using JZero.Model.Impl;

namespace JZero.Model {
    /// <summary>
    /// Container for instance of a type implementing <c>T</c> and deriving from ModelBase. 
    /// </summary>
    public class Root<T> : ModelBase {
        private readonly ModelBase ModelBase;

        /// <summary>
        /// Construct new root object, and assign Model property.
        /// </summary>
        public Root() {
            Model = Factory.NewModel<T>();
            ModelBase = (ModelBase)(object)Model;
            Connect(ModelBase, null);
        }

        /// <summary>
        /// Instance of a type implementing <c>T</c> and deriving from ModelBase.  
        /// </summary>
        public T Model { get; }

        /// <summary>
        /// Write <c>Model</c> as JSON.
        /// </summary>
        public override void WriteValue(ref JsonWriter writer) {
            ModelBase.WriteValue(ref writer);
        }
    }
}
