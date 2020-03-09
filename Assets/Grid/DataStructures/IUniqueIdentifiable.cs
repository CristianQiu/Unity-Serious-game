/// <summary>
/// Interface to implement an id which should be unique for each instance of an object
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IUniqueIdentifiable<T>
{
    int UniqueId { get; }
}