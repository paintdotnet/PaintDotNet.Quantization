// Based on: https://github.com/dotnet/corefxlab/blob/master/src/Microsoft.Experimental.Collections/Microsoft/Collections/Extensions/DictionarySlim.cs
// Retrieved on 2021-03-08

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace PaintDotNet.Collections
{
    /// <summary>
    /// A lightweight Dictionary with three principal differences compared to <see cref="Dictionary{TKey, TValue}"/>
    ///
    /// 1) It is possible to do "get or add" in a single lookup using <see cref="GetOrAddValueRef(TKey)"/>. For
    /// values that are value types, this also saves a copy of the value.
    /// 2) It assumes it is cheap to equate values.
    /// 3) It assumes the keys implement <see cref="IEquatable{TKey}"/> or else Equals() and they are cheap and sufficient.
    /// </summary>
    /// <remarks>
    /// 1) This avoids having to do separate lookups (<see cref="Dictionary{TKey, TValue}.TryGetValue(TKey, out TValue)"/>
    /// followed by <see cref="Dictionary{TKey, TValue}.Add(TKey, TValue)"/>.
    /// There is not currently an API exposed to get a value by ref without adding if the key is not present.
    /// 2) This means it can save space by not storing hash codes.
    /// 3) This means it can avoid storing a comparer, and avoid the likely virtual call to a comparer.
    /// </remarks>
    [DebuggerTypeProxy(typeof(DictionarySlimDebugView<,>))]
    [DebuggerDisplay("Count = {Count}")]
    public sealed class DictionarySlim<TKey, TValue>
        : IReadOnlyDictionary<TKey, TValue>
          where TKey : IEquatable<TKey>
    {
        private static readonly int[] SizeOneIntArray = new int[1]; // must never be written to

        private static int PowerOf2(int v)
        {
            if ((v & (v - 1)) == 0)
            {
                return v;
            }

            int i = 2;
            while (i < v)
            {
                i <<= 1;
            }

            return i;
        }

        // We want to initialize without allocating arrays. We also want to avoid null checks.
        // Array.Empty would give divide by zero in modulo operation. So we use static one element arrays.
        // The first add will cause a resize replacing these with real arrays of three elements.
        // Arrays are wrapped in a class to avoid being duplicated for each <TKey, TValue>
        private static readonly Entry[] InitialEntries = new Entry[1];
        private int count;
        // 0-based index into _entries of head of free chain: -1 means empty
        private int freeList = -1;
        // 1-based index into _entries; 0 means empty
        private int[] buckets;
        private Entry[] entries;

        [DebuggerDisplay("({key}, {value})->{next}")]
        private struct Entry
        {
            public TKey key;
            public TValue value;
            // 0-based index of next entry in chain: -1 means end of chain
            // also encodes whether this entry _itself_ is part of the free list by changing sign and subtracting 3,
            // so -2 means end of free list, -3 means index 0 but on free list, -4 means index 1 but on free list, etc.
            public int next;
        }

        /// <summary>
        /// Construct with default capacity.
        /// </summary>
        public DictionarySlim()
        {
            this.buckets = SizeOneIntArray;
            this.entries = InitialEntries;
        }

        /// <summary>
        /// Construct with at least the specified capacity for
        /// entries before resizing must occur.
        /// </summary>
        /// <param name="capacity">Requested minimum capacity</param>
        public DictionarySlim(int capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (capacity < 2)
            {
                capacity = 2; // 1 would indicate the dummy array
            }

            capacity = PowerOf2(capacity);
            this.buckets = new int[capacity];
            this.entries = new Entry[capacity];
        }

        public TValue this[TKey key]
        {
            get
            {
                if (!TryGetValue(key, out TValue value))
                {
                    throw new KeyNotFoundException();
                }

                return value;
            }

            set
            {
                GetOrAddValueRef(key) = value;
            }
        }

        /// <summary>
        /// Count of entries in the dictionary.
        /// </summary>
        public int Count => this.count;

        public KeysCollection Keys
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new KeysCollection(this);
        }

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => this.Keys;

        public ValuesCollection Values
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new ValuesCollection(this);
        }

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => this.Values;

        /// <summary>
        /// Clears the dictionary. Note that this invalidates any active enumerators.
        /// </summary>
        public void Clear()
        {
            this.count = 0;
            this.freeList = -1;
            this.buckets = SizeOneIntArray;
            this.entries = InitialEntries;
        }

        /// <summary>
        /// Looks for the specified key in the dictionary.
        /// </summary>
        /// <param name="key">Key to look for</param>
        /// <returns>true if the key is present, otherwise false</returns>
        public bool ContainsKey(TKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            Entry[] entries = this.entries;
            int collisionCount = 0;
            for (int i = this.buckets[key.GetHashCode() & (this.buckets.Length-1)] - 1;
                 unchecked((uint)i < (uint)entries.Length);
                 i = entries[i].next)
            {
                if (key.Equals(entries[i].key))
                {
                    return true;
                }

                if (collisionCount == entries.Length)
                {
                    // The chain of entries forms a loop; which means a concurrent update has happened.
                    // Break out of the loop and throw, rather than looping forever.
                    throw new InvalidOperationException("concurrent operations not supported");
                }

                collisionCount++;
            }

            return false;
        }

        /// <summary>
        /// Gets the value if present for the specified key.
        /// </summary>
        /// <param name="key">Key to look for</param>
        /// <param name="value">Value found, otherwise default(TValue)</param>
        /// <returns>true if the key is present, otherwise false</returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            Entry[] entries = this.entries;
            int collisionCount = 0;
            for (int i = this.buckets[key.GetHashCode() & (this.buckets.Length - 1)] - 1;
                 unchecked((uint)i < (uint)entries.Length);
                 i = entries[i].next)
            {
                if (key.Equals(entries[i].key))
                {
                    value = entries[i].value;
                    return true;
                }

                if (collisionCount == entries.Length)
                {
                    // The chain of entries forms a loop; which means a concurrent update has happened.
                    // Break out of the loop and throw, rather than looping forever.
                    throw new InvalidOperationException("concurrent operations not supported");
                }

                collisionCount++;
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Removes the entry if present with the specified key.
        /// </summary>
        /// <param name="key">Key to look for</param>
        /// <returns>true if the key is present, false if it is not</returns>
        public bool Remove(TKey key)
        {
            return Remove(key, out _);
        }

        public bool Remove(TKey key, out TValue value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            Entry[] entries = this.entries;
            int bucketIndex = key.GetHashCode() & (this.buckets.Length - 1);
            int entryIndex = this.buckets[bucketIndex] - 1;

            int lastIndex = -1;
            int collisionCount = 0;
            while (entryIndex != -1)
            {
                Entry candidate = entries[entryIndex];
                if (candidate.key.Equals(key))
                {
                    if (lastIndex != -1)
                    {   // Fixup preceding element in chain to point to next (if any)
                        entries[lastIndex].next = candidate.next;
                    }
                    else
                    {   // Fixup bucket to new head (if any)
                        this.buckets[bucketIndex] = candidate.next + 1;
                    }

                    value = entries[entryIndex].value;
                    entries[entryIndex] = default;

                    entries[entryIndex].next = -3 - this.freeList; // New head of free list
                    this.freeList = entryIndex;

                    this.count--;
                    return true;
                }

                lastIndex = entryIndex;
                entryIndex = candidate.next;

                if (collisionCount == entries.Length)
                {
                    // The chain of entries forms a loop; which means a concurrent update has happened.
                    // Break out of the loop and throw, rather than looping forever.
                    throw new InvalidOperationException("concurrent operations not supported");
                }

                collisionCount++;
            }

            value = default;
            return false;
        }

        // Not safe for concurrent _reads_ (at least, if either of them add)
        // For concurrent reads, prefer TryGetValue(key, out value)
        /// <summary>
        /// Gets the value for the specified key, or, if the key is not present,
        /// adds an entry and returns the value by ref. This makes it possible to
        /// add or update a value in a single look up operation.
        /// </summary>
        /// <param name="key">Key to look for</param>
        /// <returns>Reference to the new or existing value</returns>
        public ref TValue GetOrAddValueRef(TKey key)
        {
            return ref GetOrAddValueRef(key, default);
        }

        public ref TValue GetOrAddValueRef(TKey key, TValue defaultValue)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            Entry[] entries = this.entries;
            int collisionCount = 0;
            int bucketIndex = key.GetHashCode() & (this.buckets.Length - 1);
            for (int i = this.buckets[bucketIndex] - 1;
                unchecked((uint)i < (uint)entries.Length);
                i = entries[i].next)
            {
                if (key.Equals(entries[i].key))
                {
                    return ref entries[i].value;
                }

                if (collisionCount == entries.Length)
                {
                    // The chain of entries forms a loop; which means a concurrent update has happened.
                    // Break out of the loop and throw, rather than looping forever.
                    throw new InvalidOperationException("concurrent operations not supported");
                }

                collisionCount++;
            }

            return ref AddKey(key, bucketIndex, defaultValue);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private ref TValue AddKey(TKey key, int bucketIndex, TValue defaultValue)
        {
            Entry[] entries = this.entries;
            int entryIndex;
            if (this.freeList != -1)
            {
                entryIndex = this.freeList;
                this.freeList = -3 - entries[this.freeList].next;
            }
            else
            {
                if (this.count == entries.Length || entries.Length == 1)
                {
                    entries = Resize();
                    bucketIndex = key.GetHashCode() & (this.buckets.Length - 1);
                    // entry indexes were not changed by Resize
                }

                entryIndex = this.count;
            }

            entries[entryIndex].key = key;
            entries[entryIndex].next = this.buckets[bucketIndex] - 1;
            this.buckets[bucketIndex] = entryIndex + 1;
            this.count++;
            ref TValue result = ref entries[entryIndex].value;
            result = defaultValue;
            return ref result;
        }

        private Entry[] Resize()
        {
            Debug.Assert(this.entries.Length == this.count || this.entries.Length == 1); // We only copy _count, so if it's longer we will miss some
            int count = this.count;
            int newSize = this.entries.Length * 2;
            if (unchecked((uint)newSize > (uint)int.MaxValue)) // uint cast handles overflow
            {
                throw new InvalidOperationException("capacity overflow");
            }

            Entry[] entries = new Entry[newSize];
            Array.Copy(this.entries, 0, entries, 0, count);

            int[] newBuckets = new int[entries.Length];
            while (count-- > 0)
            {
                int bucketIndex = entries[count].key.GetHashCode() & (newBuckets.Length - 1);
                entries[count].next = newBuckets[bucketIndex] - 1;
                newBuckets[bucketIndex] = count + 1;
            }

            this.buckets = newBuckets;
            this.entries = entries;

            return entries;
        }

        /// <summary>
        /// Gets an enumerator over the dictionary
        /// </summary>
        public Enumerator GetEnumerator() => new Enumerator(this); // avoid boxing

        /// <summary>
        /// Gets an enumerator over the dictionary
        /// </summary>
        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() =>
            new Enumerator(this);

        /// <summary>
        /// Gets an enumerator over the dictionary
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);

        /// <summary>
        /// Enumerator
        /// </summary>
        public struct Enumerator
            : IEnumerator<KeyValuePair<TKey, TValue>>
        {
            private readonly DictionarySlim<TKey, TValue> dictionary;
            private int index;
            private int count;
            private KeyValuePair<TKey, TValue> current;

            internal Enumerator(DictionarySlim<TKey, TValue> dictionary)
            {
                this.dictionary = dictionary;
                this.index = 0;
                this.count = this.dictionary.count;
                this.current = default;
            }

            /// <summary>
            /// Move to next
            /// </summary>
            public bool MoveNext()
            {
                if (this.count == 0)
                {
                    this.current = default;
                    return false;
                }

                this.count--;

                while (this.dictionary.entries[this.index].next < -1)
                {
                    this.index++;
                }

                this.current = new KeyValuePair<TKey, TValue>(
                    this.dictionary.entries[this.index].key,
                    this.dictionary.entries[this.index++].value);

                return true;
            }

            /// <summary>
            /// Get current value
            /// </summary>
            public KeyValuePair<TKey, TValue> Current => this.current;

            object IEnumerator.Current => this.current;

            void IEnumerator.Reset()
            {
                this.index = 0;
                this.count = this.dictionary.count;
                this.current = default;
            }

            /// <summary>
            /// Dispose the enumerator
            /// </summary>
            public void Dispose()
            {
            }
        }

        public readonly struct KeysCollection
            : IEnumerable<TKey>
        {
            private readonly DictionarySlim<TKey, TValue> dictionary;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal KeysCollection(DictionarySlim<TKey, TValue> dictionary)
            {
                this.dictionary = dictionary;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Enumerator GetEnumerator()
            {
                return new Enumerator(this.dictionary.GetEnumerator());
            }

            IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator()
            {
                return GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public struct Enumerator
                : IEnumerator<TKey>
            {
                private DictionarySlim<TKey, TValue>.Enumerator source;

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                internal Enumerator(DictionarySlim<TKey, TValue>.Enumerator source)
                {
                    this.source = source;
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public void Dispose()
                {
                    this.source.Dispose();
                }

                public TKey Current
                {
                    [MethodImpl(MethodImplOptions.AggressiveInlining)]
                    get => this.source.Current.Key;
                }

                object IEnumerator.Current => this.Current;

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public bool MoveNext()
                {
                    return this.source.MoveNext();
                }

                void IEnumerator.Reset()
                {
                    throw new NotSupportedException();
                }
            }
        }

        public readonly struct ValuesCollection
            : IEnumerable<TValue>
        {
            private readonly DictionarySlim<TKey, TValue> dictionary;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal ValuesCollection(DictionarySlim<TKey, TValue> dictionary)
            {
                this.dictionary = dictionary;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Enumerator GetEnumerator()
            {
                return new Enumerator(this.dictionary.GetEnumerator());
            }

            IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
            {
                return GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public struct Enumerator
                : IEnumerator<TValue>
            {
                private DictionarySlim<TKey, TValue>.Enumerator source;

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                internal Enumerator(DictionarySlim<TKey, TValue>.Enumerator source)
                {
                    this.source = source;
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public void Dispose()
                {
                    this.source.Dispose();
                }

                public TValue Current
                {
                    [MethodImpl(MethodImplOptions.AggressiveInlining)]
                    get => this.source.Current.Value;
                }

                object IEnumerator.Current => this.Current;

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public bool MoveNext()
                {
                    return this.source.MoveNext();
                }

                void IEnumerator.Reset()
                {
                    throw new NotSupportedException();
                }
            }
        }
    }

    internal sealed class DictionarySlimDebugView<K, V> where K : IEquatable<K>
    {
        private readonly DictionarySlim<K, V> dictionary;

        public DictionarySlimDebugView(DictionarySlim<K, V> dictionary)
        {
            this.dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public KeyValuePair<K, V>[] Items
        {
            get
            {
                return this.dictionary.ToArray();
            }
        }
    }
}
