namespace JZero.Model {
    /// <summary>
    /// A simple array-like container.
    /// </summary>
    /// <see cref="JZero.Model.New.Array{T}" />
    public interface IArray<T> {
        /// <summary>
        /// Access element by <c>index</c>.
        /// </summary>
        T this[int index] { get; set; }
    }
}
