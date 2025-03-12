using networker.Packetry;
using Newtonsoft.Json;
using System.Net.Sockets;
using System.Net;
using static networker.Utility;
using networker.Packetry.Exceptions;

namespace networker
{
    namespace _Server
    {
        public class IServerPacket : IPacket // a packet which the server sends
        {
            public virtual long timeRecieved { get; set; }
            public virtual int packetType { get; set; }
            public virtual int packetID { get; set; }
            public virtual long timeSent { get; set; }
            public virtual bool isClient { get; set; }
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
                isClient = false;
            }
        }
        namespace Packets
        {
            public class ServerClientRegisterAck_1000 : IServerPacket 
            {
                public ServerClientRegisterAck_1000()
                {
                    packetType = 1000;
                    __init();
                }
            }

            public class ServerClientLifeAck_1001 : IServerPacket
            {
                public ServerClientLifeAck_1001()
                {
                    packetType = 1001;
                    __init();
                }
            }
        }
        public class Server
        {
            public static int cClientID;
            public static bool alive;
            public List<ServerClient> ClientList;
            private PacketMaster packetMaster;
            private int _port;
            private Socket _socket;
            private Thread _listeningForConnectionThread;
            public Server(int port = 443)
            {
                _port = port;
                cClientID = 0;
                packetMaster = new PacketMaster(false);
                ClientList = new List<ServerClient>();
                __init();
            }

            private void __init()
            {
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _socket.Bind(new IPEndPoint(IPAddress.Any, _port));
                _socket.Listen(22);
                alive = true;
                log($"Server opened on port {_port}...");
                _listeningForConnectionThread = new Thread(__listenForConnections); _listeningForConnectionThread.Start();
            }
            private void __listenForConnections()
            {
                while (alive)
                {
                    try
                    {
                        Socket soc = _socket.Accept();
                        IPEndPoint _IEP = (IPEndPoint)soc.RemoteEndPoint;
                        log("New client has been registered at " + _IEP.Address + ":" + _IEP.Port + ".");
                        ClientList.Add(new ServerClient(soc, packetMaster));
                    }
                    catch (Exception exc)
                    {
                        throw new ConnectionFailure(exc.ToString());
                    }
                }
            }
        }
    }
}
