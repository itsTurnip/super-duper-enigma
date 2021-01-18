using System.Collections.Generic;

namespace DispatcherLib
{
    internal class ThreadSafeList<T> : IList<T>
    {
        protected List<T> _internalList = new List<T>();

        // Other Elements of IList implementation

        public IEnumerator<T> GetEnumerator()
        {
            return Clone().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Clone().GetEnumerator();
        }

        protected static object _lock = new object();

        public int Count
        {
            get
            {
                lock (_lock)
                {
                    return ((ICollection<T>)_internalList).Count;
                }
            }
        }

        public bool IsReadOnly => ((ICollection<T>)_internalList).IsReadOnly;

        public T this[int index]
        {
            get
            {
                lock (_lock)
                {
                    return ((IList<T>)_internalList)[index];
                }
            }
            set
            {
                lock (_lock)
                    ((IList<T>)_internalList)[index] = value;
            }
        }

        public List<T> Clone()
        {
            List<T> newList = new List<T>();

            lock (_lock)
            {
                _internalList.ForEach(x => newList.Add(x));
            }

            return newList;
        }

        public int IndexOf(T item)
        {
            lock (_lock)
            {
                return ((IList<T>)_internalList).IndexOf(item);
            }
        }

        public void Insert(int index, T item)
        {
            lock (_lock)
            {
                ((IList<T>)_internalList).Insert(index, item);
            }
        }

        public void RemoveAt(int index)
        {
            lock (_lock)
            {
                ((IList<T>)_internalList).RemoveAt(index);
            }
        }

        public void Add(T item)
        {
            lock (_lock)
            {
                ((ICollection<T>)_internalList).Add(item);
            }
        }

        public void Clear()
        {
            lock (_lock)
            {
                ((ICollection<T>)_internalList).Clear();
            }
        }

        public bool Contains(T item)
        {
            lock (_lock)
            {
                return ((ICollection<T>)_internalList).Contains(item);
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (_lock)
            {
                ((ICollection<T>)_internalList).CopyTo(array, arrayIndex);
            }
        }

        public bool Remove(T item)
        {
            lock (_lock)
            {
                return ((ICollection<T>)_internalList).Remove(item);
            }
        }
    }
}
