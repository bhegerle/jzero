namespace JZero.Model {
    /// <summary>
    /// Methods that operate on arrays.
    /// </summary>
    public static class ExtensionMethods {
        /// <summary>
        /// Add one object to the end of an array.
        /// </summary>
        public static void Add<T>(this IArray<T> array, T element) {
            array[array.Count] = element;
        }
    }
}
