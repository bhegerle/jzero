namespace JZero.Model {
    /// <summary>
    /// A simple dictionary-like container.
    /// </summary>
    /// <see cref="JZero.Model.New.Dict{T}" />
    public interface IDict<T> {
        /// <summary>
        /// Access element by <c>key</c>.
        /// </summary>
        T this[string key] { get; set; }
    }
}