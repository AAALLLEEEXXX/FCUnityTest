using System;

namespace FusionCore.Test.Generic
{
    public interface IReadOnlySubscriptionProperty<T>
    {
        T Value { get; set; }
        void SubscribeOnChange(Action<T> subscriptionAction);
        void UnSubscriptionOnChange(Action<T> unsubscriptionAction);
    }
}