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
            public virtual int packetID { get; set; }
            public virtual long timeSent { get; set; }
            public virtual int type { get { return -1; } }
            public bool isClient { get { return true; } }
            public override string ToString()
            {
                return JsonConvert.SerializeObject(this, Formatting.Indented);
            }
            public void __init()
            {
                packetID = PacketMaster._cPacketId;
                PacketMaster._cPacketId++;
            }
        }
        namespace Packets
        {
            public class ClientRegisterPacket_00 : IClientPacket
            {
                public override long timeRecieved { get; internal set; }
                public override int packetID { get; set; }

                public override long timeSent { get; set; }

                public override int type { get { return 0; } }
                public ClientRegisterPacket_00()
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
