using EventBus.Base.Abstraction;
using EventBus.Test.Events.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Test.Events.EventsHandler
{
    public class OrderCreatedIntegrationEventHandler : IIntegrationEventHandler<OrderCreatedIntegrationEvent>
    {
        public Task Handler(OrderCreatedIntegrationEvent @event)
        {
           // 
           return Task.CompletedTask;   
        }
    }
}
