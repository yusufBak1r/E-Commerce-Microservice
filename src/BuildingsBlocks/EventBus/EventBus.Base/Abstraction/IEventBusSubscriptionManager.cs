using EventBus.Base.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Base.Abstraction
{
    public interface IEventBusSubscriptionManager
    { 
        bool isEmpty { get; }

        event EventHandler<string> OnEventRemoved;

        void AddSubscription<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>;

        void RemoveSubscription<T, TH>() where TH : IIntegrationEventHandler<T> where T:IntegrationEvent;

        bool HasSubscription<T>()where T : IntegrationEvent;

        bool HasSubscriptionForEvent(string eventName);

        Type GetEventTypeByName(string eventName);

        void Clear();

        IEnumerable<SubscribtionInfo> GetHandlersForEvent<T>() where T:IntegrationEvent;

        IEnumerable<SubscribtionInfo> GetHandlersForEvent(string eventName);

        string GetEventKey<T>();

    }
}
