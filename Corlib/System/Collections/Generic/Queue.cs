using System.Drawing;

namespace System.Collections.Generic
{
    public class Queue<T>
    {
        T[] _items;
        int _size;
        int _capacity;

        public Queue(int capacity = 1)
        {
            _items = new T[capacity];
            _size = 0;
            _capacity = capacity;
        }

        public T Tail 
        {
            get 
            {
               return _items[_size];
            }
        }

        public T Head
        {
            get
            {
                return _items[0];
            }
        }

        public void Enqueue(T item)
        {
            if (_capacity <= _size)
            {
                var _new = new T[_size + 1];
                Array.Copy(_items, ref _new, 0);
                _items = _new;
                _capacity++;
            }

            _items[_size++] = item;
        }

        public T Dequeue()
        {
            var res = _items[0];
            for (int i = 1; i < _size; i++)
            {
                _items[i - 1] = _items[i];
            }
            _size--;
            return res;
        }

        public T Peek()
        {
            return Head;
        }

        public int Count
        {
            get
            {
                return _size;
            }
        }

        public void Clear()
        {
            _items.Dispose();
            _items = new T[1];
            _size = 0;
            _capacity = 1;
        }

        public override void Dispose()
        {
            _items.Dispose();
            base.Dispose();
        }
    }
}