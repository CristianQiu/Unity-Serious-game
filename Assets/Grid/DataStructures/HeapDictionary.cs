using System.Collections.Generic;

/// <summary>
/// A pair of a binary heap and a dictionary to speed up the minimum extraction as well as checking if an element is contained in the collection
/// </summary>
/// <typeparam name="T"></typeparam>
public class HeapDictionary<T> where T : IBinaryHeapComparable<T>, IUniqueIdentifiable<T>
{
    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="initialCapacity"></param>
    public HeapDictionary(int initialCapacity)
    {
        heap = new BinaryHeap<T>(initialCapacity);
        dictionary = new Dictionary<int, T>(initialCapacity);
    }

    #endregion

    #region Private Attributes

    private BinaryHeap<T> heap = null;
    private Dictionary<int, T> dictionary = null;

    #endregion

    #region Properties

    public int Count { get { return dictionary.Count; } }

    #endregion

    #region Methods

    /// <summary>
    /// Add a node to both the heap and dictionary
    /// </summary>
    /// <param name="element"></param>
    public void Add(T element)
    {
        heap.Add(element);
        dictionary.Add(element.UniqueId, element);
    }

    /// <summary>
    /// Get the min from the heap and remove it from the dictionary according to its id
    /// </summary>
    /// <returns></returns>
    public T ExtractMin()
    {
        T min = heap.ExtractMin();
        dictionary.Remove(min.UniqueId);

        return min;
    }

    /// <summary>
    /// Update the position in the heap of an element that has had its values to be compared decreased and must be reordered
    /// </summary>
    /// <param name="element"></param>
    public void UpdateElementWithDecreasedValue(T element)
    {
        heap.UpdateElementWithDecreasedValue(element);
    }

    /// <summary>
    /// Check if a node is contained in the collection
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    public bool Contains(T element)
    {
        return dictionary.ContainsKey(element.UniqueId);
    }

    #endregion
}