using System.Collections.Generic;

namespace gt.Collections.Ordered
{
    public interface IOrderedCollection<T> : ICollection<T>
    {
        new int Add(T item);
    }
}