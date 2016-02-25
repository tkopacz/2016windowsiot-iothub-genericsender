using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common;

namespace RegisterDevice
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Usage: \r\n" +
                              "RegisterDevice <iotHubOwner> <deviceName>" );
            string connection = args[0];
            string deviceName = args[1];
            RegistryManager rm = RegistryManager.CreateFromConnectionString(connection);
            Device d = new Device(deviceName);
            rm.AddDeviceAsync(d).Wait();
            d = rm.GetDeviceAsync(deviceName).Result;
            Console.WriteLine(d.Id);
            Console.WriteLine(d.Authentication.SymmetricKey.PrimaryKey);

        }
    }
}
