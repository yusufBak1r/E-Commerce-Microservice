using EventBus.Base;
using EventBus.Base.Events;
using Newtonsoft.Json;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.RabbitMQ
{
    public class EventBusRabbitMQ : BaseEventBus
    {
        RabbitMQPersistentConnection persistentConnection;
        private readonly IConnectionFactory connectionFactory;
        private readonly IModel consumerChannel;
        public EventBusRabbitMQ(IServiceProvider serviceProvider, EventBusConfig config) : base(serviceProvider, config)
        {

            if (config.Connection != null)
            {
                var connJson = JsonConvert.SerializeObject(EventBusConfig.Connection, new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });




            }else 
                connectionFactory = new ConnectionFactory();    

                persistentConnection = new RabbitMQPersistentConnection(connectionFactory,config.ConnectionRetryCount);
            consumerChannel = CreateConsumerChannel();
            _subsManager.OnEventRemoved += _subsManager_OnEventRemoved;
        }

        private void _subsManager_OnEventRemoved(object? sender, string eventName)
        {
            eventName = ProcessEventName(eventName);
            if (!persistentConnection.IsConnected)
            {
                persistentConnection.TryConnect();
            }
            consumerChannel.QueueUnbind(queue:eventName,exchange:EventBusConfig.DefaultTopicName,routingKey:eventName);

            if (_subsManager.isEmpty)
            {
                consumerChannel.Close();
            }
        }

        public override void Publish(IntegrationEvent @event)
        {
            if (!persistentConnection.IsConnected)
            {
                persistentConnection.TryConnect();
            }
            var policy = Policy.Handle<BrokerUnreachableException>().Or<SocketException>().WaitAndRetry(EventBusConfig.ConnectionRetryCount,retryAttempt => TimeSpan.FromSeconds(Math.Pow(2,retryAttempt)),(ex,time)=>
            {
                //log
            });
            var evenName = @event.GetType().Name;
            evenName = ProcessEventName(evenName);
            consumerChannel.ExchangeDeclare(exchange: EventBusConfig.DefaultTopicName, type: "direct");
            var message = JsonConvert.SerializeObject(@event);
            var body = Encoding.UTF8.GetBytes(message);
            policy.Execute(() =>
            {
                var properties = consumerChannel.CreateBasicProperties();
                properties.DeliveryMode = 2;
                consumerChannel.QueueDeclare(queue:GetSubName(evenName),durable:true,exclusive:false,autoDelete:false,arguments:null);

                consumerChannel.BasicPublish(exchange:EventBusConfig.DefaultTopicName,routingKey:evenName,mandatory:true,basicProperties:properties,body:body);
            });
           
        }
        public override void Subscribe<T, TH>()
        {
          
            var eventName = typeof(T).Name;
            eventName = ProcessEventName(eventName);
            eventName = ProcessEventName(eventName);
            if (!_subsManager.HasSubscriptionForEvent(eventName))
            {
                if (!persistentConnection.TryConnect()) {
                    consumerChannel.QueueDeclare(queue: GetSubName(eventName), durable: true, exclusive: false, autoDelete: false, arguments: null);
                    consumerChannel.QueueBind(queue:GetSubName(eventName),exchange:EventBusConfig.DefaultTopicName,routingKey:eventName);
                }
            
            }
            _subsManager.AddSubscription<T, TH>();
            StartBasicConsume(eventName);
        }

        //public override void Subscribe<T, TH>()
        //{
        //    var eventName = ProcessEventName(typeof(T).Name);

        //    if (!_subsManager.HasSubscriptionForEvent(eventName))
        //    {
        //        if (!persistentConnection.IsConnected)
        //            persistentConnection.TryConnect();

        //        // Queue ve binding her zaman oluştur
        //        consumerChannel.QueueDeclare(
        //            queue: GetSubName(eventName),
        //            durable: true,
        //            exclusive: false,
        //            autoDelete: false,
        //            arguments: null);

        //        consumerChannel.QueueBind(
        //            queue: GetSubName(eventName),
        //            exchange: EventBusConfig.DefaultTopicName,
        //            routingKey: eventName);
        //    }

        //    _subsManager.AddSubscription<T, TH>();
        //    StartBasicConsume(eventName);
        //}
        public override void UnSubscribe<T, TH>()
        {
            _subsManager.RemoveSubscription<T, TH>();

        }

        private IModel CreateConsumerChannel()
        {
            if (!persistentConnection.IsConnected)
            {
                persistentConnection.TryConnect();
            }
            var channel = persistentConnection.CreateModel();
            channel.ExchangeDeclare(exchange: EventBusConfig.DefaultTopicName, type: "direct");
            return channel;
        }

        private void StartBasicConsume(string eventName)
        {
            if (consumerChannel != null)
            {
                var consumer = new EventingBasicConsumer(consumerChannel);
                consumer.Received += Consumer_Received;
                consumerChannel.BasicConsume(queue: GetSubName(eventName), autoAck: false, consumer: consumer);
            }
        }

        private async void Consumer_Received(object? sender, BasicDeliverEventArgs e)
        {
            var eventName = e.RoutingKey;
            eventName = ProcessEventName(eventName);
            var message = Encoding.UTF8.GetString(e.Body);
            try
            {
                await ProcessEvent(eventName, message); 
            }
            catch (Exception ex) { }

            consumerChannel.BasicAck(e.DeliveryTag, multiple: false);
        }
    }
}
