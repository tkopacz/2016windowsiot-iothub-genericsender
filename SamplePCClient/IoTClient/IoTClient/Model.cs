using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTClient
{
    public class MIoTBase
    {
        public DateTime Dt { get; } = DateTime.Now;
        public string MsgType { get; set; }
        public string DeviceName { get; set; }
    }
    public class MMsg1:MIoTBase
    {
        public int MyProperty1 { get; set; }
    }

    public class MMsg2:MIoTBase
    {
        public string MyProperty2 { get; set; }
        public double MyVal2 { get; set; }
    }

    public class MError //No Dt
    {
        public string MsgType { get; set; }
        public string DeviceName { get; set; }
    }
}
