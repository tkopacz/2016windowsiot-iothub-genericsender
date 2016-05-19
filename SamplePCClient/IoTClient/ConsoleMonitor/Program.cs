using Microsoft.Azure.Devices.Common;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleMonitor
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Usage: \r\n" +
                              "ConsoleMonitor <iotHubOwner> [<consumerGroup>] [<device>] ");
            string connection = args[0];

            string consumerGroupName = "$Default";
            if (args.Length>1) consumerGroupName = args[1];
            string deviceName = "";
            if (args.Length>2) deviceName = args[2];
            EventHubClient eventHubClient = null;
            EventHubReceiver eventHubReceiver = null;

            eventHubClient = EventHubClient.CreateFromConnectionString(connection, "messages/events");
            var ri = eventHubClient.GetRuntimeInformation();
            if (deviceName != "")
            {
                string partition = EventHubPartitionKeyResolver.ResolveToPartition(deviceName, ri.PartitionCount);
                eventHubReceiver = eventHubClient.GetConsumerGroup(consumerGroupName).
                    CreateReceiver(partition, DateTime.Now);
                Task.Run(() => eventLoop(eventHubReceiver));
            } else
            {
                EventHubReceiver[] eventHubReceivers = new EventHubReceiver[ri.PartitionCount];
                    
                    
                int i = 0;
                foreach (var partition in ri.PartitionIds)
                {
                    eventHubReceivers[i] = eventHubClient.GetConsumerGroup(consumerGroupName).CreateReceiver(partition, DateTime.Now);
                    //Task.Run(() => eventLoop(eventHubReceivers[i])); <- very common bug!
                    var r = eventHubReceivers[i];
                    Task.Run(() => eventLoop(r));
                    i++;
                }

            }
            Console.ReadLine();
        }

        private static async Task eventLoop(EventHubReceiver eventHubReceiver)
        {
            while (true)
            {
                var edata = await eventHubReceiver.ReceiveAsync();
                if (edata != null)
                {
                    var data = Encoding.UTF8.GetString(edata.GetBytes());
                    Console.WriteLine(data);
                }
            }
        }
    }
}
