using EventBus.Base.Abstraction;
using EventBus.Base;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Base.Events
{
    public class BaseEventBus : IEventBus
    {
        public readonly IServiceProvider _serviceProvider;
        public IEventBusSubscriptionManager _subsManager;
        public EventBusConfig EventBusConfig { get; set; }

        public BaseEventBus(IServiceProvider serviceProvider, EventBusConfig config)
        {
            _serviceProvider = serviceProvider;
            _subsManager = new InMemoryEventBusSubscriptionManager(ProcessEventName);
            EventBusConfig = config;    

        }   
         public virtual void Dispose()
        {
            EventBusConfig = null;
            _subsManager.Clear();
        }

        public  virtual string ProcessEventName(string eventName)
        {
            if (EventBusConfig.DeleteEventPrefix)
            eventName = eventName.TrimStart(EventBusConfig.EventNamePrefix.ToCharArray());
            if(EventBusConfig.DeleteEventSuffix)
                eventName = eventName.TrimEnd(EventBusConfig.EventNameSuffix.ToCharArray());    
            return eventName;

        }


        public virtual string GetSubName(string eventName)
        {
            return $"{EventBusConfig.SubscriberClientAppName}.{ProcessEventName(eventName)}";
        }

        public async Task<bool> ProcessEvent(string eventName, string message)
        {
            eventName = ProcessEventName(eventName);    
            var process = false;
            try
            {
                if (_subsManager.HasSubscriptionForEvent(eventName))
                {
                    var subscriptions = _subsManager.GetHandlersForEvent(eventName);

                    using (var scope = _serviceProvider.CreateScope())
                    {
                        foreach (var subscription in subscriptions)
                        {
                            var handler = _serviceProvider.GetService(subscription.HandlerType);

                            if (handler == null) continue;
                            var eventType = _subsManager.GetEventTypeByName(eventName);

                            var integrationEvent = JsonConvert.DeserializeObject(message, eventType);

                            var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);

                            await Task.Yield();
                            await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { integrationEvent });
                            process = true;
                        }
                        process = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


            return process;
        }
        public virtual void Publish(IntegrationEvent @event)
        {
            throw new NotImplementedException();
        }

        public virtual void Subscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            throw new NotImplementedException();
        }

        public virtual void UnSubscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            throw new NotImplementedException();
        }
    }
}
