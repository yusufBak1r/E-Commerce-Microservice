using EventBus.Base.Abstraction;
using EventBus.Base.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Base
{
    public class InMemoryEventBusSubscriptionManager : IEventBusSubscriptionManager
    {

        private readonly Dictionary<string, List<SubscribtionInfo>> _handlers;
        public readonly List<Type> _eventType;
        public event EventHandler<string> OnEventRemoved;

        public Func<string, string> eventNameGetter;

        public InMemoryEventBusSubscriptionManager(Func<string, string> eventNameGetter)
        {
            _handlers = new Dictionary<string, List<SubscribtionInfo>>();
            _eventType = new List<Type>();
            this.eventNameGetter = eventNameGetter;
        }

        public bool isEmpty => !_handlers.Keys.Any();


        public void Clear() => _handlers.Clear();




        public void AddSubscription<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = GetEventKey<T>();

            AddSubscription(typeof(TH), eventName);

            if (!_eventType.Contains(typeof(T)))
            {
                _eventType.Add(typeof(T));

            }
        }

        private void AddSubscription(Type handlerType, string eventName)
        {
            if (!HasSubscriptionForEvent(eventName))
            {
                _handlers.Add(eventName, new List<SubscribtionInfo>());
            }

            if (_handlers[eventName].Any(s => s.HandlerType == handlerType))
            {
                throw new ArgumentException($"Handler Type {handlerType.Name} already registered for '{eventName}'", nameof(handlerType));
            }

            _handlers[eventName].Add(SubscribtionInfo.Typed(handlerType));
        }

        private void RemoveHandler(string eventName, SubscribtionInfo subsToRemove)
        {
            if (subsToRemove != null)
            {
                _handlers[eventName].Remove(subsToRemove);
                if (!_handlers[eventName].Any())
                {
                    _handlers.Remove(eventName);
                    var eventType = _eventType.SingleOrDefault(e => e.Name == eventName);
                    if (eventType != null)
                    {
                        _eventType.Remove(eventType);
                    }
                    RaiseOnEventRemoved(eventName);
                }
            }
        }

        private SubscribtionInfo FindSubscriptionToRemove<T, TH>()
       where T : IntegrationEvent
       where TH : IIntegrationEventHandler<T>
        {
            var eventName = GetEventKey<T>();
            return FindSubscriptionToRemove(eventName, typeof(TH));
        }




        public IEnumerable<SubscribtionInfo> GetHandlersForEvent<T>() where T : IntegrationEvent
        {
            var key = GetEventKey<T>();
            return GetHandlersForEvent(key);
        }

        public IEnumerable<SubscribtionInfo> GetHandlersForEvent(string eventName) => _handlers[eventName];

        private void RaiseOnEventRemoved(string eventName)
        {
            var handler = OnEventRemoved;
            handler?.Invoke(this, eventName);
        }


        private SubscribtionInfo FindSubscriptionToRemove(string eventName, Type handlerType)
        {
            if (!HasSubscriptionForEvent(eventName))
            {
                return null;
            }

            return _handlers[eventName].SingleOrDefault(s => s.HandlerType == handlerType);

        }






        public Type GetEventTypeByName(string eventName) => _eventType.SingleOrDefault(q => q.Name == eventName);

        public string GetEventKey<T>()
        {
            string eventName = typeof(T).Name;
            return eventNameGetter(eventName);
        }





        public bool HasSubscription<T>() where T : IntegrationEvent
        {
            string key = GetEventKey<T>();
            return HasSubscriptionForEvent(key);
        }

        public bool HasSubscriptionForEvent(string eventName) => _handlers.ContainsKey(eventName);


        public void RemoveSubscription<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var handlerToRemove = FindSubscriptionToRemove<T, TH>();
            var eventName = GetEventKey<T>();
            RemoveHandler(eventName, handlerToRemove);
        }





    }
}
