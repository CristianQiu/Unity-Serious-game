/// <summary>
/// Interface to compare two objects so that we can require a class to implement the comparison method to be used
/// in the heap, and keep it as a priority queue where all objects are ordered from the root to its leafs.
/// We also need to require the index where the element is placed in the heap so that we can know from which
/// index we should shift down or up the element once it's updated, since it must be reordered
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IBinaryHeapComparable<T>
{
    int HeapIndex { get; set; }

    bool LowerThan(T otherObj);
}