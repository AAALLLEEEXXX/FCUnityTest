using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace UniRx
{
	public struct DictionaryAddEvent<TKey, TValue> : IEquatable<DictionaryAddEvent<TKey, TValue>>
	{
		public TKey Key { get; }
		public TValue Value { get; }

		public DictionaryAddEvent(TKey key, TValue value)
			: this()
		{
			Key = key;
			Value = value;
		}

		public override string ToString()
		{
			return string.Format("Key:{0} Value:{1}", Key, Value);
		}

		public override int GetHashCode()
		{
			return EqualityComparer<TKey>.Default.GetHashCode(Key) ^ (EqualityComparer<TValue>.Default.GetHashCode(Value) << 2);
		}

		public bool Equals(DictionaryAddEvent<TKey, TValue> other)
		{
			return EqualityComparer<TKey>.Default.Equals(Key, other.Key) && EqualityComparer<TValue>.Default.Equals(Value, other.Value);
		}
	}

	public struct DictionaryRemoveEvent<TKey, TValue> : IEquatable<DictionaryRemoveEvent<TKey, TValue>>
	{
		public TKey Key { get; }
		public TValue Value { get; }

		public DictionaryRemoveEvent(TKey key, TValue value)
			: this()
		{
			Key = key;
			Value = value;
		}

		public override string ToString()
		{
			return string.Format("Key:{0} Value:{1}", Key, Value);
		}

		public override int GetHashCode()
		{
			return EqualityComparer<TKey>.Default.GetHashCode(Key) ^ (EqualityComparer<TValue>.Default.GetHashCode(Value) << 2);
		}

		public bool Equals(DictionaryRemoveEvent<TKey, TValue> other)
		{
			return EqualityComparer<TKey>.Default.Equals(Key, other.Key) && EqualityComparer<TValue>.Default.Equals(Value, other.Value);
		}
	}

	public struct DictionaryReplaceEvent<TKey, TValue> : IEquatable<DictionaryReplaceEvent<TKey, TValue>>
	{
		public TKey Key { get; }
		public TValue OldValue { get; }
		public TValue NewValue { get; }

		public DictionaryReplaceEvent(TKey key, TValue oldValue, TValue newValue)
			: this()
		{
			Key = key;
			OldValue = oldValue;
			NewValue = newValue;
		}

		public override string ToString()
		{
			return string.Format("Key:{0} OldValue:{1} NewValue:{2}", Key, OldValue, NewValue);
		}

		public override int GetHashCode()
		{
			return EqualityComparer<TKey>.Default.GetHashCode(Key) ^ (EqualityComparer<TValue>.Default.GetHashCode(OldValue) << 2) ^ (EqualityComparer<TValue>.Default.GetHashCode(NewValue) >> 2);
		}

		public bool Equals(DictionaryReplaceEvent<TKey, TValue> other)
		{
			return EqualityComparer<TKey>.Default.Equals(Key, other.Key) && EqualityComparer<TValue>.Default.Equals(OldValue, other.OldValue) && EqualityComparer<TValue>.Default.Equals(NewValue, other.NewValue);
		}
	}

	// IReadOnlyDictionary is from .NET 4.5
	public interface IReadOnlyReactiveDictionary<TKey, TValue> : IReadonlyReinitializableCollection,
#if (NET_4_6 || NET_STANDARD_2_0)
		IReadOnlyDictionary<TKey, TValue>
#else
		IEnumerable<KeyValuePair<TKey, TValue>>
#endif
	{
#if !(NET_4_6 || NET_STANDARD_2_0)
        int Count { get; }
        TValue this[TKey index] { get; }
        bool ContainsKey(TKey key);
        bool TryGetValue(TKey key, out TValue value);
#endif
		bool IsDisposed { get; }

		IObservable<DictionaryAddEvent<TKey, TValue>> ObserveAdd();
		IObservable<int> ObserveCountChanged(bool notifyCurrentCount = false);
		IObservable<DictionaryRemoveEvent<TKey, TValue>> ObserveRemove(bool observePerElementClear = false);
		IObservable<DictionaryReplaceEvent<TKey, TValue>> ObserveReplace();
		IObservable<Unit> ObserveReset();
		IObservable<Unit> ObserveAnyChange();
	}

	public interface IReactiveDictionary<TKey, TValue> : IReadOnlyReactiveDictionary<TKey, TValue>, IDictionary<TKey, TValue>, IReinitializableCollection
	{
	}

	[Serializable]
	public partial class ReactiveDictionary<TKey, TValue> : IReactiveDictionary<TKey, TValue>, IReinitializedLockedCollection, IDictionary<TKey, TValue>, IEnumerable, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IDictionary, IDisposable
#if !UNITY_METRO
	  , ISerializable, IDeserializationCallback
#endif
	{
		[NonSerialized]
		private bool isDisposed = false;

		private bool observePerElementClear;

#if !UniRxLibrary
		[UnityEngine.SerializeField]
#endif
		private readonly Dictionary<TKey, TValue> inner;

		public IEqualityComparer<TKey> Comparer => inner.Comparer;

		int IReinitializedLockedCollection.ReinitializeLockCount { get; set; }

		public ReactiveDictionary(bool observePerElementClear = false)
		{
			inner = new Dictionary<TKey, TValue>();
			this.observePerElementClear = observePerElementClear;
		}

		public ReactiveDictionary(IEqualityComparer<TKey> comparer, bool observePerElementClear = false)
		{
			inner = new Dictionary<TKey, TValue>(comparer);
			this.observePerElementClear = observePerElementClear;
		}

		public ReactiveDictionary(Dictionary<TKey, TValue> innerDictionary, bool observePerElementClear = false)
		{
			inner = innerDictionary;
			this.observePerElementClear = observePerElementClear;
		}

		public ReactiveCollectionReinitializeLock Reinitialize(bool disposeValues = true)
		{
			return ReactiveCollectionReinitializeLock.Reinitialize(this, disposeValues);
		}

		public ReactiveCollectionReinitializeLock SuppressEvents()
		{
			return ReactiveCollectionReinitializeLock.SuppressEvents(this);
		}

		public TValue this[TKey key]
		{
			get => inner[key];

			set
			{
				TValue oldValue;

				if (TryGetValue(key, out oldValue))
				{
					inner[key] = value;

					if (!ReactiveCollectionReinitializeLock.IsLocked(this))
					{
						dictionaryReplace?.OnNext(new DictionaryReplaceEvent<TKey, TValue>(key, oldValue, value));
						dictionaryAnyChange?.OnNext(Unit.Default);
					}
				}
				else
				{
					inner[key] = value;

					if (!ReactiveCollectionReinitializeLock.IsLocked(this))
					{
						dictionaryAdd?.OnNext(new DictionaryAddEvent<TKey, TValue>(key, value));
						countChanged?.OnNext(Count);
						dictionaryAnyChange?.OnNext(Unit.Default);
					}
				}
			}
		}

		public bool ObservePerElementClear
		{
			get => observePerElementClear;
			set => observePerElementClear = value;
		}

		public int Count => inner.Count;
#if (NET_4_6 || NET_STANDARD_2_0)

		IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => inner.Keys;

		IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => inner.Values;

#endif

		public Dictionary<TKey, TValue>.KeyCollection Keys => inner.Keys;

		public Dictionary<TKey, TValue>.ValueCollection Values => inner.Values;

		public void Add(TKey key, TValue value)
		{
			inner.Add(key, value);

			if (ReactiveCollectionReinitializeLock.IsLocked(this))
				return;

			dictionaryAdd?.OnNext(new DictionaryAddEvent<TKey, TValue>(key, value));
			countChanged?.OnNext(Count);
			dictionaryAnyChange?.OnNext(Unit.Default);
		}

		public void Clear()
		{
			if (ReactiveCollectionReinitializeLock.IsLocked(this))
			{
				inner.Clear();
				return;
			}

			var beforeCount = Count;

			if (observePerElementClear && dictionaryRemove != null)
			{
				foreach (var pair in inner)
					dictionaryRemove.OnNext(new DictionaryRemoveEvent<TKey, TValue>(pair.Key, pair.Value));
			}

			inner.Clear();

			collectionReset?.OnNext(Unit.Default);

			if (beforeCount > 0)
				countChanged?.OnNext(Count);

			dictionaryAnyChange?.OnNext(Unit.Default);
		}

		public bool Remove(TKey key)
		{
			TValue oldValue;

			if (inner.TryGetValue(key, out oldValue))
			{
				var isSuccessRemove = inner.Remove(key);

				if (isSuccessRemove && !ReactiveCollectionReinitializeLock.IsLocked(this))
				{
					dictionaryRemove?.OnNext(new DictionaryRemoveEvent<TKey, TValue>(key, oldValue));
					countChanged?.OnNext(Count);
					dictionaryAnyChange?.OnNext(Unit.Default);
				}

				return isSuccessRemove;
			}

			return false;
		}

		void IReinitializedLockedCollection.RaiseOnReinitialized()
		{
			collectionReinitialized?.OnNext(Unit.Default);
			dictionaryAnyChange?.OnNext(Unit.Default);
		}

		IEnumerable<IDisposable> IReinitializedLockedCollection.Disposables => inner.Values.OfType<IDisposable>();

		public bool ContainsKey(TKey key)
		{
			return inner.ContainsKey(key);
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			return inner.TryGetValue(key, out value);
		}

		public Dictionary<TKey, TValue>.Enumerator GetEnumerator()
		{
			return inner.GetEnumerator();
		}

		private void DisposeSubject<TSubject>(ref Subject<TSubject> subject)
		{
			if (subject != null)
			{
				try
				{
					subject.OnCompleted();
				}
				finally
				{
					subject.Dispose();
					subject = null;
				}
			}
		}

		#region IDisposable Support

		private bool disposedValue;

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					DisposeSubject(ref countChanged);
					DisposeSubject(ref collectionReset);
					DisposeSubject(ref dictionaryAdd);
					DisposeSubject(ref dictionaryRemove);
					DisposeSubject(ref dictionaryReplace);
					DisposeSubject(ref dictionaryAnyChange);
				}

				disposedValue = true;
			}
		}

		public bool IsDisposed => disposedValue;

		public void Dispose()
		{
			Dispose(true);
		}

		#endregion


		#region Observe

		[NonSerialized]
		private Subject<Unit> collectionReinitialized;

		public IObservable<Unit> ObserveReinitializeFinished()
		{
			if (isDisposed) return Observable.Empty<Unit>();

			return collectionReinitialized ?? (collectionReinitialized = new Subject<Unit>());
		}

		[NonSerialized]
		private Subject<int> countChanged;

		public IObservable<int> ObserveCountChanged(bool notifyCurrentCount = false)
		{
			if (isDisposed) return Observable.Empty<int>();

			var subject = countChanged ?? (countChanged = new Subject<int>());

			if (notifyCurrentCount)
				return subject.StartWith(() => Count);

			return subject;
		}

		[NonSerialized]
		private Subject<Unit> collectionReset;

		public IObservable<Unit> ObserveReset()
		{
			if (isDisposed) return Observable.Empty<Unit>();

			return collectionReset ?? (collectionReset = new Subject<Unit>());
		}

		[NonSerialized]
		private Subject<DictionaryAddEvent<TKey, TValue>> dictionaryAdd;

		public IObservable<DictionaryAddEvent<TKey, TValue>> ObserveAdd()
		{
			if (isDisposed) return Observable.Empty<DictionaryAddEvent<TKey, TValue>>();

			return dictionaryAdd ?? (dictionaryAdd = new Subject<DictionaryAddEvent<TKey, TValue>>());
		}

		[NonSerialized]
		private Subject<DictionaryRemoveEvent<TKey, TValue>> dictionaryRemove;

		public IObservable<DictionaryRemoveEvent<TKey, TValue>> ObserveRemove(bool observePerElementClear = false)
		{
			if (isDisposed) return Observable.Empty<DictionaryRemoveEvent<TKey, TValue>>();

			if (observePerElementClear)
				this.observePerElementClear = true;

			return dictionaryRemove ?? (dictionaryRemove = new Subject<DictionaryRemoveEvent<TKey, TValue>>());
		}

		[NonSerialized]
		private Subject<DictionaryReplaceEvent<TKey, TValue>> dictionaryReplace;

		public IObservable<DictionaryReplaceEvent<TKey, TValue>> ObserveReplace()
		{
			if (isDisposed) return Observable.Empty<DictionaryReplaceEvent<TKey, TValue>>();

			return dictionaryReplace ?? (dictionaryReplace = new Subject<DictionaryReplaceEvent<TKey, TValue>>());
		}

		[NonSerialized]
		private Subject<Unit> dictionaryAnyChange;

		public IObservable<Unit> ObserveAnyChange()
		{
			if (isDisposed) return Observable.Empty<Unit>();

			return dictionaryAnyChange ?? (dictionaryAnyChange = new Subject<Unit>());
		}

		#endregion

		#region implement explicit

		object IDictionary.this[object key]
		{
			get => this[(TKey) key];

			set => this[(TKey) key] = (TValue) value;
		}


		bool IDictionary.IsFixedSize => ((IDictionary) inner).IsFixedSize;

		bool IDictionary.IsReadOnly => ((IDictionary) inner).IsReadOnly;

		bool ICollection.IsSynchronized => ((IDictionary) inner).IsSynchronized;

		ICollection IDictionary.Keys => ((IDictionary) inner).Keys;

		object ICollection.SyncRoot => ((IDictionary) inner).SyncRoot;

		ICollection IDictionary.Values => ((IDictionary) inner).Values;


		bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => ((ICollection<KeyValuePair<TKey, TValue>>) inner).IsReadOnly;

		ICollection<TKey> IDictionary<TKey, TValue>.Keys => inner.Keys;

		ICollection<TValue> IDictionary<TKey, TValue>.Values => inner.Values;

		void IDictionary.Add(object key, object value)
		{
			Add((TKey) key, (TValue) value);
		}

		bool IDictionary.Contains(object key)
		{
			return ((IDictionary) inner).Contains(key);
		}

		void ICollection.CopyTo(Array array, int index)
		{
			((IDictionary) inner).CopyTo(array, index);
		}

#if !UNITY_METRO

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			((ISerializable) inner).GetObjectData(info, context);
		}

		public void OnDeserialization(object sender)
		{
			((IDeserializationCallback) inner).OnDeserialization(sender);
		}

#endif

		void IDictionary.Remove(object key)
		{
			Remove((TKey) key);
		}

		void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
		{
			Add(item.Key, item.Value);
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
		{
			return ((ICollection<KeyValuePair<TKey, TValue>>) inner).Contains(item);
		}

		void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			((ICollection<KeyValuePair<TKey, TValue>>) inner).CopyTo(array, arrayIndex);
		}

		IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
		{
			return ((ICollection<KeyValuePair<TKey, TValue>>) inner).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return inner.GetEnumerator();
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
		{
			TValue v;

			if (TryGetValue(item.Key, out v))
			{
				if (EqualityComparer<TValue>.Default.Equals(v, item.Value))
				{
					Remove(item.Key);
					return true;
				}
			}

			return false;
		}

		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			return ((IDictionary) inner).GetEnumerator();
		}

		#endregion
	}

	public static class ReactiveDictionaryExtensions
	{
		public static ReactiveDictionary<TKey, TValue> ToReactiveDictionary<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
		{
			return new ReactiveDictionary<TKey, TValue>(dictionary);
		}
	}
}