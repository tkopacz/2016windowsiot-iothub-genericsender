using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IEventProcessorHost
{
    partial class Program
    {
        /*
         * Change to own params
        const string eventHubConnectionString = "{Event Hub connection string}";
        const string eventHubName = "{Event Hub name}";
        const string storageAccountName = "{storage account name}";
        const string storageAccountKey = "{storage account key}";
        */
        static string storageConnectionString = string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", storageAccountName, storageAccountKey);

        static void Main(string[] args)
        {

            string eventProcessorHostName = Guid.NewGuid().ToString();
            //messages/events - location of events in EPH
            EventProcessorHost eventProcessorHost = new EventProcessorHost
                (eventProcessorHostName, 
                "messages/events",
                "mycode" /*EventHubConsumerGroup.DefaultGroupName*/, 
                iotHubConnectionString, 
                storageConnectionString,
                "messages-events");
            Console.WriteLine("Registering EventProcessor...");
            var options = new EventProcessorOptions();
            options.ExceptionReceived += (sender, e) => { Console.WriteLine(e.Exception); };
            eventProcessorHost.RegisterEventProcessorAsync<SimpleEventProcessor>(options).Wait();

            Console.WriteLine("Receiving. Press enter key to stop worker.");
            Console.ReadLine();
            eventProcessorHost.UnregisterEventProcessorAsync().Wait();

        }
    }
}
