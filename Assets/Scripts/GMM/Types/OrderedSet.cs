using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GMM
{
    namespace Types
    {
        /// <summary>
        ///   This is not a regular set, but instead one that also keeps track
        ///     of the order the elements where inserted in.
        /// </summary>
        /// <remarks>
        ///   Methods are not documented here. Consider them being analogous to the ones in both Set and List classes.
        /// </remarks>
        /// <typeparam name="T">Arbitrary type of your choice. You may want to pay attention to its <c>GetHashCode()</c> method.</typeparam>
        public class OrderedSet<T> : ICollection<T>
        {
            private readonly IDictionary<T, LinkedListNode<T>> m_Dictionary;
            private readonly LinkedList<T> m_LinkedList;

            public OrderedSet() : this(EqualityComparer<T>.Default)
            {
            }

            public OrderedSet(IEqualityComparer<T> comparer)
            {
                m_Dictionary = new Dictionary<T, LinkedListNode<T>>(comparer);
                m_LinkedList = new LinkedList<T>();
            }

            public int Count
            {
                get { return m_Dictionary.Count; }
            }

            public virtual bool IsReadOnly
            {
                get { return m_Dictionary.IsReadOnly; }
            }

            void ICollection<T>.Add(T item)
            {
                Add(item);
            }

            public void Clear()
            {
                m_LinkedList.Clear();
                m_Dictionary.Clear();
            }

            public bool Remove(T item)
            {
                LinkedListNode<T> node;
                bool found = m_Dictionary.TryGetValue(item, out node);
                if (!found) return false;
                m_Dictionary.Remove(item);
                m_LinkedList.Remove(node);
                return true;
            }

            public IEnumerator<T> GetEnumerator()
            {
                return m_LinkedList.GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public bool Contains(T item)
            {
                return m_Dictionary.ContainsKey(item);
            }

            public void CopyTo(T[] array, int arrayIndex)
            {
                m_LinkedList.CopyTo(array, arrayIndex);
            }

            public bool Add(T item)
            {
                if (m_Dictionary.ContainsKey(item)) return false;
                LinkedListNode<T> node = m_LinkedList.AddLast(item);
                m_Dictionary.Add(item, node);
                return true;
            }

            public T Shift()
            {
                T element = First;
                m_LinkedList.RemoveFirst();
                m_Dictionary.Remove(element);
                return element;
            }

            public T Pop()
            {
                T element = Last;
                m_LinkedList.RemoveLast();
                m_Dictionary.Remove(element);
                return element;
            }

            public T Last
            {
                get
                {
                    return m_LinkedList.Last.Value;
                }
            }

            public T First
            {
                get
                {
                    return m_LinkedList.First.Value;
                }
            }
        }
    }
}
