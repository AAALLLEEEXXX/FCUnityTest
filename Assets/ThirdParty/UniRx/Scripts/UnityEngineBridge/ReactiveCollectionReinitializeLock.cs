using System;

namespace UniRx
{
	/// <summary>
	/// Подавляет вызов OnAdd, OnRemove, OnClear и остальные, но в конце вызывает collectionReinitialized
	/// </summary>
	public struct ReactiveCollectionReinitializeLock : IDisposable
	{
		/// <summary>
		/// Помимо подавления событий, ещё диспоузит все элементы и очищает коллекцию.
		/// </summary>
		internal static ReactiveCollectionReinitializeLock Reinitialize(IReinitializedLockedCollection collection, bool disposeItems = true)
		{
			var reinitializeLock = new ReactiveCollectionReinitializeLock(collection);

			if (reinitializeLock._collection.ReinitializeLockCount == 1)
			{
				if (disposeItems)
				{
					foreach (var disposable in reinitializeLock._collection.Disposables)
						disposable.Dispose();
				}

				reinitializeLock._collection.Clear();
			}

			return reinitializeLock;
		}

		/// <summary>
		/// Просто подавляет все события и в конце вызывает collectionReinitialized.
		/// </summary>
		internal static ReactiveCollectionReinitializeLock SuppressEvents(IReinitializedLockedCollection collection)
		{
			return new ReactiveCollectionReinitializeLock(collection);
		}

		private readonly IReinitializedLockedCollection _collection;
		private bool _disposed;

		private ReactiveCollectionReinitializeLock(IReinitializedLockedCollection collection)
		{
			_disposed = false;
			_collection = collection;
			_collection.ReinitializeLockCount++;
		}

		public void Dispose()
		{
			if (_disposed)
				return;

			_disposed = true;
			_collection.ReinitializeLockCount--;

			if (_collection.ReinitializeLockCount == 0)
				_collection.RaiseOnReinitialized();
		}

		internal static bool IsLocked(IReinitializedLockedCollection collection)
		{
			return collection.ReinitializeLockCount > 0;
		}
	}
}