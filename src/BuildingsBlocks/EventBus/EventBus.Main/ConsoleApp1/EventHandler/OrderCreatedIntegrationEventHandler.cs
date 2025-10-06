using EventBus.Base.Abstraction;
using EventBus.Main.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Main.EventHandler
{
    public class OrderCreatedIntegrationEventHandler : IIntegrationEventHandler<OrderCreatedIntegrationEvent>
    {
        public Task Handler(OrderCreatedIntegrationEvent @event)
        {
            throw new NotImplementedException();
        }
    }
}
