using System;

namespace UniRx
{
	public interface IReadonlyReinitializableCollection
	{
		IObservable<Unit> ObserveReinitializeFinished();
	}

	public interface IReinitializableCollection
	{
		ReactiveCollectionReinitializeLock Reinitialize(bool disposeValues = true);
		ReactiveCollectionReinitializeLock SuppressEvents();
	}
}