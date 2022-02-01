using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceBus.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TopicController: ControllerBase
    {

        private static string connectionString = "";
        private static string topic = "";
        private static string subscription = "";
        public string eventMessage = "";
        private ITopicClient topicClient;
        private SubscriptionClient subscriptionClient;
        public TopicController()
        {
            topicClient = new TopicClient(connectionString, topic);
            //ServiceBusConnectionStringBuilder connectionStringBuilder = new ServiceBusConnectionStringBuilder(connectionString);
            subscriptionClient = new SubscriptionClient(connectionString,topic, subscription);


        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult> SendMessage(string message)
        {
            await topicClient.SendAsync(new Message(Encoding.UTF8.GetBytes(message)));
            return Ok("Message added to the queue");
        }

        [HttpGet]
        [Route("[action]")]
        public ActionResult ReceiveMessage(string message)
        {

            var Options = new MessageHandlerOptions(EventException)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };
            subscriptionClient.RegisterMessageHandler(EventMessage, Options);
            return Ok(eventMessage);
        }

        public async Task EventMessage(Message message, CancellationToken token)
        {
           eventMessage =  Encoding.UTF8.GetString(message.Body);

            await subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
           
        }

        public Task EventException(ExceptionReceivedEventArgs args)
        {
            Console.WriteLine(args.Exception);
            return Task.CompletedTask;
        }
    }
}
