/// <summary>
/// A binary heap used to implement a priority queue where objects are ordered depending on certain values, perhaps the most 
/// common case is to store objects so that lower values are at the top of the heap, with the minimum being at the root level
/// </summary>
/// <typeparam name="T"></typeparam>
public class BinaryHeap<T> where T : IBinaryHeapComparable<T>
{
    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="initialCapacity"></param>
    public BinaryHeap(int initialCapacity)
    {
        count = 0;
        capacity = initialCapacity;
        elements = new T[initialCapacity]; 
    }

    #endregion

    #region Private Attributes

    private const int InvalidIndex = -1;

    private int count = 0;
    private int capacity = 0;

    private T[] elements = null;
    private T[] nullElement = new T[1];

    #endregion

    #region Properties

    public int Count { get { return count; } }

    #endregion

    #region Heap Methods

    /// <summary>
    /// Adds an element to the heap and orders it so lower values are at the top of the heap, with the minimum being at the root level
    /// </summary>
    /// <param name="element"></param>
    public void Add(T element)
    {
        if (count == capacity)
            DoubleElementsCapacity();

        elements[count] = element;
        element.HeapIndex = count;
        ShiftUp(count);

        count++;
    }

    /// <summary>
    /// Extract the minimum value element and reorder the heap
    /// </summary>
    /// <returns></returns>
    public T ExtractMin()
    {
        if (count <= 0)
            return nullElement[0];

        // get the min
        T min = elements[0];

        // delete the root by placing the last element there
        count--;
        SwapElements(0, count);

        // this may not really be needed but just for clarity, nullify the spot from where we moved the last element
        elements[count] = nullElement[0];

        // shift down from the start
        ShiftDown(0);

        return min;
    }

    /// <summary>
    /// Update the position in the heap of an element that has had its values to be compared decreased and must be reordered
    /// </summary>
    /// <param name="element"></param>
    public void UpdateElementWithDecreasedValue(T element)
    {
        ShiftUp(element.HeapIndex);
    }

    #endregion

    #region Useful Methods

    /// <summary>
    /// Double the capacity of the elements in case we run out of space, just like List<T> does
    /// </summary>
    private void DoubleElementsCapacity()
    {
        capacity *= 2;
        T[] newElements = new T[capacity];

        elements.CopyTo(newElements, 0);

        elements = newElements;
    }

    /// <summary>
    /// Move an element down while higher than its children
    /// </summary>
    /// <param name="fromIndex"></param>
    private void ShiftDown(int fromIndex)
    {
        // get children
        int left = GetLeftChildIndex(fromIndex);
        int right = GetRightChildIndex(fromIndex);

        int smallest = fromIndex;

        // calculate which child is smaller
        if (left < count && elements[left].LowerThan(elements[fromIndex]))
            smallest = left;

        if (right < count && elements[right].LowerThan(elements[smallest]))
            smallest = right;

        // swap with it if any of the children are smaller
        if (smallest != fromIndex)
        {
            SwapElements(fromIndex, smallest);
            ShiftDown(smallest);
        }
    }

    /// <summary>
    /// Move an element up while lower than its parent
    /// </summary>
    /// <param name="fromIndex"></param>
    private void ShiftUp(int fromIndex)
    {
        int parentIndex = GetParentIndex(fromIndex);

        // while there's a parent and this is lower than it
        while (parentIndex != InvalidIndex && elements[fromIndex].LowerThan(elements[parentIndex]))
        {
            // swap both elements
            SwapElements(fromIndex, parentIndex);
            fromIndex = parentIndex;
            parentIndex = GetParentIndex(fromIndex);
        }
    }

    /// <summary>
    /// Swap an element in the heap by another one
    /// </summary>
    /// <param name="firstIndex"></param>
    /// <param name="secondIndex"></param>
    private void SwapElements(int firstIndex, int secondIndex)
    {
        // save temporarily the element to overwrite
        T temp = elements[firstIndex];

        // replace it
        elements[firstIndex] = elements[secondIndex];
        elements[firstIndex].HeapIndex = firstIndex;

        // restore the temp element to the place we moved the other element from
        elements[secondIndex] = temp;
        elements[secondIndex].HeapIndex = secondIndex;
    }

    #endregion

    #region Parent & Children Methods

    /// <summary>
    /// Get the parent index of an index
    /// </summary>
    /// <param name="ofIndex"></param>
    /// <returns></returns>
    private int GetParentIndex(int ofIndex)
    {
        int i = (ofIndex - 1) / 2;

        if (ofIndex == 0)
            i = InvalidIndex;

        return i;
    }

    /// <summary>
    /// Get the left child index of an index
    /// </summary>
    /// <param name="ofIndex"></param>
    /// <returns></returns>
    private int GetLeftChildIndex(int ofIndex)
    {
        return (ofIndex * 2) + 1;
    }

    /// <summary>
    /// Get the right child index of an index
    /// </summary>
    /// <param name="ofIndex"></param>
    /// <returns></returns>
    private int GetRightChildIndex(int ofIndex)
    {
        return (ofIndex * 2) + 2;
    }

    #endregion
}