using System;
using System.Collections.Generic;
using System.Linq;
// using System.Linq; do not use LINQ
using System.Text;

namespace UniRx
{
    // copy, modified from Rx Official

    public sealed class CompositeDisposable : ICollection<IDisposable>, IDisposable, ICancelable
    {
        private readonly object _gate = new object();

        private bool _disposed;
        private List<DisposableInfo> _disposables;
        private int _count;
        private const int SHRINK_THRESHOLD = 64;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Reactive.Disposables.CompositeDisposable"/> class with no disposables contained by it initially.
        /// </summary>
        public CompositeDisposable()
        {
            _disposables = new List<DisposableInfo>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Reactive.Disposables.CompositeDisposable"/> class with the specified number of disposables.
        /// </summary>
        /// <param name="capacity">The number of disposables that the new CompositeDisposable can initially store.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than zero.</exception>
        public CompositeDisposable(int capacity)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException("capacity");

            _disposables = new List<DisposableInfo>(capacity);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Reactive.Disposables.CompositeDisposable"/> class from a group of disposables.
        /// </summary>
        /// <param name="disposables">Disposables that will be disposed together.</param>
        /// <exception cref="ArgumentNullException"><paramref name="disposables"/> is null.</exception>
        public CompositeDisposable(params IDisposable[] disposables)
        {
            if (disposables == null)
                throw new ArgumentNullException("disposables");

            _disposables = new List<DisposableInfo>();
            _count = _disposables.Count;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Reactive.Disposables.CompositeDisposable"/> class from a group of disposables.
        /// </summary>
        /// <param name="disposables">Disposables that will be disposed together.</param>
        /// <exception cref="ArgumentNullException"><paramref name="disposables"/> is null.</exception>
        public CompositeDisposable(IEnumerable<IDisposable> disposables)
        {
            if (disposables == null)
                throw new ArgumentNullException("disposables");

            _disposables = new List<DisposableInfo>(disposables.Select(d => new DisposableInfo(d, 0)));
            _count = _disposables.Count;
        }

        /// <summary>
        /// Gets the number of disposables contained in the CompositeDisposable.
        /// </summary>
        public int Count
        {
            get
            {
                return _count;
            }
        }

        /// <summary>
        /// Adds a disposable to the CompositeDisposable or disposes the disposable if the CompositeDisposable is disposed.
        /// </summary>
        /// <param name="item">Disposable to add.</param>
        /// <exception cref="ArgumentNullException"><paramref name="item"/> is null.</exception>
        public void Add(IDisposable item)
        {
           Add(item, 0);
        }

        public void Add(IDisposable item, int priority)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            
            var shouldDispose = false;
            lock (_gate)
            {
                shouldDispose = _disposed;
                if (!_disposed)
                {
                    _disposables.Add(new DisposableInfo(item, priority));
                    _count++;
                }
            }
            if (shouldDispose)
                item.Dispose();
        }
        
        private struct DisposableInfo : IEquatable<DisposableInfo>
        {
            public IDisposable Disposable;
            public int Priority;

            public DisposableInfo(IDisposable disposable, int priority)
            {
                Disposable = disposable;
                Priority = priority;
            }

            public bool Equals(DisposableInfo other)
            {
                return Equals(Disposable, other.Disposable) && Priority == other.Priority;
            }

            public override bool Equals(object obj)
            {
                return obj is DisposableInfo other && Equals(other);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((Disposable != null ? Disposable.GetHashCode() : 0) * 397) ^ Priority;
                }
            }

            public static bool operator ==(DisposableInfo left, DisposableInfo right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(DisposableInfo left, DisposableInfo right)
            {
                return !left.Equals(right);
            }
        }

        /// <summary>
        /// Removes and disposes the first occurrence of a disposable from the CompositeDisposable.
        /// </summary>
        /// <param name="item">Disposable to remove.</param>
        /// <returns>true if found; false otherwise.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="item"/> is null.</exception>
        public bool Remove(IDisposable item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            var shouldDispose = false;

            lock (_gate)
            {
                if (!_disposed)
                {
                    //
                    // List<T> doesn't shrink the size of the underlying array but does collapse the array
                    // by copying the tail one position to the left of the removal index. We don't need
                    // index-based lookup but only ordering for sequential disposal. So, instead of spending
                    // cycles on the Array.Copy imposed by Remove, we use a null sentinel value. We also
                    // do manual Swiss cheese detection to shrink the list if there's a lot of holes in it.
                    //
                    var i = _disposables.FindIndex(d => d.Disposable == item);
                    if (i >= 0)
                    {
                        shouldDispose = true;
                        _disposables[i] = default;
                        _count--;

                        if (_disposables.Capacity > SHRINK_THRESHOLD && _count < _disposables.Capacity / 2)
                        {
                            var old = _disposables;
                            _disposables = new List<DisposableInfo>(_disposables.Capacity / 2);

                            foreach (var d in old)
                                if (d.Disposable != null)
                                    _disposables.Add(d);
                        }
                    }
                }
            }

            if (shouldDispose)
                item.Dispose();

            return shouldDispose;
        }

        /// <summary>
        /// Disposes all disposables in the group and removes them from the group.
        /// </summary>
        public void Dispose()
        {
            var currentDisposables = default(DisposableInfo[]);
            lock (_gate)
            {
                if (!_disposed)
                {
                    _disposed = true;
                    currentDisposables = _disposables.ToArray();
                    _disposables.Clear();
                    _count = 0;
                }
            }
            
            if (currentDisposables != default)
            {
                currentDisposables = currentDisposables.OrderBy(x => x.Priority).Reverse().ToArray();
                foreach (var d in currentDisposables)
                    if (d.Disposable != null)
                        d.Disposable.Dispose();
            }
        }

        /// <summary>
        /// Removes and disposes all disposables from the CompositeDisposable, but does not dispose the CompositeDisposable.
        /// </summary>
        public void Clear()
        {
            var currentDisposables = default(DisposableInfo[]);
            lock (_gate)
            {
                currentDisposables = _disposables.ToArray();
                _disposables.Clear();
                _count = 0;
            }
            
            currentDisposables = currentDisposables.OrderBy(x => x.Priority).Reverse().ToArray();
            foreach (var d in currentDisposables)
                if (d.Disposable != null)
                    d.Disposable.Dispose();
        }

        /// <summary>
        /// Determines whether the CompositeDisposable contains a specific disposable.
        /// </summary>
        /// <param name="item">Disposable to search for.</param>
        /// <returns>true if the disposable was found; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="item"/> is null.</exception>
        public bool Contains(IDisposable item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            lock (_gate)
            {
                return _disposables.Any(d => d.Disposable == item);
            }
        }

        /// <summary>
        /// Copies the disposables contained in the CompositeDisposable to an array, starting at a particular array index.
        /// </summary>
        /// <param name="array">Array to copy the contained disposables to.</param>
        /// <param name="arrayIndex">Target index at which to copy the first disposable of the group.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than zero. -or - <paramref name="arrayIndex"/> is larger than or equal to the array length.</exception>
        public void CopyTo(IDisposable[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            if (arrayIndex < 0 || arrayIndex >= array.Length)
                throw new ArgumentOutOfRangeException("arrayIndex");

            lock (_gate)
            {
                var disArray = new List<DisposableInfo>();
                foreach (var item in _disposables)
                {
                    if (item != null) disArray.Add(item);
                }

                Array.Copy(disArray.ToArray(), 0, array, arrayIndex, array.Length - arrayIndex);
            }
        }

        /// <summary>
        /// Always returns false.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the CompositeDisposable.
        /// </summary>
        /// <returns>An enumerator to iterate over the disposables.</returns>
        public IEnumerator<IDisposable> GetEnumerator()
        {
            var res = new List<IDisposable>();

            lock (_gate)
            {
                foreach (var d in _disposables)
                {
                    if (d != null) res.Add(d.Disposable);
                }
            }

            return res.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the CompositeDisposable.
        /// </summary>
        /// <returns>An enumerator to iterate over the disposables.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Gets a value that indicates whether the object is disposed.
        /// </summary>
        public bool IsDisposed
        {
            get { return _disposed; }
        }
    }
}