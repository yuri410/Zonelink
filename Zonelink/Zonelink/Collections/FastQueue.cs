/*
-----------------------------------------------------------------------------
This source file is part of Apoc3D Engine

Copyright (c) 2009+ Tao Games

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  if not, write to the Free Software Foundation, 
Inc., 59 Temple Place - Suite 330, Boston, MA 02111-1307, USA, or go to
http://www.gnu.org/copyleft/gpl.txt.

-----------------------------------------------------------------------------
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Apoc3D.Core;

namespace Apoc3D.Collections
{
    internal sealed class System_QueueDebugView<T>
    {
        // Fields
        private FastQueue<T> queue;

        // Methods
        public System_QueueDebugView(FastQueue<T> queue)
        {
            if (queue == null)
            {
                throw new ArgumentNullException("queue");
            }
            this.queue = queue;
        }

        // Properties
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items
        {
            get
            {
                return this.queue.ToArray();
            }
        }
    }

    [Serializable, DebuggerTypeProxy(typeof(System_QueueDebugView<>)), DebuggerDisplay("Count = {Count}")]
    public class FastQueue<T> : IEnumerable<T>, IEnumerable
    {
        [Serializable, StructLayout(LayoutKind.Sequential)]
        public struct Enumerator<S> : IEnumerator<S>, IDisposable, IEnumerator
        {
            private FastQueue<S> q;
            private int index;
            private int version;
            private S currentElement;
            internal Enumerator(FastQueue<S> q)
            {
                this.q = q;
                this.version = this.q.version;
                this.index = -1;
                this.currentElement = default(S);
            }

            public void Dispose()
            {
                this.index = -2;
                this.currentElement = default(S);
            }

            public bool MoveNext()
            {
                if (this.version != this.q.version)
                {
                    throw new InvalidOperationException("InvalidOperation_EnumFailedVersion");
                }
                if (this.index == -2)
                {
                    return false;
                }
                this.index++;
                if (this.index == this.q.size)
                {
                    this.index = -2;
                    this.currentElement = default(S);
                    return false;
                }
                this.currentElement = this.q.GetElement(this.index);
                return true;
            }

            public S Current
            {
                get
                {
                    if (this.index < 0)
                    {
                        if (this.index == -1)
                        {
                            throw new InvalidOperationException("InvalidOperation_EnumNotStarted");
                        }
                        else
                        {
                            throw new InvalidOperationException("InvalidOperation_EnumEnded");
                        }
                    }
                    return this.currentElement;
                }
            }
            object IEnumerator.Current
            {
                get
                {
                    if (this.index < 0)
                    {
                        if (this.index == -1)
                        {
                            throw new InvalidOperationException("InvalidOperation_EnumNotStarted");
                        }
                        else
                        {
                            throw new InvalidOperationException("InvalidOperation_EnumEnded");
                        }
                    }
                    return this.currentElement;
                }
            }
            void IEnumerator.Reset()
            {
                if (this.version != this.q.version)
                {
                    throw new InvalidOperationException("InvalidOperation_EnumFailedVersion");
                }
                this.index = -1;
                this.currentElement = default(S);
            }
        }

        private T[] array;
        private const int DefaultCapacity = 4;
        private static T[] emptyArray;
        private const int GrowFactor = 200;
        private int head;
        private const int MinimumGrow = 4;
        private const int ShrinkThreshold = 32;
        private int size;

        private int tail;
        private int version;

        static FastQueue()
        {
            FastQueue<T>.emptyArray = new T[0];
        }

        public FastQueue()
        {
            this.array = FastQueue<T>.emptyArray;
        }

        public FastQueue(IEnumerable<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }
            this.array = new T[4];
            this.size = 0;
            this.version = 0;
            using (IEnumerator<T> enumerator = collection.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    this.Enqueue(enumerator.Current);
                }
            }
        }

        public FastQueue(int capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException("capacity");
            }
            this.array = new T[capacity];
            this.head = 0x0;
            this.tail = 0x0;
            this.size = 0x0;
        }

        public void Clear()
        {
            if (this.head < this.tail)
            {
                Array.Clear(this.array, this.head, this.size);
            }
            else
            {
                Array.Clear(this.array, this.head, this.array.Length - this.head);
                Array.Clear(this.array, 0, this.tail);
            }
            this.head = 0;
            this.tail = 0;
            this.size = 0;
            this.version++;
        }

        public bool Contains(T item)
        {
            int index = this.head;
            int num2 = this.size;
            EqualityComparer<T> comparer = EqualityComparer<T>.Default;
            while (num2-- > 0)
            {
                if (item == null)
                {
                    if (this.array[index] == null)
                    {
                        return true;
                    }
                }
                else if ((this.array[index] != null) && comparer.Equals(this.array[index], item))
                {
                    return true;
                }
                index = (index + 1) % this.array.Length;
            }
            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            if ((arrayIndex < 0) || (arrayIndex > array.Length))
            {
                throw new ArgumentOutOfRangeException("arrayIndex");
            }
            int length = array.Length;
            if ((length - arrayIndex) < this.size)
            {
                throw new ArgumentException("Argument_InvalidOffLen");
            }
            int num2 = ((length - arrayIndex) < this.size) ? (length - arrayIndex) : this.size;
            if (num2 != 0)
            {
                int num3 = ((this.array.Length - this.head) < num2) ? (this.array.Length - this.head) : num2;
                Array.Copy(this.array, this.head, array, arrayIndex, num3);
                num2 -= num3;
                if (num2 > 0)
                {
                    Array.Copy(this.array, 0, array, (arrayIndex + this.array.Length) - this.head, num2);
                }
            }
        }

        public T Dequeue()
        {
            if (this.size == 0)
            {
                throw new InvalidOperationException("EmptyQueue");
            }
            T local = this.array[this.head];
            this.array[this.head] = default(T);
            this.head = (this.head + 1) % this.array.Length;
            this.size--;
            this.version++;
            return local;
        }

        public void Enqueue(T item)
        {
            if (this.size == this.array.Length)
            {
                int capacity = (int)((this.array.Length * 200L) / 100L);
                if (capacity < (this.array.Length + 4))
                {
                    capacity = this.array.Length + 4;
                }
                this.SetCapacity(capacity);
            }
            this.array[this.tail] = item;
            this.tail = (this.tail + 1) % this.array.Length;
            this.size++;
            this.version++;
        }

        public T GetElement(int i)
        {
            return this.array[(this.head + i) % this.array.Length];
        }

        public Enumerator<T> GetEnumerator()
        {
            return new Enumerator<T>((FastQueue<T>)this);
        }

        public T Head()
        {
            return this.array[this.head];
        }
        public T Tail()
        {
            return GetElement(this.size - 1);
        }

        private void SetCapacity(int capacity)
        {
            T[] destinationArray = new T[capacity];
            if (this.size > 0)
            {
                if (this.head < this.tail)
                {
                    Array.Copy(this.array, this.head, destinationArray, 0, this.size);
                }
                else
                {
                    Array.Copy(this.array, this.head, destinationArray, 0, this.array.Length - this.head);
                    Array.Copy(this.array, 0, destinationArray, this.array.Length - this.head, this.tail);
                }
            }
            this.array = destinationArray;
            this.head = 0;
            this.tail = (this.size == capacity) ? 0 : this.size;
            this.version++;
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return new Enumerator<T>(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator<T>(this);
        }

        public T[] ToArray()
        {
            T[] destinationArray = new T[this.size];
            if (this.size != 0)
            {
                if (this.head < this.tail)
                {
                    Array.Copy(this.array, this.head, destinationArray, 0, this.size);
                    return destinationArray;
                }
                Array.Copy(this.array, this.head, destinationArray, 0, this.array.Length - this.head);
                Array.Copy(this.array, 0, destinationArray, this.array.Length - this.head, this.tail);
            }
            return destinationArray;
        }

        public void TrimExcess()
        {
            int num = (int)(this.array.Length * 0.9);
            if (this.size < num)
            {
                this.SetCapacity(this.size);
            }
        }

        // Properties
        public int Count
        {
            get
            {
                return this.size;
            }
        }

    }


}
