using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace UniRx
{
	public static partial class ReactiveCollectionExtensions
	{
		internal class BaseCollectionAdapter<T, TBase> : IReadOnlyReactiveCollection<TBase> where T : TBase where TBase : class
		{
			private readonly ReactiveCollection<T> _parent;

			internal BaseCollectionAdapter(ReactiveCollection<T> parent)
			{
				_parent = parent;
			}

			public static explicit operator BaseCollectionAdapter<T, TBase>(ReactiveCollection<T> collection)
			{
				return AsVariantCollectionInternal<T, TBase>(collection);
			}

			public TBase this[int index] => _parent[index];

			public int Count => _parent.Count;

			public IEnumerator<TBase> GetEnumerator()
			{
				using (var enumerator = _parent.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						yield return enumerator.Current;
					}
				}
			}

			public IObservable<CollectionAddEvent<TBase>> ObserveAdd()
			{
				return _parent.ObserveAdd().Select(x => new CollectionAddEvent<TBase>(x.Index, x.Value));
			}

			public IObservable<int> ObserveCountChanged(bool notifyCurrentCount = false)
			{
				return _parent.ObserveCountChanged(notifyCurrentCount);
			}

			public IObservable<CollectionMoveEvent<TBase>> ObserveMove()
			{
				return _parent.ObserveMove().Select(x => new CollectionMoveEvent<TBase>(x.OldIndex, x.NewIndex, x.Value));
			}

			public IObservable<CollectionRemoveEvent<TBase>> ObserveRemove()
			{
				return _parent.ObserveRemove().Select(x => new CollectionRemoveEvent<TBase>(x.Index, x.Value));
			}

			public IObservable<CollectionReplaceEvent<TBase>> ObserveReplace()
			{
				return _parent.ObserveReplace().Select(x => new CollectionReplaceEvent<TBase>(x.Index, x.OldValue, x.NewValue));
			}

			public IObservable<Unit> ObserveReset()
			{
				return _parent.ObserveReset();
			}

			public IObservable<Unit> ObserveReinitializeFinished()
			{
				return _parent.ObserveReinitializeFinished();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
		}

		public static IReadOnlyReactiveCollection<TBase> AsVariant<T, TBase>([NotNull] this ReactiveCollection<T> reactiveCollection) where TBase : class where T : TBase
		{
			return AsVariantCollectionInternal<T, TBase>(reactiveCollection);
		}

		internal static BaseCollectionAdapter<T, TBase> AsVariantCollectionInternal<T, TBase>([NotNull] this ReactiveCollection<T> reactiveCollection)
			where TBase : class where T : TBase
		{
			if (reactiveCollection.TryGetBaseCollection(typeof(TBase), out var collection))
				return (BaseCollectionAdapter<T, TBase>) collection;

			var adapter = new BaseCollectionAdapter<T, TBase>(reactiveCollection);
			reactiveCollection.AddBaseCollection(typeof(TBase), adapter);

			return adapter;
		}
	}

	public partial class ReactiveCollection<T>
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