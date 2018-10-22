using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using gt.Collections.Ordered;
using gt.Collections.Ordered.Helpers;

namespace gt.Collections.OrderedList
{
    /// <summary>
    /// Use <see cref="Ordered.OrderedList{T}" /> instead. This class will be removed in the next version of package. For backward compatibility only."/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Obsolete("Use gt.Collections.Ordered.OrderedList<T>. This class will be removed in the next version of package. For backward compatibility only.")]
    public class OrderedList<T> : Ordered.OrderedList<T>
    {
        public OrderedList(Expression<Func<T, object>> orderByFunc, IComparer<object> orderComparer = null, SortMethod sortMethod = SortMethod.BisectRight) : base(orderByFunc, orderComparer, sortMethod)
        {
        }
    }
}

namespace gt.Collections.Ordered
{
    /// <summary>
    /// List that sorts items when adding
    /// </summary>    
    public class OrderedList<T> : OrderedList<T, object>
    {
        public static OrderedList<T, TOrder> Create<TOrder>(Expression<Func<T, TOrder>> orderByFunc,
            Comparer<TOrder> orderComparer = null, SortMethod sortMethod = SortMethod.BisectRight)
        {
            return new OrderedList<T, TOrder>(orderByFunc, orderComparer, sortMethod);
        }

        public OrderedList(Expression<Func<T, object>> orderByFunc, IComparer<object> orderComparer = null, SortMethod sortMethod = SortMethod.BisectRight) : base(orderByFunc, orderComparer, sortMethod)
        {
        }
    }

    /// <summary>
    /// List that sorts items when adding
    /// </summary>
    public class OrderedList<T, TOrder> : IList<T>, IReadOnlyList<T>, IList, IOrderedCollection<T>
    {        
        public OrderedList(Expression<Func<T, TOrder>> orderByFunc, IComparer<TOrder> comparer = null, SortMethod sortMethod = SortMethod.BisectRight)
        {
            _sortMethod = sortMethod;
            _comparer = comparer ?? Comparer<TOrder>.Default;
            if (typeof(T).GetInterfaces().Contains(typeof(INotifyPropertyChanged)))
            {                
                _listenPropertyNotifications = ReflectionHelper.GetPropertyChain(orderByFunc).FirstOrDefault();                
            }

            _orderByFunc = orderByFunc.Compile();

            _implementation = new List<T>();
            _collectionImplementation = _implementation;
            _listImplementation = _implementation;
        }

        private readonly SortMethod _sortMethod;
        private readonly IComparer<TOrder> _comparer;
        private readonly Func<T, TOrder> _orderByFunc;
        private readonly string _listenPropertyNotifications;
        private readonly List<TOrder> _orderValues = new List<TOrder>();
        private readonly List<T> _implementation;
        private readonly ICollection _collectionImplementation;
        private readonly IList _listImplementation;

        public IEnumerator<T> GetEnumerator() => _implementation.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_implementation).GetEnumerator();

        void ICollection<T>.Add(T item) => Add(item);

        int IList.Add(object value) => Add((T)value);

        public int Add(T item)
        {
            var orderValue = _orderByFunc(item);
            var insertIndex = SortingHelper.CalculateSortedInsertIndex(_orderValues, orderValue, _sortMethod, _comparer);
            InsertInternal(insertIndex, item, orderValue);
            return insertIndex;
        }

        public void Clear()
        {
            _implementation.Clear();
        }

        bool IList.Contains(object value)
        {
            return _listImplementation.Contains(value);
        }

        int IList.IndexOf(object value)
        {
            return _listImplementation.IndexOf(value);
        }

        void IList.Insert(int index, object value)
        {
            var item = (T) value;
            var c = _orderByFunc(item);
            InsertInternal(index, item, c);
        }

        void IList.Remove(object value)
        {
            Remove((T)value);
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

        public void CopyTo(Array array, int index)
        {
            _collectionImplementation.CopyTo(array, index);
        }

        public int Count => _implementation.Count;
        bool ICollection.IsSynchronized => _collectionImplementation.IsSynchronized;

        object ICollection.SyncRoot => _collectionImplementation.SyncRoot;

        private bool IsReadOnly => _listImplementation.IsReadOnly;
        bool IList.IsReadOnly => _listImplementation.IsReadOnly;
        bool ICollection<T>.IsReadOnly => _listImplementation.IsReadOnly;

        object IList.this[int index]
        {
            get => _listImplementation[index];
            set => this[index] = (T)value;
        }

        public int IndexOf(T item)
        {
            return _implementation.IndexOf(item);
        }

        void IList<T>.Insert(int index, T item)
        {
            var c = _orderByFunc(item);
            InsertInternal(index, item, c);
        }

        public void RemoveAt(int index)
        {            
            RemoveInternal(index, _implementation[index]);
        }

        bool IList.IsFixedSize => _listImplementation.IsFixedSize;

        public T this[int index]
        {
            get => _implementation[index];
            set => ReplaceInternal(index, value);
        }        

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

        private void ReplaceInternal(int index, T newItem)
        {
            var oldItem = _implementation[index];
            if (_listenPropertyNotifications != null)
                if (oldItem is INotifyPropertyChanged notifier)
                    notifier.PropertyChanged -= OnItemPropertyChanged;
            if (_listenPropertyNotifications != null)
                if (newItem is INotifyPropertyChanged notifier)
                    notifier.PropertyChanged += OnItemPropertyChanged;
            var c = _orderByFunc(newItem);
            _orderValues[index] = c;
            _implementation[index] = newItem;            
        }

        private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == _listenPropertyNotifications)
            {
                Remove((T) sender);
                Add((T)sender);
            }
        }
    }
}
