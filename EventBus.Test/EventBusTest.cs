using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;
using EventBus.Test.Events.Events;
using EventBus.Test.Events.EventsHandler;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RabbitMQ.Client;

namespace EventBus.Test
{


    [TestClass]
    public class EventBusTest
    {
        private ServiceCollection _services;
        
        public EventBusTest()
        {
           _services = new ServiceCollection();
            _services.AddLogging(configure => configure.AddConsole());
        }
        [TestMethod]
        public void Subscribe_event_on_rabbitmq_test()
        {

            _services.AddSingleton<IEventBus>(sp =>
            {
                EventBusConfig config = new()
                {
                    ConnectionRetryCount = 5,
                    SubscriberClientAppName = "EventBus.UnitTest",
                    DefaultTopicName = "SellingBuddyTopicName",
                    EventBusType = EventBusType.RabbitMQ,
                    EventNameSuffix = "IntegrationEvent",
                    Connection = new ConnectionFactory()
                    {
                        HostName = "localhost",
                        Port = 15672,
                        UserName = "guest",
                        Password = "guest",
                    }
                };
              return  EventBusFactory.Create(config,sp);
            });



            var sp = _services.BuildServiceProvider();  
            var eventBus = sp.GetRequiredService<IEventBus>();

            eventBus.Subscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();
            eventBus.UnSubscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();
        }

        public void Subscribe_event_on_azure_test()
        {

            _services.AddSingleton<IEventBus>(sp =>
            {
                EventBusConfig config = new()
                {
                    ConnectionRetryCount = 5,
                    SubscriberClientAppName = "EventBus.UnitTest",
                    DefaultTopicName = "SellingBuddyTopicName",
                    EventBusType = EventBusType.RabbitMQ,
                    EventNameSuffix = "IntegrationEvent",
                    EventBusConnectionString = ""
                  

                };
                return EventBusFactory.Create(config, sp);
            });



            var sp = _services.BuildServiceProvider();
            var eventBus = sp.GetRequiredService<IEventBus>();

            eventBus.Subscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();
            eventBus.UnSubscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();
        }
    }
}