using System;
using System.Collections.Generic;
using UnityEngine;

namespace UniRx
{
    public static partial class DisposableExtensions
    {
        /// <summary>Add disposable(self) to CompositeDisposable(or other ICollection). Return value is self disposable.</summary>
        public static T AddTo<T>(this T disposable, ICollection<IDisposable> container)
            where T : IDisposable
        {
            if (disposable == null) throw new ArgumentNullException("disposable");
            if (container == null) throw new ArgumentNullException($"container for {disposable} ");

            container.Add(disposable);

            return disposable;
        }
        
        public static T AddTo<T>(this T disposable, CompositeDisposable container, int priority)
            where T : IDisposable
        {
            if (disposable == null) throw new ArgumentNullException("disposable");
            if (container == null) throw new ArgumentNullException($"container for {disposable} ");

            container.Add(disposable, priority);

            return disposable;
        }
    }
}