using JZero.Model.Impl;

namespace JZero.Model {
    public static class New {
        public static T Model<T>() { return Factory.NewModel<T>(); }
        public static IArray<T> Array<T>() { return Model<IArray<T>>(); }
        public static IDict<T> Dict<T>() { return Model<IDict<T>>(); }
    }
}