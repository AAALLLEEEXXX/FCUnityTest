using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace UniRx
{
	public static partial class ReactiveCollectionExtensions
	{
		internal class BaseDictionaryAdapter<TKey, TValue, TValueBase> : IReadOnlyReactiveDictionary<TKey, TValueBase>
			where TValue : TValueBase
			where TValueBase : class
		{
			private readonly ReactiveDictionary<TKey, TValue> _parent;

			internal BaseDictionaryAdapter(ReactiveDictionary<TKey, TValue> parent)
			{
				_parent = parent;
			}

			public TValueBase this[TKey key] => _parent[key];

			public int Count => _parent.Count;

			public IEnumerable<TKey> Keys => _parent.Keys;

			public IEnumerable<TValueBase> Values
			{
				get
				{
					foreach (var value in _parent.Values)
						yield return value;
				}
			}

			public bool IsDisposed => _parent.IsDisposed;


			public IObservable<Unit> ObserveReinitializeFinished()
			{
				return _parent.ObserveReinitializeFinished();
			}


			public IEnumerator<KeyValuePair<TKey, TValueBase>> GetEnumerator()
			{
				using (var enumerator = _parent.GetEnumerator())
				{
					while (enumerator.MoveNext())
						yield return new KeyValuePair<TKey, TValueBase>(enumerator.Current.Key, enumerator.Current.Value);
				}
			}

			public bool ContainsKey(TKey key)
			{
				return _parent.ContainsKey(key);
			}

			public bool TryGetValue(TKey key, out TValueBase value)
			{
				if (_parent.TryGetValue(key, out var parentValue))
				{
					value = parentValue;
					return true;
				}

				value = default;
				return false;
			}

			public IObservable<DictionaryAddEvent<TKey, TValueBase>> ObserveAdd()
			{
				return _parent.ObserveAdd().Select(x => new DictionaryAddEvent<TKey, TValueBase>(x.Key, x.Value));
			}

			public IObservable<int> ObserveCountChanged(bool notifyCurrentCount = false)
			{
				return _parent.ObserveCountChanged(notifyCurrentCount);
			}

			public IObservable<DictionaryRemoveEvent<TKey, TValueBase>> ObserveRemove(bool observePerElementClear = false)
			{
				return _parent.ObserveRemove(observePerElementClear).Select(x => new DictionaryRemoveEvent<TKey, TValueBase>(x.Key, x.Value));
			}

			public IObservable<DictionaryReplaceEvent<TKey, TValueBase>> ObserveReplace()
			{
				return _parent.ObserveReplace().Select(x => new DictionaryReplaceEvent<TKey, TValueBase>(x.Key, x.OldValue, x.NewValue));
			}

			public IObservable<Unit> ObserveReset()
			{
				return _parent.ObserveReset();
			}
			public IObservable<Unit> ObserveAnyChange()
			{
				return _parent.ObserveAnyChange();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
		}

		public static IReadOnlyReactiveDictionary<TKey, TValueBase> AsVariant<TKey, TValue, TValueBase>([NotNull]
			this ReactiveDictionary<TKey, TValue> reactiveDictionary)
			where TValueBase : class
			where TValue : TValueBase
		{
			return AsVariantInternal<TKey, TValue, TValueBase>(reactiveDictionary);
		}

		internal static BaseDictionaryAdapter<TKey, TValue, TValueBase> AsVariantInternal<TKey, TValue, TValueBase>(
			[NotNull]
			this ReactiveDictionary<TKey, TValue> reactiveDictionary)
			where TValueBase : class
			where TValue : TValueBase
		{
			if (reactiveDictionary.TryGetBaseCollection(typeof(TValueBase), out var collection))
				return (BaseDictionaryAdapter<TKey, TValue, TValueBase>) collection;

			var adapter = new BaseDictionaryAdapter<TKey, TValue, TValueBase>(reactiveDictionary);
			reactiveDictionary.AddBaseCollection(typeof(TValueBase), adapter);
			return adapter;
		}
	}

	public partial class ReactiveDictionary<TKey, TValue>
	{
		private Dictionary<Type, object> _baseCollections;

		internal bool TryGetBaseCollection(Type type, out object collection)
		{
			collection = default;
			return _baseCollections != null && _baseCollections.TryGetValue(type, out collection);
		}

		internal void AddBaseCollection(Type type, object collection)
		{
			if (_baseCollections == null)
				_baseCollections = new Dictionary<Type, object>();

			_baseCollections.Add(type, collection);
		}
	}
}