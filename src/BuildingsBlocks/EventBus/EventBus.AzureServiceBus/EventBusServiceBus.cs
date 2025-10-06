using EventBus.Base;
using EventBus.Base.Events;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.AzureServiceBus
{
    public class EventBusServiceBus : BaseEventBus
    {
        private  ITopicClient topicClient;
        private ManagementClient managerClient;
        private ILogger logger; 

        public EventBusServiceBus(IServiceProvider serviceProvider, EventBusConfig config) : base(serviceProvider, config)
        {
            logger = serviceProvider.GetService(typeof(ILogger<EventBusServiceBus>)) as ILogger<EventBusServiceBus>;

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
            topicClient.CloseAsync().GetAwaiter().GetResult();
            managerClient.CloseAsync().GetAwaiter().GetResult();
            topicClient = null;
            managerClient = null;
            

        }

        public override void Publish(IntegrationEvent @event)
        {

            var eventName = @event.GetType().Name;  // example OrderCreatedIntegrationEvent
            eventName = ProcessEventName(eventName);
            var eventStr = JsonConvert.SerializeObject(@event);
            var bodyArr = Encoding.UTF8.GetBytes(eventStr);

            var message = new Message()
            {
                MessageId = Guid.NewGuid().ToString(),  
                Body = null,
                Label = eventName
            };
            topicClient.SendAsync(message).GetAwaiter().GetResult();
        }
        public override void Subscribe<T, TH>()
        {
            var eventName = typeof(T).Name;
            eventName = ProcessEventName(eventName);
            if (!_subsManager.HasSubscriptionForEvent(eventName))
            {
                var subscriptionClient = createSubscriptionClient(eventName);

                RegisterSubscriptionClientMessageHandler(subscriptionClient);
            }
            _subsManager.AddSubscription<T,TH>();
            logger.LogInformation("subscribing to event {EventNmae} with {EventHandler}",eventName, typeof(TH).Name);
        }


        public override void UnSubscribe<T, TH>()
        {
            var eventName = typeof(T).Name;
            try
            {
                var subscriptionClient = createSubscriptionClient(eventName);
                subscriptionClient.RemoveRuleAsync(eventName).GetAwaiter().GetResult(); 
            }
            catch (MessagingEntityAlreadyExistsException)
            {
                
                
                logger.LogWarning("the messaging entitiy {eventName} Could not be found.", eventName);
            
           
            
            }
            _subsManager.RemoveSubscription<T, TH>();
            logger.LogInformation($"unsubscribtion    to remove subscription from {eventName}");
        }


        private void RegisterSubscriptionClientMessageHandler(ISubscriptionClient subscriptionClient)
        {
            subscriptionClient.RegisterMessageHandler(
                async (message, token) =>
                {
                    var eventName = $"{message.Label}";
                    var messageData = Encoding.UTF8.GetString(message.Body);
                    if (await ProcessEvent(ProcessEventName(eventName), messageData))
                    {
                        await subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
                    }
                },
                new MessageHandlerOptions(ExceptionReceivedHandler)
                {
                    MaxConcurrentCalls = 10,
                    AutoComplete = true
                });
                
                

            
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            var ex  = exceptionReceivedEventArgs.Exception;
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            logger.LogError(ex,"Error handling message : {ExceptionMessage} - Context: {@ExceptionContext}",ex.Message,context);
            return Task.CompletedTask;  
        }


        private ISubscriptionClient CretaeSubscriptionClientIfNotExists(string evenName)
        {
            var subClient = createSubscriptionClient(evenName);
         var exists =    managerClient.SubscriptionExistsAsync(EventBusConfig.DefaultTopicName,GetSubName(evenName)).GetAwaiter().GetResult();
            if (!exists)
            {
                managerClient.CreateSubscriptionAsync(EventBusConfig.DefaultTopicName, GetSubName(evenName)).GetAwaiter().GetResult();
                RemoveDefaultRule(subClient);
            }
            CreateRuleIfNotExists(ProcessEventName(evenName), subClient);
            return subClient;   
        }

        private void
            CreateRuleIfNotExists (string eventName,ISubscriptionClient subscriptionClient)
        {
            bool ruleExists;
            try
            {
                var rule = managerClient.GetRuleAsync(EventBusConfig.DefaultTopicName, eventName, eventName).GetAwaiter().GetResult();
                ruleExists = rule != null;  
            }
            catch(MessagingEntityAlreadyExistsException)
            { 
                ruleExists = false;
            }

            if (!ruleExists) 
            { 
            subscriptionClient.AddRuleAsync(new RuleDescription
            {
                Filter = new CorrelationFilter { Label = eventName},
                
            }).GetAwaiter().GetResult();
            }


        }

        private void RemoveDefaultRule(SubscriptionClient subscriptionClient)
        {
            try
            {
                subscriptionClient.RemoveRuleAsync(RuleDescription.DefaultRuleName).GetAwaiter().GetResult();
            } catch (MessagingEntityNotFoundException)
            {
                logger.LogWarning("the messaging entity{DefaultName} could not be found", RuleDescription.DefaultRuleName);

            } 
        }
        private SubscriptionClient createSubscriptionClient(string eventName)
        {
            return new SubscriptionClient(EventBusConfig.EventBusConnectionString, EventBusConfig.DefaultTopicName, GetSubName(eventName));
        }


    

    }
}
