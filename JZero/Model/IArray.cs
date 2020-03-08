namespace JZero.Model {
    public interface IArray<T> {
        T this[int key] { get; set; }
    }
}
