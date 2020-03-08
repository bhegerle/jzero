namespace JZero.Model {
    public interface IDict<T> {
        T this[string key] { get; set; }
    }
}