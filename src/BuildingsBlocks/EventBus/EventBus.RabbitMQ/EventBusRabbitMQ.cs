using EventBus.Base;
using EventBus.Base.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.RabbitMQ
{
    class EventBusRabbitMQ : BaseEventBus
    {
        public EventBusRabbitMQ(IServiceProvider serviceProvider, EventBusConfig config) : base(serviceProvider, config)
        {
        }

        public override void Publish(IntegrationEvent @event)
        {
            base.Publish(@event);
        }
        public override void Subscribe<T, TH>()
        {
            base.Subscribe<T, TH>();
        }
        public override void UnSubscribe<T, TH>()
        {
            base.UnSubscribe<T, TH>();
        }
    }
}
