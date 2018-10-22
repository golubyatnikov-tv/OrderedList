using System.Collections;
using System.Collections.Generic;
using System.Linq;
using gt.Collections.Ordered;

namespace gt.Collections.Ordered.Helpers
{
    public static class SortingHelper
    {
        public static int CalculateSortedInsertIndex<T>(List<T> sortedList, T orderValue, IComparer<T> comparer)
        {            
            var index = sortedList.BinarySearch(orderValue, comparer);
            return index;
        }

        public static int CalculateSortedInsertIndex(IList sortedList, object orderValue, SortMethod method, IComparer comparer = null)
        {
            if (comparer == null)
                comparer = Comparer.Default;
            var index = BinarySearchHelper.BinarySearch(sortedList, orderValue, comparer);
            if (index < 0)
                return ~index;
            if (method == SortMethod.BisectLeft)
            {
                while (index - 1 >= 0 && comparer.Compare(orderValue, sortedList[index-1]) == 0)
                {
                    --index;
                }
            }
            if (method == SortMethod.BisectRight)
            {                
                while (index + 1 < sortedList.Count && comparer.Compare(orderValue, sortedList[++index]) == 0)
                {
                }
            }
            return index;
        }

        public static int CalculateSortedInsertIndex<TOrder>(IList<TOrder> sortedList, TOrder orderValue, SortMethod method, IComparer<TOrder> comparer = null)
        {
            if (comparer == null)
                comparer = Comparer<TOrder>.Default;
            var index = BinarySearchHelper.BinarySearch(sortedList, orderValue, comparer);
            if (index < 0)
                return ~index;
            if (method == SortMethod.BisectLeft)
            {
                while (index - 1 >= 0 && comparer.Compare(orderValue, sortedList[index - 1]) == 0)
                {
                    --index;
                }
            }
            if (method == SortMethod.BisectRight)
            {
                while (index + 1 < sortedList.Count && comparer.Compare(orderValue, sortedList[++index]) == 0)
                {
                }
            }
            return index;
        }

        private static int DefaultImplementation<T>(List<T> sortedList, object orderValue, IComparer comparer)
        {
            if (sortedList.Count <= 0)
                return 0;

            if (comparer.Compare(sortedList.First(), orderValue) > 0)
                return 0;

            if (comparer.Compare(sortedList.Last(), orderValue) <= 0)
                return sortedList.Count;

            var index = sortedList.FindLastIndex(c => comparer.Compare(c, orderValue) <= 0);

            if (index < 0)
                return sortedList.Count;

            return index + 1;
        }
        
        public static int Bisect<T>(IList<T> array, object target, IComparer comparer = null)
        {
            if (array == null || array.Count == 0)
                return 0;
            var actualComparer = comparer ?? Comparer.Default;

            var min = 0;
            var max = array.Count - 1;
            while (min <= max)
            {
                var mid = (min + max) / 2;
                var compared = actualComparer.Compare(target, array[mid]);
                if (compared == 0)
                    return ++mid;

                if (compared < 0)
                    max = mid - 1;
                else
                    min = mid + 1;
            }
            return min;
        }
    }
}