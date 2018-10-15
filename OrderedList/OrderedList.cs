using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace gt.Collections.OrderedList
{
    public class OrderedList<T> : ICollection<T>, IEnumerable<T>, IEnumerable, IList<T>, IReadOnlyCollection<T>, IReadOnlyList<T>, ICollection, IList
    {
        private readonly IComparer _comparer;
        private readonly Func<T, object> _orderByFunc;
        private readonly string _listenPropertyNotifications;

        public OrderedList(Expression<Func<T, object>> orderByFunc, IComparer comparer = null)
        {
            _comparer = comparer ?? Comparer.DefaultInvariant;
            if (typeof(T).GetInterfaces().Contains(typeof(INotifyPropertyChanged)))
            {
                Type type = typeof(T);
                _listenPropertyNotifications = ReflectionHelper.GetPropertyChain(orderByFunc).FirstOrDefault();
                /*MemberExpression member = orderByFunc.Body as MemberExpression;
                if (member?.Member is PropertyInfo propInfo)
                {
                    if (type == propInfo.ReflectedType
                        || type.IsSubclassOf(propInfo.ReflectedType))
                    {
                        _listenPropertyNotifications = propInfo.Name;
                    }
                }*/
            }

            _orderByFunc = orderByFunc.Compile();

            _implementation = new List<T>();
            _collectionImplementation = _implementation;
            _listImplementation = _implementation;
        }

        private readonly List<object> _orderValues = new List<object>();
        private readonly List<T> _implementation;
        private readonly ICollection _collectionImplementation;
        private readonly IList _listImplementation;

        public IEnumerator<T> GetEnumerator()
        {
            return _implementation.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_implementation).GetEnumerator();
        }

        public void Add(T item)
        {
            var c = _orderByFunc(item);
            int insertIndex = CalcInsertIndex(c);
            Insert(insertIndex, item);
        }

        int IList.Add(object value)
        {
            T item = (T)value;
            var c = _orderByFunc(item);
            int insertIndex = CalcInsertIndex(c);
            InsertInternal(insertIndex, item, c);
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
            Insert(index, (T)value);
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
            int index = _implementation.IndexOf(item);
            if (index < 0)
            {
                return false;
            }

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

        public void Insert(int index, T item)
        {
            var c = _orderByFunc(item);
            InsertInternal(index, item, c);
        }

        public void RemoveAt(int index)
        {
            _orderValues.RemoveAt(index);
            _implementation.RemoveAt(index);
        }

        bool IList.IsFixedSize => _listImplementation.IsFixedSize;

        public T this[int index]
        {
            get => _implementation[index];
            set => ReplaceInternal(index, value);
        }

        private int CalcInsertIndex(object orderValue)
        {
            if (_orderValues.Count <= 0)
            {
                return 0;
            }

            if (_comparer.Compare(_orderValues.First(), orderValue) > 0)
            {
                return 0;
            }

            if (_comparer.Compare(_orderValues.Last(), orderValue) <= 0)
            {
                return _orderValues.Count;
            }

            int index = _orderValues.FindLastIndex(c => _comparer.Compare(c, orderValue) <= 0);

            if (index < 0)
            {
                return _implementation.Count;
            }

            return index + 1;
        }

        private void InsertInternal(int index, T item, object orderValue)
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
