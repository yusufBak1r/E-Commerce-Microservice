using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;
using EventBus.Main.Event;
using EventBus.Main.EventHandler;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualBasic;
using RabbitMQ.Client;
using System;
using System.Net;

class Program
{
    static void Main(string[] args)
    {
        var host = CreateHost();
        var eventBus = host.Services.GetRequiredService<IEventBus>();
        var order = new OrderCreatedIntegrationEvent(1);

        eventBus.Publish(order);

        //eventBus.Subscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();
        //Console.WriteLine("Subscribed to OrderCreatedIntegrationEvent");

        //eventBus.UnSubscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();
        //Console.WriteLine("Unsubscribed from OrderCreatedIntegrationEvent");
 
        //Console.ReadLine();
       
    }
    public static IHost CreateHost()
    {
        var host = Host.CreateDefaultBuilder().ConfigureAppConfiguration((context, config) =>
        {

            config.AddJsonFile("appsettings.json",optional:true,reloadOnChange:true);
            config.AddEnvironmentVariables();
        })            
            
            
  
        .ConfigureServices((context, service) =>
        {
            service.AddSingleton<IEventBus>(sp =>
            {
                var configure = new EventBusConfig()
                {
                    ConnectionRetryCount = 5,
                    SubscriberClientAppName = "EventBus.Main",
                    DefaultTopicName = "Anonymous.Tehc",
                    EventBusType = EventBusType.RabbitMQ,
                    EventNameSuffix = "IntegrationEvent",
                    //Connection = new ConnectionFactory()
                    //{
                    //    HostName = "localhost",
                    //    Port = 5672,
                    //    UserName = "guest",
                    //    Password = "guest"
                    //}
                };
                return EventBusFactory.Create(configure, sp);
            });
           
            service.AddTransient<OrderCreatedIntegrationEventHandler>();
            service.BuildServiceProvider();



        }).Build();
    
    
    return host;
    }
}
