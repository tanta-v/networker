using networker.Packetry;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static networker.Utility;
namespace networker
{
    namespace Client
    {
        public class IClientPacket : IPacket // a packet which the client sends
        {
            public virtual long timeRecieved { get; internal set; } // the UTC time that the packet was recieved, defined as a 'long'. keeps track of ping. Server sided.
            public virtual int packetType { get { return -1; } }
            public virtual int packetID { get { return -1; } }
            public virtual long timeSent { get; set; }
            public virtual int type { get { return -1; } }
            public bool isClient { get { return true; } }
            public override string ToString()
            {
                return JsonConvert.SerializeObject(this, Formatting.Indented);
            }
            public void __init()
            {

            }
        }
        namespace Packets
        {
            public class ClientRegisterPacket_1000 : IClientPacket
            {
                public override int packetType { get { return 1000; } }
                public ClientRegisterPacket_1000()
                {
                    timeSent = UTCTimeAsLong;

                    __init(); // required for packetID
                }

            }
        }
        public class Client
        {
            public Client()
            {

            }
        }
    }
}
