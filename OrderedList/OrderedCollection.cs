using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using gt.Collections.Ordered;
using gt.Collections.Ordered.Helpers;

namespace gt.Collections.Ordered
{
    /// <summary>
    /// Collection that sorts items when adding
    /// </summary>
    public class OrderedCollection<T> : OrderedCollection<T, object>
    {
        public static OrderedCollection<T, TOrder> Create<TOrder>(Expression<Func<T, TOrder>> orderByFunc,
            Comparer<TOrder> orderComparer = null, SortMethod sortMethod = SortMethod.BisectRight)
        {
            return new OrderedCollection<T, TOrder>(orderByFunc, orderComparer, sortMethod);            
        }

        public OrderedCollection(Expression<Func<T, object>> orderByFunc, IComparer<object> orderComparer = null, SortMethod sortMethod = SortMethod.BisectRight) : base(orderByFunc, orderComparer, sortMethod)
        {
        }
    }

    /// <summary>
    /// Collection that sorts items when adding
    /// </summary>
    public class OrderedCollection<T, TOrder> : ICollection<T>, IOrderedCollection<T>
    {        
        public OrderedCollection(Expression<Func<T, TOrder>> orderByFunc, IComparer<TOrder> orderComparer = null, SortMethod sortMethod = SortMethod.BisectRight)
        {
            if (typeof(T).GetInterfaces().Contains(typeof(INotifyPropertyChanged)))
            {
                _listenPropertyNotifications = ReflectionHelper.GetPropertyChain(orderByFunc).FirstOrDefault();
            }

            _orderComparer = orderComparer;
            _sortMethod = sortMethod;
            _orderByFunc = orderByFunc.Compile();            
        }

        private readonly Func<T, TOrder> _orderByFunc;
        private readonly IComparer<TOrder> _orderComparer;
        private readonly SortMethod _sortMethod;
        private readonly List<TOrder> _orderValues = new List<TOrder>();
        private readonly List<T> _implementation = new List<T>();
        private readonly string _listenPropertyNotifications;        

        public IEnumerator<T> GetEnumerator()
        {
            return _implementation.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) _implementation).GetEnumerator();
        }

        void ICollection<T>.Add(T item)
        {
            Add(item);
        }

        public int Add(T item)
        {
            var order = _orderByFunc(item);
            var index = SortingHelper.CalculateSortedInsertIndex(_orderValues, order, _sortMethod);
            InsertInternal(index, item, order);
            return index;
        }

        public void Clear()
        {
            _orderValues.Clear();
            _implementation.Clear();
        }

        public bool Contains(T item)
        {
            return _implementation.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _implementation.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            var index = _implementation.IndexOf(item);
            if (index < 0) return false;
            RemoveInternal(index, item);
            return true;
        }

        public int Count => _implementation.Count;

        bool ICollection<T>.IsReadOnly => ((ICollection<T>)_implementation).IsReadOnly;

        private void InsertInternal(int index, T item, TOrder orderValue)
        {
            if (_listenPropertyNotifications != null)
                if (item is INotifyPropertyChanged notifier)
                    notifier.PropertyChanged += OnItemPropertyChanged;

            _orderValues.Insert(index, orderValue);
            _implementation.Insert(index, item);
        }

        private void RemoveInternal(int index, T item)
        {
            if (_listenPropertyNotifications != null)
                if (item is INotifyPropertyChanged notifier)
                    notifier.PropertyChanged -= OnItemPropertyChanged;

            _orderValues.RemoveAt(index);
            _implementation.RemoveAt(index);
        }

        private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == _listenPropertyNotifications)
            {
                Remove((T)sender);
                Add((T)sender);
            }
        }
    }
}