using System.Diagnostics;

namespace System.Collections.Generic
{
    public class List<T>
    {
        private T[] _items;
        private int _size;
        private int _capacity;

        public List(int capacity = 1)
        {
            _items = new T[capacity];
            _size = 0;
            _capacity = capacity;
        }

        public List(T[] t)
        {
            _items = t;
            _size = t.Length;
            _capacity = _size;
        }

        public int Count
        {
            get
            {
                return _size;
            }
        }

        public T this[int index]
        {
            get
            {
                return _items[index];
            }
            set
            {
                _items[index] = value;
            }
        }

        public void Add(T t)
        {
            if (_capacity <= _size)
            {
                var _new = new T[_size + 1];
                Array.Copy(_items, ref _new, 0);
                _items = _new;
                _capacity++;
            }

            _items[_size++] = t;
        }

        public void Insert(int index, T item, bool internalMove = false)
        {
            //Broken
            //if (index == IndexOf(item)) return;

            if (!internalMove)
                _size++;

            if (internalMove)
            {
                int _index = IndexOf(item);
                for (int i = _index; i < Count - 1; i++)
                {
                    _items[i] = _items[i + 1];
                }
            }

            for (int i = Count - 1; i > index; i--)
            {
                _items[i] = _items[i - 1];
            }
            _items[index] = item;
        }

        public T[] ToArray()
        {
            T[] array = new T[Count];
            for (int i = 0; i < Count; i++)
            {
                array[i] = this[i];
            }
            return array;
        }

        public int IndexOf(T item)
        {
            for (int i = 0; i < Count; i++)
            {
                T first = this[i];
                T second = item;

                if (this[i].Equals( item))
                    return i;
            }

            return -1;
        }
        public bool Remove(T item)
        {
            int at = IndexOf(item);

            if (at < 0)
                return false;

            RemoveAt(at);

            return true;
        }

        public void RemoveAt(int index)
        {
            Console.WriteLine($"RemoveAt: {index}");
            _size--;

            for (int i = index; i < Count; i++)
            {
                _items[i] = _items[i + 1];
            }

            _items[Count] = default(T);
        }

        public override void Dispose()
        {
            _items.Dispose();
            base.Dispose();
        }

        public void Clear()
        {
            _size = 0;
        }
    }
}