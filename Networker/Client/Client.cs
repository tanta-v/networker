using networker.Packetry;
using networker.Packetry.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using static networker.Utility;
namespace networker
{
    namespace _Client
    {
        public class IClientPacket : IPacket // a packet which the client sends
        {
            public virtual long timeRecieved { get;  set; }
            public virtual int packetType { get { return -1; } }
            public virtual int packetID { get; private set; }
            public virtual long timeSent { get; set; }
            public virtual int type { get { return -1; } }
            public bool isClient { get { return true; } }
            public override string ToString()
            {
                return JsonConvert.SerializeObject(this, Formatting.Indented);
            }
            public void __init()
            {
                timeSent = UTCTimeAsLong;
                timeRecieved = 0;
                packetID = PacketMaster.cPKTID;
                PacketMaster.cPKTID++;
            }
        }
        namespace Packets
        {
            public class ClientRegisterPacket_1000 : IClientPacket
            {
                public override int packetType { get { return 1000; } }
                public ClientRegisterPacket_1000()
                {
                    __init(); // required for packetID
                }
            }
        }
        public class Client
        {
            public static bool alive;
            private Socket __socket;
            public Client(IPAddress? _ip, int port = 443)
            {
                if (_ip == null)
                    _ip = IPAddress.Parse("127.0.0.1");
                __socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                try
                {
                    __socket.Connect(new IPEndPoint(_ip, port));
                } 
                catch (Exception c)
                {
                    throw new ConnectionFailure(c.ToString());
                }
            }
        }
    }
}
