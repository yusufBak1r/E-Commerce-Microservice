using EventBus.Base;
using EventBus.Base.Events;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.AzureServiceBus
{
    public class EventBusServiceBus : BaseEventBus
    {
        private ITopicClient topicClient;
        private ManagementClient managerClient;

        public EventBusServiceBus(IServiceProvider serviceProvider, EventBusConfig config) : base(serviceProvider, config)
        {
            managerClient = new ManagementClient(config.EventBusConnectionString);
            topicClient = CreateTopicClient();
                
               
        }
        public ITopicClient CreateTopicClient()
        {
            if(topicClient == null || topicClient.IsClosedOrClosing)
            {
                topicClient = new TopicClient(EventBusConfig.EventBusConnectionString,EventBusConfig.DefaultTopicName,RetryPolicy.Default);
            }
            // Ensure the topic exists  
            if (!managerClient.TopicExistsAsync(EventBusConfig.DefaultTopicName).GetAwaiter().GetResult())
                managerClient.CreateTopicAsync(EventBusConfig.DefaultTopicName).GetAwaiter().GetResult();
              return topicClient; 

        }

        public override void Dispose()
        {
            base.Dispose();
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
