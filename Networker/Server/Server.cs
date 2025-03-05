using networker.Client;
using networker.Packetry.Exceptions;
using networker.Packetry;
using Newtonsoft.Json;
using System.Net.Sockets;
using System.Net;
using static networker.Utility;
using networker.Client.Packets;

namespace networker
{
    namespace Server
    {
        public class IServerPacket : IPacket // a packet which the server sends
        {
            public virtual long timeRecieved { get; internal set; } // the UTC time that the packet was recieved, defined as a 'long'. keeps track of ping. Server sided.
            public virtual int packetType { get { return -1; } }
            public virtual int packetID { get { return -1; } }
            public virtual long timeSent { get; set; }
            public virtual int type { get { return -1; } }
            public bool isClient { get { return false; } }
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
            public class ServerClientRegisterAck_1000 : IServerPacket 
            {
                public override int packetType { get { return 1000; } }
                public ServerClientRegisterAck_1000()
                {
                    timeSent = UTCTimeAsLong;

                    __init(); // required for packetID
                }
            }
        }
        /// <summary>
        /// The server-sided representation of the client. When a client connects to the server, a new instance of this is created which handles the sending and recieving of packets from the client.
        /// </summary>
        public class ServerClient
        {
            public int ClientID;
            public IPAddress clientIP;
            private PacketMaster packetMaster;
            private Socket __socket;
            private Thread recieveThread;
            private Thread sendThread;
            private Queue<IServerPacket> __sendPacketQueue;

            public ServerClient(Socket _socket, PacketMaster _packetMaster)
            {
                __socket = _socket;
                packetMaster = _packetMaster;
                ClientID = Server.cClientID;
                Server.cClientID++;
#pragma warning disable CS8602 // Dereference of a possibly null reference. so annoying
                clientIP = (__socket.RemoteEndPoint
                            as IPEndPoint).Address;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                __sendPacketQueue = new Queue<IServerPacket>();
                recieveThread = new Thread(recieveThreadFunc); recieveThread.Start();
                sendThread = new Thread(sendThreadFunc); sendThread.Start();

                packetRecieved += handlePacket;
            }
            private void handlePacket(IClientPacket _pkt)
            {
                switch (_pkt)
                {
                    case ClientRegisterPacket_1000 pkt:
                        
                        break;
                }
            }
            private void recieveThreadFunc()
            {
                while (Server.alive && __socket.Connected)
                {
                    byte[] __r1 = new byte[4]; /// length
                    if (__socket.Receive(__r1) == 0) { throw new FailedtoReadSCRStrmException("Client disconnected during packet length reading process."); }
                    int paclength = BitConverter.ToInt32(__r1);
                    if (paclength <= 8) { throw new FailedtoReadSCRStrmException("Invalid packet length... "); }
                    log($"Packet length of {paclength} recieved. trying the rest of the packet...");
                    byte[] unf = new byte[paclength];
                    if (__socket.Receive(unf) == 0) { throw new FailedtoReadSCRStrmException("Client disconnected during packet reading process."); }
                    packetRecieved?.Invoke((IClientPacket)packetMaster.unformatPacketFromTransmission(unf));
                }
            }

            private void sendThreadFunc()
            {
                while (Server.alive && __socket.Connected)
                {
                    if (__sendPacketQueue.Count > 0)
                    {
                        IServerPacket toSend = __sendPacketQueue.Dequeue();
                        __socket.Send(packetMaster.formatPacketForTransmission(toSend));
                        log(toSend.ToString());
                    }
                }
            }
            public void addToSendQueue(IServerPacket toQ) => __sendPacketQueue.Append(toQ);
            public delegate void packetRecievedDel(IClientPacket recievedPacket);
            public event packetRecievedDel packetRecieved;
        }
        public class Server
        {
            public static int cClientID;
            public static bool alive;
            private PacketMaster packetMaster;
            private int _port;
            private Socket _socket;
            private Thread _listeningForConnectionThread;

            public Server(int port = 443)
            {
                _port = port;
                cClientID = 0;
                packetMaster = new PacketMaster(false);
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

                    }
                    catch (Exception exc)
                    {

                    }
                }
            }
        }
    }
}
