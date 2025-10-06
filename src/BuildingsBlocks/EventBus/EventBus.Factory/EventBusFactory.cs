using EventBus.AzureServiceBus;
using EventBus.RabbitMQ;
using EventBus.Base;
using EventBus.Base.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Metadata.Ecma335;


namespace EventBus.Factory
{
    public static class EventBusFactory
    {

        public static IEventBus Create(EventBusConfig config, IServiceProvider serviceProvider)
        {
            return config.EventBusType switch
            {
                EventBusType.AzureServiceBus => new EventBusServiceBus(serviceProvider, config),
                EventBusType.RabbitMQ => new EventBusRabbitMQ(serviceProvider, config),
                _ => throw new NotImplementedException()
            };

        }



    }
}
