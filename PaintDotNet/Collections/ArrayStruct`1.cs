/////////////////////////////////////////////////////////////////////////////////
// paint.net                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, and contributors.                  //
// All Rights Reserved.                                                        //
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PaintDotNet.Collections
{
    // Wraps an array so we can get inlining when using generics
    public struct ArrayStruct<T>
        : IList<T>,
          IReadOnlyList<T>
    {
        private T[] array;

        public ArrayStruct(T[] array)
        {
            this.array = array;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf(T item)
        {
            return Array.IndexOf<T>(this.array, item);
        }

        public void Insert(int index, T item)
        {
            throw new NotSupportedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        public T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return this.array[index];
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                this.array[index] = value;
            }
        }

        public void Add(T item)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            throw new NotSupportedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(T item)
        {
            return IndexOf(item) != -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(T[] array, int arrayIndex)
        {
            this.array.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get 
            { 
                return this.array.Length;
            }
        }

        public bool IsReadOnly
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get 
            { 
                return false;
            }
        }

        public bool Remove(T item)
        {
            throw new NotSupportedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Enumerator GetEnumerator()
        {
            return new Enumerator(this.array);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public struct Enumerator
            : IEnumerator<T>
        {
            private T[] array;
            private int index;

            public Enumerator(T[] array)
            {
                this.array = array;
                this.index = -1;
            }

            public T Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get 
                { 
                    return this.array[this.index];
                }
            }

            public void Dispose()
            {
                this.array = null;
                this.index = -1;
            }

            object System.Collections.IEnumerator.Current
            {
                get 
                { 
                    return this.Current;
                }
            }

            public bool MoveNext()
            {
                if (this.index == this.array.Length)
                {
                    return false;
                }

                ++this.index;

                if (this.index == this.array.Length)
                {
                    return false;
                }

                return true;
            }

            public void Reset()
            {
                throw new NotSupportedException();
            }
        }
    }
}
