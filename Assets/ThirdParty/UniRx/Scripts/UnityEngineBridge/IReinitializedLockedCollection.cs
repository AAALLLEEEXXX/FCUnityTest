using System;
using System.Collections.Generic;

namespace UniRx
{
	internal interface IReinitializedLockedCollection
	{
		int ReinitializeLockCount { get; set; }
		IEnumerable<IDisposable> Disposables { get; }

		void Clear();
		void RaiseOnReinitialized();
	}
}