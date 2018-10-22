using System;
using System.Collections;
using System.Collections.Generic;

namespace gt.Collections.Ordered.Helpers
{
    public class BinarySearchHelper
    {
        /// <summary>
        /// Binary search algorithm. 
        /// </summary>        
        /// <returns>Same result as <see cref="Array.BinarySearch(System.Array,int,int,object)"/></returns>
        public static int BinarySearch(IList array, object value, IComparer comparer) => 
            BinarySearch(array, 0, array?.Count ?? 0, value, comparer);

        /// <summary>
        /// Binary search algorithm. 
        /// </summary>        
        /// <returns>Same result as <see cref="Array.BinarySearch(System.Array,int,int,object)"/></returns>
        public static int BinarySearch<T>(IList<T> array, T value, IComparer<T> comparer) =>
            BinarySearch(array, 0, array?.Count ?? 0, value, comparer);

        /// <summary>
        /// Binary search algorithm. 
        /// </summary>        
        /// <returns>Same result as <see cref="Array.BinarySearch(System.Array,int,int,object)"/></returns>
        public static int BinarySearch(IList array, int index, int length, object value, IComparer comparer)
        {            
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            if (array.Count - index < length)
                throw new ArgumentException("Count - index < length");

            if (comparer == null)
                comparer = Comparer.Default;

            int lo = index;
            int hi = index + length - 1;

            while (lo <= hi)
            {
                // overflow possible
                int median = lo + ((hi - lo) >> 1);

                var c = comparer.Compare(array[median], value);
 
                if (c == 0)
                    return median;

                if (c < 0)
                    lo = median + 1;
                else
                    hi = median - 1;
            }
            return ~lo;
        }

        /// <summary>
        /// Binary search algorithm. 
        /// </summary>        
        /// <returns>Same result as <see cref="Array.BinarySearch(System.Array,int,int,object)"/></returns>
        public static int BinarySearch<T>(IList<T> array, int index, int length, T value, IComparer<T> comparer)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            if (array.Count - index < length)
                throw new ArgumentException("Count - index < length");

            if (comparer == null)
                comparer = Comparer<T>.Default;

            int lo = index;
            int hi = index + length - 1;

            while (lo <= hi)
            {
                // overflow possible
                int median = lo + ((hi - lo) >> 1);

                var c = comparer.Compare(array[median], value);

                if (c == 0)
                    return median;

                if (c < 0)
                    lo = median + 1;
                else
                    hi = median - 1;
            }
            return ~lo;
        }
    }
}