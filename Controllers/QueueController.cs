using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
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
    public class QueueController : ControllerBase
    {
        private static string connectionString = "";
        private static string queue = "";
        private IQueueClient queueClient;
        public QueueController()
        {
            queueClient = new QueueClient(connectionString, queue);
           

        }

        public async Task<ActionResult> SendMessage(string message)
        {
            await queueClient.SendAsync(new Message(Encoding.UTF8.GetBytes(message)));
            return Ok("Message added to the queue");
        }


        public ActionResult ReceiveMessage(string message)
        {

            var Options = new MessageHandlerOptions(EventException)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };
            queueClient.RegisterMessageHandler(EventMessage,Options);
            return Ok("Message added to the queue");
        }

        public async Task EventMessage(Message message, CancellationToken token)
        {
            Encoding.UTF8.GetString(message.Body);

            await queueClient.CompleteAsync(message.SystemProperties.LockToken);
        }

        public Task EventException(ExceptionReceivedEventArgs args)
        {
            Console.WriteLine(args.Exception);
            return Task.CompletedTask;
        }

    }
}
