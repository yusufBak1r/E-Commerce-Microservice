using EventBus.Base.Abstraction;
using EventBus.Base.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Base.InMemoryEventBusSubscriptionManager
{
    public class InMemoryEventBusSubscriptionManager : IEventBusSubscriptionManager
    {
        public bool isEmpty => throw new NotImplementedException();

        public event EventHandler<string> OnEventRemoved;

        public void AddSubscription<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public string GetEventKey<T>()
        {
            throw new NotImplementedException();
        }

        public Type GetEventTypeByName(string eventName)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SubscribtionInfo> GetHandlersForEvent<T>() where T : IntegrationEvent
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SubscribtionInfo> GetHandlersForEvent(string eventName)
        {
            throw new NotImplementedException();
        }

        public bool HasSubscription<T>() where T : IntegrationEvent
        {
            throw new NotImplementedException();
        }

        public bool HasSubscriptionForEvent(string eventName)
        {
            throw new NotImplementedException();
        }

        public void RemoveSubscription<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            throw new NotImplementedException();
        }
    }
}
