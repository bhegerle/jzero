using JZero.Model.Impl;

namespace JZero.Model {
    /// <summary>
    /// Easy way to construct implementations of interfaces that derive from ModelBase.
    /// </summary>
    public static class New {
        /// <summary>
        /// Construct instance of a type implementing <c>I</c> and deriving from ModelBase.
        /// Valid interface contains: scalars, other such interfaces, IArray or IDict properties.
        /// </summary>
        public static I Model<I>() { return Factory.NewModel<I>(); }

        /// <summary>
        /// Construct new instance of a type implementing IArray and deriving from ModelBase.
        /// </summary>
        public static IArray<T> Array<T>() { return Model<IArray<T>>(); }

        /// <summary>
        /// Construct new instance of a type implementing IDict and deriving from ModelBase.
        /// </summary>
        public static IDict<T> Dict<T>() { return Model<IDict<T>>(); }
    }
}