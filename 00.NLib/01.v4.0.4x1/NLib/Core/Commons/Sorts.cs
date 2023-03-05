#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2010-01-23
=================
- All sort related classes ported from GFA Library GFA37 tor GFA38v3

======================================================================================================================
Update 2008-11-30
=================
- All sort related classes move from GFA.Lib to GFA.Lib.Core.
- Multi Property Comparer class change access to public.
- Enhance Multi Property Comparer class change access to public.

======================================================================================================================
Update 2008-06-12
=================
- Add New Comparer
  - Add Enhance MultiProperty Comparer class (used internal).

======================================================================================================================
Update 2008-01-03
=================
- Collection - Sort algorithm classes added.
  - BubbleSort class added.
  - BiDirectionalBubbleSort class added.
  - InPlaceMergeSort class added.
  - DoubleStorageMergeSort class added.
  - QuickSort class added.
  - ArrayQuickSort class added.
  - FastQuickSort class added.
  - HybridBubbleQuickSort class added.
  - OddEvenTransportSort class added.
  - ShakerSort class added.
  - ShearSort class added.
  - ShellSort class added.
  - SortedManager class added. The helper class for sort IList.

======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;
using System.ComponentModel;
using System.Collections;
using System.Reflection;

#endregion

namespace NLib.Collections
{
    #region Interface

    #region ISort

    /// <summary>
    /// Summary description for ISort.
    /// </summary>
    public interface ISort
    {
        /// <summary>
        /// Sort IList
        /// </summary>
        /// <param name="list">IList instance that need to sort</param>
        void Sort(IList list);
    }

    #endregion

    #region ISwap

    /// <summary>
    /// Object swapper interface
    /// </summary>
    public interface ISwap
    {
        /// <summary>
        /// Swap IList
        /// </summary>
        /// <param name="array">IList Instance to swap</param>
        /// <param name="left">index of left object</param>
        /// <param name="right">index of right object</param>
        void Swap(IList array, int left, int right);
        /// <summary>
        /// Assign
        /// </summary>
        /// <param name="array">IList Instance to assign</param>
        /// <param name="left">index of left object</param>
        /// <param name="right">index of right object</param>
        void Assign(IList array, int left, int right);
        /// <summary>
        /// Assign
        /// </summary>
        /// <param name="array">IList Instance to assign</param>
        /// <param name="left">index of left object</param>
        /// <param name="obj">object to assign</param>
        void Assign(IList array, int left, object obj);
    }

    #endregion

    #endregion

    #region Common Class

    #region Swaper

    /// <summary>
    /// Default swap class
    /// </summary>
    public class DefaultSwap : ISwap
    {
        #region ISwap Implement

        /// <summary>
        /// Swap IList
        /// </summary>
        /// <param name="array">IList Instance to swap</param>
        /// <param name="left">index of left object</param>
        /// <param name="right">index of right object</param>
        public void Swap(IList array, int left, int right)
        {
            object swap = array[left];
            array[left] = array[right];
            array[right] = swap;
        }

        /// <summary>
        /// Assign
        /// </summary>
        /// <param name="array">IList Instance to assign</param>
        /// <param name="left">index of left object</param>
        /// <param name="right">index of right object</param>
        public void Assign(IList array, int left, int right)
        {
            array[left] = array[right];
        }

        /// <summary>
        /// Assign
        /// </summary>
        /// <param name="array">IList Instance to assign</param>
        /// <param name="left">index of left object</param>
        /// <param name="obj">object to assign</param>
        public void Assign(IList array, int left, object obj)
        {
            array[left] = obj;
        }

        #endregion
    }

    #endregion

    #endregion

    #region Base Class

    #region Abstract Sorter

    /// <summary>
    /// Abstract base class for Swap sort algorithms.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class serves as a base class for swap based sort algorithms.
    /// </para>
    /// </remarks>
    public abstract class AbstractSorter : ISort
    {
        #region Internal Variable

        private IComparer comparer;
        private ISwap swapper;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public AbstractSorter()
        {
            this.comparer = new ComparableComparer();
            this.swapper = new DefaultSwap();
        }

        /// <summary>
        /// Constructor with comparer and swaper
        /// </summary>
        /// <param name="comparer">Comparer instance</param>
        /// <param name="swapper">Swaper instance</param>
        public AbstractSorter(IComparer comparer, ISwap swapper)
        {
            if (comparer == null) throw new ArgumentNullException("comparer");
            if (swapper == null) throw new ArgumentNullException("swapper");

            this.comparer = comparer;
            this.swapper = swapper;
        }

        #endregion

        #region Public Property

        /// <summary>
        /// Gets or sets the <see cref="IComparer"/> object
        /// </summary>
        /// <value>
        /// Comparer object
        /// </value>
        /// <exception cref="ArgumentNullException">
        /// Set property, the value is a null reference
        /// </exception>
        public IComparer Comparer
        {
            get { return this.comparer; }
            set
            {
                if (value == null) throw new ArgumentNullException("comparer");
                this.comparer = value;
            }
        }

        /// <summary>
        /// Gets or set the swapper object
        /// </summary>
        /// <value>
        /// The <see cref="ISwap"/> swapper.
        /// </value>
        /// <exception cref="ArgumentNullException">Swapper is a null reference</exception>
        public ISwap Swapper
        {
            get { return this.swapper; }
            set
            {
                if (value == null) throw new ArgumentNullException("swapper");
                this.swapper = value;
            }
        }

        #endregion

        #region Abstract Method

        /// <summary>
        /// Sort IList Instance
        /// </summary>
        /// <param name="list">IList Instance</param>
        public abstract void Sort(IList list);

        #endregion
    }

    #endregion

    #endregion

    #region Each Sort

    #region Bubble sort

    /// <summary>
    /// Bubble sort sequential algorithm
    /// </summary>
    public class BubbleSort : AbstractSorter
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public BubbleSort() : base() { }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="comparer">comparer object instance</param>
        /// <param name="swapper">swaper object instance</param>
        public BubbleSort(IComparer comparer, ISwap swapper) : base(comparer, swapper) { }

        #endregion

        #region Abstract Implements

        /// <summary>
        /// Sorts the array.
        /// </summary>
        /// <param name="list">The array to sort.</param>
        public override void Sort(IList list)
        {
            for (int i = list.Count; --i >= 0; )
            {
                for (int j = 0; j < i; j++)
                {
                    if (Comparer.Compare(list[j], list[j + 1]) > 0) Swapper.Swap(list, j, j + 1);
                }
            }
        }

        #endregion
    }

    #endregion

    #region Bidirection Bubble Sort

    /// <summary>
    /// Bi Directional Bubble sort sequential algorithm
    /// </summary>
    public class BiDirectionalBubbleSort : AbstractSorter
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public BiDirectionalBubbleSort() : base() { }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="comparer">comparer object instance</param>
        /// <param name="swapper">swaper object instance</param>
        public BiDirectionalBubbleSort(IComparer comparer, ISwap swapper) : base(comparer, swapper) { }

        #endregion

        #region Abstract Implement

        /// <summary>
        /// Sorts the array.
        /// </summary>
        /// <param name="list">The array to sort.</param>
        public override void Sort(IList list)
        {
            int j;
            int limit;
            int st;
            bool flipped;

            st = -1;
            limit = list.Count;
            flipped = true;
            while (st < limit & flipped)
            {
                flipped = false;
                st++;
                limit--;
                for (j = st; j < limit; j++)
                {
                    if (Comparer.Compare(list[j], list[j + 1]) > 0)
                    {
                        Swapper.Swap(list, j, j + 1);
                        flipped = true;
                    }
                }

                if (flipped)
                {
                    for (j = limit - 1; j >= st; j--)
                    {
                        if (Comparer.Compare(list[j], list[j + 1]) > 0)
                        {
                            Swapper.Swap(list, j, j + 1);
                            flipped = true;
                        }
                    }
                }
            }
        }

        #endregion
    }

    #endregion

    #region In Place Merge Sort

    /// <summary>
    /// InPlace Merge Sort algorithm
    /// </summary>
    public class InPlaceMergeSort : AbstractSorter
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public InPlaceMergeSort() : base() { }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="comparer">comparer object instance</param>
        /// <param name="swapper">swaper object instance</param>
        public InPlaceMergeSort(IComparer comparer, ISwap swapper) : base(comparer, swapper) { }

        #endregion

        #region Abstract Implement

        /// <summary>
        /// Sorts the array.
        /// </summary>
        /// <param name="list">The array to sort.</param>
        public override void Sort(IList list)
        {
            Sort(list, 0, list.Count - 1);
        }

        #endregion

        #region Private Method

        private void Sort(IList list, int fromPos, int toPos)
        {
            int end_low;
            int start_high;
            int i;
            object tmp;
            int mid;

            if (fromPos < toPos)
            {
                mid = (fromPos + toPos) / 2;

                Sort(list, fromPos, mid);
                Sort(list, mid + 1, toPos);

                end_low = mid;
                start_high = mid + 1;

                while (fromPos <= end_low & start_high <= toPos)
                {
                    if (Comparer.Compare(list[fromPos], list[start_high]) < 0)
                    {
                        fromPos++;
                    }
                    else
                    {
                        tmp = list[start_high];
                        for (i = start_high - 1; i >= fromPos; i--) Swapper.Assign(list, i + 1, list[i]);
                        Swapper.Assign(list, fromPos, tmp);
                        fromPos++;
                        end_low++;
                        start_high++;
                    }
                }
            }
        }

        #endregion
    }

    #endregion

    #region Double Storage Merge Sort

    /// <summary>
    /// Double Storage Merge Sort algorithm
    /// </summary>
    public class DoubleStorageMergeSort : AbstractSorter
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public DoubleStorageMergeSort() : base() { }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="comparer">comparer object instance</param>
        /// <param name="swapper">swaper object instance</param>
        public DoubleStorageMergeSort(IComparer comparer, ISwap swapper) : base(comparer, swapper) { }

        #endregion

        #region Abstract Implement

        /// <summary>
        /// Sorts the array.
        /// </summary>
        /// <param name="list">The array to sort.</param>
        public override void Sort(IList list)
        {
            object[] scratch = new IComparable[list.Count];
            Sort(list, 0, list.Count - 1, scratch);
        }

        #endregion

        #region Private Method

        private void Sort(IList list, int fromPos, int toPos, object[] scratch)
        {
            int mid = 0;
            int i;
            int t_low;
            int t_high;

            if (fromPos < toPos)
            {
                mid = (fromPos + toPos) / 2;
                Sort(list, fromPos, mid, scratch);
                Sort(list, mid + 1, toPos, scratch);

                t_low = fromPos;
                t_high = mid + 1;
                for (i = fromPos; i <= toPos; i++)
                {
                    if (t_low <= mid)
                    {
                        if (t_high > toPos)
                        {
                            scratch[i] = list[t_low];
                            t_low++;
                        }
                        else
                        {
                            if (Comparer.Compare(list[t_low], list[t_high]) < 0)
                            {
                                scratch[i] = list[t_low];
                                t_low++;
                            }
                            else
                            {
                                scratch[i] = list[t_high];
                                t_high++;
                            }
                        }
                    }
                    else
                    {
                        scratch[i] = list[t_high];
                        t_high++;
                    }
                }
                for (i = fromPos; i <= toPos; i++) Swapper.Assign(list, i, scratch[i]);
            }
        }

        #endregion
    }

    #endregion

    #region Quick Sort

    /// <summary>
    /// Quick Sort algorithm
    /// </summary>
    public class QuickSort : AbstractSorter
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public QuickSort() : base() { }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="comparer">comparer object instance</param>
        /// <param name="swapper">swaper object instance</param>
        public QuickSort(IComparer comparer, ISwap swapper) : base(comparer, swapper) { }

        #endregion

        #region Abstract Implement

        /// <summary>
        /// Sorts the array.
        /// </summary>
        /// <param name="array">The array to sort.</param>
        public override void Sort(IList array)
        {
            Sort(array, 0, array.Count - 1);
        }

        #endregion

        #region Public Method

        /// <summary>
        /// Sort Array (IList) with specificed lower and upper bound
        /// </summary>
        /// <param name="array">Array to Sort</param>
        /// <param name="lower">Lower bound index</param>
        /// <param name="upper">Upper bound index</param>
        public void Sort(IList array, int lower, int upper)
        {
            // Check for non-base case
            if (lower < upper)
            {
                // Split and sort partitions
                int split = Pivot(array, lower, upper);
                Sort(array, lower, split - 1);
                Sort(array, split + 1, upper);
            }
        }

        #endregion

        #region Internal

        internal int Pivot(IList array, int lower, int upper)
        {
            // Pivot with first element
            int left = lower + 1;
            object pivot = array[lower];
            int right = upper;

            // Partition array elements
            while (left <= right)
            {
                // Find item out of place
                while ((left <= right) && (Comparer.Compare(array[left], pivot) <= 0)) ++left;
                while ((left <= right) && (Comparer.Compare(array[right], pivot) > 0)) --right;
                // Swap values if necessary
                if (left < right)
                {
                    Swapper.Swap(array, left, right);
                    ++left;
                    --right;
                }
            }
            // Move pivot element
            Swapper.Swap(array, lower, right);
            return right;
        }

        #endregion
    }

    #endregion

    #region Array Quick Sort

    /// <summary>
    /// Array Quick Sort algorithm
    /// </summary>
    public class ArrayQuickSort : AbstractSorter
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public ArrayQuickSort() : base() { }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="comparer">comparer object instance</param>
        /// <param name="swapper">swaper object instance</param>
        public ArrayQuickSort(IComparer comparer, ISwap swapper) : base(comparer, swapper) { }

        #endregion

        #region Abstract Implement

        /// <summary>
        /// Sorts the array.
        /// </summary>
        /// <param name="array">The array to sort.</param>
        public override void Sort(IList array)
        {
            Array.Sort((object[])array, this.Comparer);
        }

        #endregion
    }

    #endregion

    #region Fast Quick Sort

    /// <summary>
    /// Fast Quick Sort algorithm
    /// </summary>
    public class FastQuickSort : AbstractSorter
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public FastQuickSort() : base() { }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="comparer">comparer object instance</param>
        /// <param name="swapper">swaper object instance</param>
        public FastQuickSort(IComparer comparer, ISwap swapper) : base(comparer, swapper) { }

        #endregion

        #region Abstract Implement

        /// <summary>
        /// Sorts the array.
        /// </summary>
        /// <param name="list">The array to sort.</param>
        public override void Sort(IList list)
        {
            QuickSort(list, 0, list.Count - 1);
            InsertionSort(list, 0, list.Count - 1);
        }

        #endregion

        #region Internal

        /// <summary>
        /// This is a generic version of C.A.R Hoare's Quick Sort 
        /// algorithm.  This will handle arrays that are already
        /// sorted, and arrays with duplicate keys.
        /// </summary>
        /// <remarks>
        /// If you think of a one dimensional array as going from
        /// the lowest index on the left to the highest index on the right
        /// then the parameters to this function are lowest index or
        /// left and highest index or right.  The first time you call
        /// this function it will be with the parameters 0, a.length - 1.
        /// </remarks>
        /// <param name="list">list to sort</param>
        /// <param name="l">left boundary of array partition</param>
        /// <param name="r">right boundary of array partition</param>
        internal void QuickSort(IList list, int l, int r)
        {
            int M = 4;
            int i;
            int j;
            object v;
            if ((r - l) > M)
            {
                i = (r + l) / 2;
                if (Comparer.Compare(list[l], list[i]) > 0)
                    Swapper.Swap(list, l, i);     // Tri-Median Method!
                if (Comparer.Compare(list[l], list[r]) > 0)
                    Swapper.Swap(list, l, r);
                if (Comparer.Compare(list[i], list[r]) > 0)
                    Swapper.Swap(list, i, r);

                j = r - 1;
                Swapper.Swap(list, i, j);
                i = l;
                v = list[j];
                for (; ; )
                {
                    while (Comparer.Compare(list[++i], v) > 0)
                    {
                        //Console.WriteLine("loop i");
                    }

                    while (Comparer.Compare(list[--j], v) < 0)
                    {
                        //Console.WriteLine("loop j");
                    }

                    if (j < i) break;
                    Swapper.Swap(list, i, j);
                }
                Swapper.Swap(list, i, r - 1);
                QuickSort(list, l, j);
                QuickSort(list, i + 1, r);
            }
        }

        internal void InsertionSort(IList list, int lo0, int hi0)
        {
            int i;
            int j;
            object v;
            for (i = lo0 + 1; i <= hi0; i++)
            {
                v = list[i];
                j = i;
                while ((j > lo0) && (Comparer.Compare(list[j - 1], v) > 0))
                {
                    Swapper.Assign(list, j, j - 1);
                    j--;
                }
                list[j] = v;
            }
        }

        #endregion
    }

    #endregion

    #region QuickSort With BubbleSort (Hybrid)

    /// <summary>
    /// Hybrid Bubble Fast Quick Sort algorithm
    /// </summary>
    public class HybridBubbleQuickSort : AbstractSorter
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public HybridBubbleQuickSort() : base() { }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="comparer">comparer object instance</param>
        /// <param name="swapper">swaper object instance</param>
        public HybridBubbleQuickSort(IComparer comparer, ISwap swapper) : base(comparer, swapper) { }

        #endregion

        #region Abstarct Imeplement

        /// <summary>
        /// Sorts the array.
        /// </summary>
        /// <param name="list">The array to sort.</param>
        public override void Sort(IList list)
        {
            Sort1(list, 0, list.Count - 1);
        }

        #endregion

        #region Private Method

        private void Sort1(IList list, int fromPos, int toPos)
        {
            int low;
            int high;
            object pivot;

            low = fromPos;
            high = toPos;
            if (high - low <= 16) Sort2(list, low, high);
            else
            {
                pivot = list[(low + high) / 2];
                list[(low + high) / 2] = list[high];
                list[high] = pivot;
                while (low < high)
                {
                    while (Comparer.Compare(list[low], pivot) <= 0 & low < high) low++;
                    while (Comparer.Compare(pivot, list[high]) <= 0 & low < high) high--;
                    if (low < high) Swapper.Swap(list, low, high);
                }
                Swapper.Assign(list, toPos, high);
                Swapper.Assign(list, high, pivot);
                Sort1(list, fromPos, low - 1);
                Sort1(list, high + 1, toPos);
            }
        }

        private void Sort2(IList list, int low, int high)
        {
            int j;
            int i;
            for (j = high; j > low; j--)
            {
                for (i = low; i < j; i++)
                {
                    if (Comparer.Compare(list[i], list[i + 1]) > 0) Swapper.Swap(list, i, i + 1);
                }
            }
        }

        #endregion
    }

    #endregion

    #region Heap Sort

    /// <summary>
    /// Heap Sort algorithm
    /// </summary>
    public class HeapSort : AbstractSorter
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public HeapSort() : base() { }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="comparer">comparer object instance</param>
        /// <param name="swapper">swaper object instance</param>
        public HeapSort(IComparer comparer, ISwap swapper) : base(comparer, swapper) { }

        #endregion

        #region Abstract Implement

        /// <summary>
        /// Sorts the array.
        /// </summary>
        /// <param name="list">The array to sort.</param>
        public override void Sort(IList list)
        {
            int n;
            int i;

            n = list.Count;
            for (i = n / 2; i > 0; i--) DownHeap(list, i, n);
            do
            {
                Swapper.Swap(list, 0, n - 1);
                n = n - 1;
                DownHeap(list, 1, n);
            } while (n > 1);
        }

        #endregion

        #region Private Method

        private void DownHeap(IList list, int k, int n)
        {
            int j;
            bool loop = true;

            while ((k <= n / 2) && loop)
            {
                j = k + k;
                if (j < n)
                {
                    if (Comparer.Compare(list[j - 1], list[j]) < 0) j++;
                }
                if (Comparer.Compare(list[k - 1], list[j - 1]) >= 0) loop = false;
                else
                {
                    Swapper.Swap(list, k - 1, j - 1);
                    k = j;
                }
            }
        }

        #endregion
    }

    #endregion

    #region Insertion Sort

    /// <summary>
    /// Insertion Sort algorithm
    /// </summary>
    public class InsertionSort : AbstractSorter
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public InsertionSort() : base() { }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="comparer">comparer object instance</param>
        /// <param name="swapper">swaper object instance</param>
        public InsertionSort(IComparer comparer, ISwap swapper) : base(comparer, swapper) { }

        #endregion

        #region Abstract Imeplement

        /// <summary>
        /// Sorts the array.
        /// </summary>
        /// <param name="list">The array to sort.</param>
        public override void Sort(IList list)
        {
            int i;
            int j;
            object b;
            for (i = 1; i < list.Count; i++)
            {
                j = i;
                b = list[i];
                while ((j > 0) && (Comparer.Compare(list[j - 1], b) > 0))
                {
                    Swapper.Assign(list, j, list[j - 1]);
                    --j;
                }
                Swapper.Assign(list, j, b);
            }
        }

        #endregion
    }

    #endregion

    #region Odd-Even Transport sort

    /// <summary>
    /// Odd-Even Transport sort parralel algorithm
    /// </summary>
    public class OddEvenTransportSort : AbstractSorter
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public OddEvenTransportSort() : base() { }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="comparer">comparer object instance</param>
        /// <param name="swapper">swaper object instance</param>
        public OddEvenTransportSort(IComparer comparer, ISwap swapper) : base(comparer, swapper) { }

        #endregion

        #region Abstract Implement

        /// <summary>
        /// Sorts the array.
        /// </summary>
        /// <param name="list">The array to sort.</param>
        public override void Sort(IList list)
        {
            for (int i = 0; i < list.Count / 2; ++i)
            {
                for (int j = 0; j + 1 < list.Count; j += 2)
                {
                    if (Comparer.Compare(list[j], list[j + 1]) > 0) Swapper.Swap(list, j, j + 1);
                }

                for (int j = 1; j + 1 < list.Count; j += 2)
                {
                    if (Comparer.Compare(list[j], list[j + 1]) > 0) Swapper.Swap(list, j, j + 1);
                }
            }
        }

        #endregion
    }

    #endregion

    #region Shaker Sort

    /// <summary>
    /// Shaker sort algorithm
    /// </summary>
    public class ShakerSort : AbstractSorter
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public ShakerSort() : base() { }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="comparer">comparer object instance</param>
        /// <param name="swapper">swaper object instance</param>
        public ShakerSort(IComparer comparer, ISwap swapper) : base(comparer, swapper) { }

        #endregion

        #region Abstract Imeplement

        /// <summary>
        /// Sorts the array.
        /// </summary>
        /// <param name="list">The array to sort.</param>
        public override void Sort(IList list)
        {
            int i;
            int j;
            int k;
            int min;
            int max;
            i = 0;
            k = list.Count - 1;
            while (i < k)
            {
                min = i;
                max = i;
                for (j = i + 1; j <= k; j++)
                {
                    if (Comparer.Compare(list[j], list[min]) < 0) min = j;
                    if (Comparer.Compare(list[j], list[max]) > 0) max = j;
                }
                Swapper.Swap(list, min, i);
                if (max == i) Swapper.Swap(list, min, k);
                else Swapper.Swap(list, max, k);
                i++;
                k--;
            }
        }

        #endregion
    }

    #endregion

    #region Shear Sort

    /// <summary>
    /// Shear sort parralel algorithm
    /// </summary>
    public class ShearSort : AbstractSorter
    {
        #region Internal Variable

        private int rows;
        private int cols;
        private int log;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public ShearSort() : base() { }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="comparer">comparer object instance</param>
        /// <param name="swapper">swaper object instance</param>
        public ShearSort(IComparer comparer, ISwap swapper) : base(comparer, swapper) { }

        #endregion

        #region Abstract Imeplement

        /// <summary>
        /// Sorts the array.
        /// </summary>
        /// <param name="list">The array to sort.</param>
        public override void Sort(IList list)
        {
            int pow = 1, div = 1;
            for (int i = 1; i * i <= list.Count; ++i)
            {
                if (list.Count % i == 0) div = i;
            }

            this.rows = div;
            this.cols = list.Count / div;
            for (this.log = 0; pow <= this.rows; ++this.log) pow = pow * 2;

            int[] h = new int[this.rows];
            for (int i = 0; i < this.rows; ++i) h[i] = i * this.cols;

            for (int k = 0; k < this.log; ++k)
            {
                for (int j = 0; j < this.cols / 2; ++j)
                {
                    for (int i = 0; i < this.rows; i++)
                        SortPart1(list, i * this.cols, (i + 1) * this.cols, 1, (i % 2 == 0 ? true : false));
                    for (int i = 0; i < this.rows; i++)
                        SortPart2(list, i * this.cols, (i + 1) * this.cols, 1, (i % 2 == 0 ? true : false));
                }
                for (int j = 0; j < this.rows / 2; j++)
                {
                    for (int i = 0; i < this.cols; i++)
                        SortPart1(list, i, this.rows * this.cols + i, this.cols, true);
                    for (int i = 0; i < this.cols; i++)
                        SortPart2(list, i, this.rows * this.cols + i, this.cols, true);
                }
            }
            for (int j = 0; j < this.cols / 2; j++)
            {
                for (int i = 0; i < this.rows; i++)
                    SortPart1(list, i * this.cols, (i + 1) * this.cols, 1, true);
                for (int i = 0; i < this.rows; i++)
                    SortPart2(list, i * this.cols, (i + 1) * this.cols, 1, true);
            }
            for (int i = 0; i < this.rows; i++)
                h[i] = -1;
        }

        #endregion

        #region Internal Method

        internal void SortPart1(IList list, int Lo, int Hi, int Nx, bool Up)
        {
            int c;
            for (int j = Lo; j + Nx < Hi; j += 2 * Nx)
            {
                c = Comparer.Compare(list[j], list[j + Nx]);
                if ((Up && c > 0) || !Up && c < 0) Swapper.Swap(list, j, j + Nx);
            }
        }

        internal void SortPart2(IList list, int Lo, int Hi, int Nx, bool Up)
        {
            int c;
            for (int j = Lo + Nx; j + Nx < Hi; j += 2 * Nx)
            {
                c = Comparer.Compare(list[j], list[j + Nx]);
                if ((Up && c > 0) || !Up && c < 0) Swapper.Swap(list, j, j + Nx);
            }
        }

        #endregion
    }

    #endregion

    #region Shell Sort

    /// <summary>
    /// Shell sort algorithm
    /// </summary>
    public class ShellSort : AbstractSorter
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public ShellSort() : base() { }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="comparer">comparer object instance</param>
        /// <param name="swapper">swaper object instance</param>
        public ShellSort(IComparer comparer, ISwap swapper) : base(comparer, swapper) { }

        #endregion

        #region Abstract Implement

        /// <summary>
        /// Sorts the array.
        /// </summary>
        /// <param name="list">The array to sort.</param>
        public override void Sort(IList list)
        {
            int h;
            int i;
            int j;
            object b;
            bool loop = true;
            h = 1;
            while (h * 3 + 1 <= list.Count) h = 3 * h + 1;
            while (h > 0)
            {
                for (i = h - 1; i < list.Count; i++)
                {
                    b = list[i];
                    j = i;
                    loop = true;
                    while (loop)
                    {
                        if (j >= h)
                        {
                            if (Comparer.Compare(list[j - h], b) > 0)
                            {
                                Swapper.Assign(list, j, j - h);
                                j = j - h;
                            }
                            else loop = false;
                        }
                        else loop = false;
                    }
                    Swapper.Assign(list, j, b);
                }
                h = h / 3;
            }
        }

        #endregion
    }

    #endregion

    #endregion
}

namespace NLib.Collections
{
    #region IComparer implement classes

    #region Comparable Comparer

    /// <summary>
    /// Default <see cref="IComparable"/> object comparer.
    /// </summary>
    internal class ComparableComparer : IComparer
    {
        #region Public Method

        /// <summary>
        /// Compare
        /// </summary>
        /// <param name="x">IComparable instance</param>
        /// <param name="y">object to compare</param>
        /// <returns>0 if both object is equal. less than zero if x less than y and otherwise return value greater than zero</returns>
        public int Compare(IComparable x, object y)
        {
            return x.CompareTo(y);
        }

        #endregion

        #region IComparer Members

        /// <summary>
        /// IComparer.Compare
        /// </summary>
        /// <param name="x">object x to compare</param>
        /// <param name="y">object y to compare</param>
        /// <returns>See IComparer.Compare</returns>
        int IComparer.Compare(object x, object y)
        {
            return this.Compare((IComparable)x, y);
        }

        #endregion
    }

    #endregion

    #region Object Comparer

    /// <summary>
    /// General Object Comparer
    /// </summary>
    public class ObjectComparer : IComparer
    {
        private string[] Fields;
        private bool[] Descending;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fields">field list to compare</param>
        /// <param name="descending">descending field list</param>
        public ObjectComparer(string[] fields, bool[] descending)
        {
            Fields = fields;
            Descending = descending;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fields">field list to compare</param>
        public ObjectComparer(params string[] fields) : this(fields, new bool[fields.Length]) { }

        /// <summary>
        /// Compare
        /// </summary>
        /// <param name="x">object 1 to compare</param>
        /// <param name="y">object 2 to compare</param>
        /// <returns>see IComparer.Compare</returns>
        public int Compare(object x, object y)
        {
            //Get types of the objects
            Type typex = x.GetType();
            Type typey = y.GetType();

            for (int i = 0; i < Fields.Length; i++)
            {
                //Get each property by name
                PropertyInfo pix = typex.GetProperty(Fields[i]);
                PropertyInfo piy = typey.GetProperty(Fields[i]);

                //Get the value of the property for each object
                IComparable pvalx = (IComparable)pix.GetValue(x, null);
                object pvaly = piy.GetValue(y, null);

                int iResult;
                if (pvalx == null && pvaly == null)
                {
                    //nulls are equal
                    iResult = 0;
                }
                else if (pvalx == null && pvaly != null)
                {
                    //null is always less then anything else
                    iResult = -1;
                }
                else
                {
                    //Compare values, using IComparable interface of the property's type
                    iResult = pvalx.CompareTo(pvaly);
                }

                if (iResult != 0)
                {
                    //Return if not equal
                    if (Descending[i])
                    {
                        //Invert order
                        return -iResult;
                    }
                    else
                    {
                        return iResult;
                    }
                }
            }
            //Objects have the same sort order
            return 0;
        }
    }

    #endregion

    #region Multi Property Comparer

    /// <summary>
    /// Multi Property Comparer
    /// </summary>
    public class MultiPropertyComparer : IComparer
    {
        #region Internal Variable

        private bool bCase = false;
        private string[] properties = new string[] { };
        private string[] desc = new string[] { };
        private bool bUsedPropertyAccess = false;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor
        /// </summary>
        public MultiPropertyComparer() : base() { }
        /// <summary>
        /// Destructor
        /// </summary>
        ~MultiPropertyComparer() { }

        #endregion

        #region IComparer Implements

        /// <summary>
        /// Compare object
        /// </summary>
        /// <param name="objA">Object A to Compare</param>
        /// <param name="objB">Object B to Compare</param>
        /// <returns>0 - A equal to B, -1 A less than B, 1 A greater than B</returns>
        public int Compare(object objA, object objB)
        {
            if (objA == null && objB == null)
                return 0; // both is null
            else if (objA == null && objB != null)
                return -1; // obj A is exist B is not
            else if (objA != null && objB == null)
                return 1; // obj B is exist A is not
            else
            {
                if (properties.Length <= 0)
                {
                    #region Try with IClonable object
                    try
                    {
                        if (objA is ICloneable && objB is IComparable)
                            return ((IComparable)objA).CompareTo(objB);
                        else
                        {
                            if (objA == objB) return 0;
                            else
                            {
                                if (objA.Equals(objB)) return 0;
                                else return -1;
                            }
                        }
                    }
                    catch
                    {
                        throw new Exception("Properties is not set and Object is not Implement IComparable interface.");
                        //return 0;
                    }

                    #endregion
                }
                else
                {
                    if (bUsedPropertyAccess)
                    {
                        #region Use Property Access

                        IList descProps = desc;

                        int iLen = properties.Length;
                        for (int i = 0; i < iLen; i++)
                        {
                            object valA = NLib.Reflection.PropertyAccess.GetValue(objA, properties[i]);
                            object valB = NLib.Reflection.PropertyAccess.GetValue(objB, properties[i]);
                            if (valA == null && valB == null)
                                continue; // check next property
                            else if (valA == null && valB != null)
                                return (descProps.Contains(properties[i])) ? 1 : -1; // prop A is exist B is not
                            else if (valA != null && valB == null)
                                return (descProps.Contains(properties[i])) ? -1 : 1; // prop B is exist A is not
                            else
                            {
                                int result = PropertyCompare(valA, valB);
                                if (result != 0)
                                    return result; // not equal return
                            }
                        }
                        return 0;

                        #endregion
                    }
                    else
                    {
                        #region Use Normal Reflection

                        IList descProps = desc;

                        System.Reflection.PropertyInfo propA, propB;

                        Type TypeA = objA.GetType();
                        Type TypeB = objA.GetType();

                        int iLen = properties.Length;
                        for (int i = 0; i < iLen; i++)
                        {
                            propA = TypeA.GetProperty(properties[i]);
                            propB = TypeB.GetProperty(properties[i]);
                            if (propA == null && propB == null)
                                continue; // check next property
                            else if (propA == null && propB != null)
                                return (descProps.Contains(properties[i])) ? 1 : -1; // prop A is exist B is not
                            else if (propA != null && propB == null)
                                return (descProps.Contains(properties[i])) ? -1 : 1; // prop B is exist A is not
                            else
                            {
                                object valA = propA.GetValue(objA, Type.EmptyTypes);
                                object valB = propB.GetValue(objB, Type.EmptyTypes);
                                if (valA == null && valB == null)
                                    continue; // check next property
                                else if (valA == null && valB != null)
                                    return (descProps.Contains(properties[i])) ? 1 : -1; // prop A is exist B is not
                                else if (valA != null && valB == null)
                                    return (descProps.Contains(properties[i])) ? -1 : 1; // prop B is exist A is not
                                else
                                {
                                    int result = PropertyCompare(valA, valB);
                                    if (result != 0)
                                        return result; // not equal return
                                }
                            }
                        }
                        return 0;

                        #endregion
                    }
                }
            }
        }
        /// <summary>
        /// Compare object's property
        /// </summary>
        /// <param name="x">Object x to Compare</param>
        /// <param name="y">Object y to Compare</param>
        /// <returns>0 - x equal to y, -1 x less than y, 1 x greater than y</returns>
        private int PropertyCompare(object x, object y)
        {
            if (x == null && y == null) return 0;
            else if (x != null && y == null) return 1; // x greater than y
            else if (x == null && y != null) return -1; // x less than y
            else
            {
                if (x is string && y is string)
                {
                    if (!bCase)
                        return string.Compare((string)x, (string)y, true); // case insensitive
                    else return string.CompareOrdinal((string)x, (string)y);
                }
                else if (x.GetType().IsPrimitive && y.GetType().IsPrimitive)
                {
                    // The primitive types are 
                    // Boolean, Byte, SByte, Int16, UInt16, Int32, UInt32, Int64, UInt64, 
                    // Char, Double, and Single.
                    // this check condition is make slow down ~ 10%
                    // at 50000 items 0.46s -> 0.51s

                    return Comparer.DefaultInvariant.Compare(x, y);
                }
                else if (x is Enum && y is Enum)
                {
                    return Comparer.DefaultInvariant.Compare(x, y);
                }
                else if ((x is decimal && y is decimal))
                {
                    return decimal.Compare((decimal)x, (decimal)y);
                }
                else if (x is DateTime && y is DateTime)
                {
                    return DateTime.Compare((DateTime)x, (DateTime)y);
                }
                else return string.CompareOrdinal(x.ToString(), y.ToString());
            }
        }

        #endregion

        #region Public Property

        /// <summary>
        /// Get/Set Compare with Case Sensitive or not
        /// </summary>
        public bool CaseSensitive { get { return bCase; } set { bCase = value; } }
        /// <summary>
        /// Get/Set List of Property Name to Compares
        /// </summary>
        public string[] Properties { get { return properties; } set { properties = value; } }
        /// <summary>
        /// Get/Set List of Property Name that required to sort Descending (z to a)
        /// </summary>
        public string[] Descending { get { return desc; } set { desc = value; } }
        /// <summary>
        /// Get/Set Use internal property access for fast access proeprty
        /// </summary>
        public bool UsedPropertyAccess { get { return bUsedPropertyAccess; } set { bUsedPropertyAccess = value; } }

        #endregion
    }

    #endregion

    #region Enhance Multi Property Comparer

    /// <summary>
    /// Enhance Multi Property Comparer
    /// </summary>
    public class EnhanceMultiPropertyComparer : IComparer
    {
        #region Internal Variable

        private bool bCase = false;
        private string[] properties = new string[] { };
        private string[] desc = new string[] { };
        private bool bUsedDynamicAccess = false;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor
        /// </summary>
        public EnhanceMultiPropertyComparer() : base() { }
        /// <summary>
        /// Destructor
        /// </summary>
        ~EnhanceMultiPropertyComparer() { }

        #endregion

        #region IComparer Implements

        /// <summary>
        /// Compare object
        /// </summary>
        /// <param name="objA">Object A to Compare</param>
        /// <param name="objB">Object B to Compare</param>
        /// <returns>0 - A equal to B, -1 A less than B, 1 A greater than B</returns>
        public int Compare(object objA, object objB)
        {
            if (objA == null && objB == null)
                return 0; // both is null
            else if (objA == null && objB != null)
                return -1; // obj A is exist B is not
            else if (objA != null && objB == null)
                return 1; // obj B is exist A is not
            else
            {
                if (properties.Length <= 0)
                {
                    #region Try with IClonable object
                    try
                    {
                        if (objA is ICloneable && objB is IComparable)
                            return ((IComparable)objA).CompareTo(objB);
                        else
                        {
                            if (objA == objB) return 0;
                            else
                            {
                                if (objA.Equals(objB)) return 0;
                                else return -1;
                            }
                        }
                    }
                    catch
                    {
                        throw new Exception("Properties is not set and Object is not Implement IComparable interface.");
                        //return 0;
                    }

                    #endregion
                }
                else
                {
                    if (bUsedDynamicAccess)
                    {
                        #region Use Property Access

                        IList descProps = desc;

                        int iLen = properties.Length;
                        for (int i = 0; i < iLen; i++)
                        {
                            object valA = NLib.Reflection.DynamicAccess.Get(objA, properties[i]);
                            object valB = NLib.Reflection.DynamicAccess.Get(objB, properties[i]);
                            if (valA == null && valB == null)
                                continue; // check next property
                            else if (valA == null && valB != null)
                                return (descProps.Contains(properties[i])) ? 1 : -1; // prop A is exist B is not
                            else if (valA != null && valB == null)
                                return (descProps.Contains(properties[i])) ? -1 : 1; // prop B is exist A is not
                            else
                            {
                                int result = PropertyCompare(valA, valB);
                                if (result != 0)
                                    return result; // not equal return
                            }
                        }
                        return 0;

                        #endregion
                    }
                    else
                    {
                        #region Use Normal Reflection

                        IList descProps = desc;

                        System.Reflection.PropertyInfo propA, propB;

                        Type TypeA = objA.GetType();
                        Type TypeB = objA.GetType();

                        int iLen = properties.Length;
                        for (int i = 0; i < iLen; i++)
                        {
                            propA = TypeA.GetProperty(properties[i]);
                            propB = TypeB.GetProperty(properties[i]);
                            if (propA == null && propB == null)
                                continue; // check next property
                            else if (propA == null && propB != null)
                                return (descProps.Contains(properties[i])) ? 1 : -1; // prop A is exist B is not
                            else if (propA != null && propB == null)
                                return (descProps.Contains(properties[i])) ? -1 : 1; // prop B is exist A is not
                            else
                            {
                                object valA = propA.GetValue(objA, Type.EmptyTypes);
                                object valB = propB.GetValue(objB, Type.EmptyTypes);
                                if (valA == null && valB == null)
                                    continue; // check next property
                                else if (valA == null && valB != null)
                                    return (descProps.Contains(properties[i])) ? 1 : -1; // prop A is exist B is not
                                else if (valA != null && valB == null)
                                    return (descProps.Contains(properties[i])) ? -1 : 1; // prop B is exist A is not
                                else
                                {
                                    int result = PropertyCompare(valA, valB);
                                    if (result != 0)
                                        return result; // not equal return
                                }
                            }
                        }
                        return 0;

                        #endregion
                    }
                }
            }
        }
        /// <summary>
        /// Compare object's property
        /// </summary>
        /// <param name="x">Object x to Compare</param>
        /// <param name="y">Object y to Compare</param>
        /// <returns>0 - x equal to y, -1 x less than y, 1 x greater than y</returns>
        private int PropertyCompare(object x, object y)
        {
            if (x == null && y == null) return 0;
            else if (x != null && y == null) return 1; // x greater than y
            else if (x == null && y != null) return -1; // x less than y
            else
            {
                if (x is string && y is string)
                {
                    if (!bCase)
                        return string.Compare((string)x, (string)y, true); // case insensitive
                    else return string.CompareOrdinal((string)x, (string)y);
                }
                else if (x.GetType().IsPrimitive && y.GetType().IsPrimitive)
                {
                    // The primitive types are 
                    // Boolean, Byte, SByte, Int16, UInt16, Int32, UInt32, Int64, UInt64, 
                    // Char, Double, and Single.
                    // this check condition is make slow down ~ 10%
                    // at 50000 items 0.46s -> 0.51s

                    return Comparer.DefaultInvariant.Compare(x, y);
                }
                else if (x is Enum && y is Enum)
                {
                    return Comparer.DefaultInvariant.Compare(x, y);
                }
                else if ((x is decimal && y is decimal))
                {
                    return decimal.Compare((decimal)x, (decimal)y);
                }
                else if (x is DateTime && y is DateTime)
                {
                    return DateTime.Compare((DateTime)x, (DateTime)y);
                }
                else return string.CompareOrdinal(x.ToString(), y.ToString());
            }
        }

        #endregion

        #region Public Property

        /// <summary>
        /// Get/Set Compare with Case Sensitive or not
        /// </summary>
        public bool CaseSensitive { get { return bCase; } set { bCase = value; } }
        /// <summary>
        /// Get/Set List of Property Name to Compares
        /// </summary>
        public string[] Properties { get { return properties; } set { properties = value; } }
        /// <summary>
        /// Get/Set List of Property Name that required to sort Descending (z to a)
        /// </summary>
        public string[] Descending { get { return desc; } set { desc = value; } }
        /// <summary>
        /// Get/Set Use internal dynamic access for fast access proeprty
        /// </summary>
        public bool UsedDynamicAccess { get { return bUsedDynamicAccess; } set { bUsedDynamicAccess = value; } }

        #endregion
    }

    #endregion

    #endregion

    #region Sorted Manager

    /// <summary>
    /// IList Sort Manager Helper Class
    /// </summary>
    public sealed class SortedManager
    {
        #region Static Sort Method

        /// <summary>
        /// Sort Data in List
        /// </summary>
        /// <param name="datasource">source list</param>
        /// <param name="SortedBy">sorted property list</param>
        /// <returns>sorted list (same instance as source)</returns>
        public static IList Sort(IList datasource, string[] SortedBy)
        {
            return Sort(datasource, SortedBy, false);
        }
        /// <summary>
        /// Sort Data in List
        /// </summary>
        /// <param name="datasource">source list</param>
        /// <param name="SortedBy">sorted property list</param>
        /// <param name="usePropertyAccess">used property access instead of reflection</param>
        /// <returns>sorted list (same instance as source)</returns>
        public static IList Sort(IList datasource, string[] SortedBy, bool usePropertyAccess)
        {
            if (SortedBy.Length <= 0) return datasource;
            if (datasource == null || datasource.Count <= 0) return datasource;
            IList source = datasource;

            return Sort(datasource, SortedBy, null, usePropertyAccess);
        }
        /// <summary>
        /// Sort Data in List
        /// </summary>
        /// <param name="datasource">source list</param>
        /// <param name="SortedBy">sorted property list</param>
        /// <param name="Descending">property that sorted Descending</param>
        /// <returns>sorted list (same instance as source)</returns>
        public static IList Sort(IList datasource, string[] SortedBy, string[] Descending)
        {
            return Sort(datasource, SortedBy, Descending, false);
        }
        /// <summary>
        /// Sort Data in List
        /// </summary>
        /// <param name="datasource">source list</param>
        /// <param name="SortedBy">sorted property list</param>
        /// <param name="Descending">property that sorted Descending</param>
        /// <param name="usePropertyAccess">used property access instead of reflection</param>
        /// <returns>sorted list (same instance as source)</returns>
        public static IList Sort(IList datasource, string[] SortedBy, string[] Descending, bool usePropertyAccess)
        {
            if (SortedBy.Length <= 0) return datasource;
            if (datasource == null || datasource.Count <= 0) return datasource;
            IList source = datasource;

            MultiPropertyComparer cmp = new MultiPropertyComparer();
            ArrayQuickSort sorter = new ArrayQuickSort();

            cmp.Properties = SortedBy;
            cmp.UsedPropertyAccess = usePropertyAccess;
            if (Descending != null) cmp.Descending = Descending;
            sorter.Comparer = cmp;
            sorter.Sort(source);

            return source;
        }
        /// <summary>
        /// Sort Data in List
        /// </summary>
        /// <param name="datasource">source list</param>
        /// <param name="elementType">element type in list</param>
        /// <param name="SortedBy">sorted property list</param>
        /// <returns>sorted list (new instance of list will created)</returns>
        public static IList Sort(IList datasource, Type elementType, string[] SortedBy)
        {
            return Sort(datasource, elementType, SortedBy, false);
        }
        /// <summary>
        /// Sort Data in List
        /// </summary>
        /// <param name="datasource">source list</param>
        /// <param name="elementType">element type in list</param>
        /// <param name="SortedBy">sorted property list</param>
        /// <param name="usePropertyAccess">used property access instead of reflection</param>
        /// <returns>sorted list (new instance of list will created)</returns>
        public static IList Sort(IList datasource, Type elementType, string[] SortedBy, bool usePropertyAccess)
        {
            if (datasource == null) return null;
            if (elementType == null) return null; // not support untyped element

            return Sort(datasource, elementType, SortedBy, null, usePropertyAccess);
        }
        /// <summary>
        /// Sort Data in List
        /// </summary>
        /// <param name="datasource">source list</param>
        /// <param name="elementType">element type in list</param>
        /// <param name="SortedBy">sorted property list</param>
        /// <param name="Descending">property that sorted Descending</param>
        /// <returns>sorted list (new instance of list will created)</returns>
        public static IList Sort(IList datasource, Type elementType, string[] SortedBy, string[] Descending)
        {
            return Sort(datasource, elementType, SortedBy, Descending, false);
        }
        /// <summary>
        /// Sort Data in List
        /// </summary>
        /// <param name="datasource">source list</param>
        /// <param name="elementType">element type in list</param>
        /// <param name="SortedBy">sorted property list</param>
        /// <param name="Descending">property that sorted Descending</param>
        /// <param name="usePropertyAccess">used property access instead of reflection</param>
        /// <returns>sorted list (new instance of list will created)</returns>
        public static IList Sort(IList datasource, Type elementType, string[] SortedBy, string[] Descending,
            bool usePropertyAccess)
        {
            if (datasource == null) return null;
            if (elementType == null) return null; // not support untyped element

            Array source = Array.CreateInstance(elementType, datasource.Count);

            if (SortedBy.Length <= 0) return source;
            datasource.CopyTo(source, 0);

            //bool isAbstractObj = elementType.IsSubclassOf(typeof(SysLib.Lib.Objects.AbstractObject));

            if (!usePropertyAccess)
            {
                #region Used Reflection to copy

                System.Reflection.PropertyInfo[] infos = elementType.GetProperties();
                int propCnt = infos.Length;
                int elementCnt = datasource.Count;
                for (int i = 0; i < elementCnt; i++)
                {
                    (source as IList)[i] = System.Activator.CreateInstance(elementType, Type.EmptyTypes);
                    //if (isAbstractObj) ((SysLib.Lib.Objects.AbstractObject )(((IList )source)[i])).Lock();
                    for (int j = 0; j < propCnt; j++)
                    {
                        try
                        {
                            object val = infos[j].GetValue(datasource[i], Type.EmptyTypes);
                            if (val != null && infos[j].PropertyType.IsEnum)
                            {
                                val = Enum.ToObject(infos[j].PropertyType, val);
                            }
                            infos[j].SetValue((source as IList)[i], val, Type.EmptyTypes);
                        }
                        catch
                        {
                        }
                    }
                    //if (isAbstractObj) ((SysLib.Lib.Objects.AbstractObject )(((IList )source)[i])).Unlock();
                }

                #endregion
            }
            else
            {
                #region Use Property Access

                System.Reflection.PropertyInfo[] infos = elementType.GetProperties();
                int propCnt = infos.Length;
                int elementCnt = datasource.Count;
                for (int i = 0; i < elementCnt; i++)
                {
                    (source as IList)[i] = NLib.Reflection.TypeAccess.CreateInstance(elementType);
                    for (int j = 0; j < propCnt; j++)
                    {
                        object val = NLib.Reflection.PropertyAccess.GetValue(datasource[i], infos[j].Name);
                        if (val != null)
                        {
                            NLib.Reflection.PropertyAccess.SetValue((source as IList)[i], infos[j].Name, val);
                        }
                    }
                }

                #endregion
            }

            MultiPropertyComparer cmp = new MultiPropertyComparer();
            ArrayQuickSort sorter = new ArrayQuickSort();

            cmp.Properties = SortedBy;
            cmp.UsedPropertyAccess = usePropertyAccess;
            if (Descending != null) cmp.Descending = Descending;
            sorter.Comparer = cmp;
            sorter.Sort(source);

            return source;
        }

        #endregion
    }

    #endregion
}
