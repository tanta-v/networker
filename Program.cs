using networker;
using static networker.Utility;
using networker.Packetry;
using networker.Client;
using networker.Server;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using networker.Server.Packets;
using System.Reflection;
using System.Linq;
using networker.Client.Packets;
using Newtonsoft.Json;
using networker.Packetry.Exceptions;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Runtime.ConstrainedExecution;
using System.Data.SqlTypes;
namespace networker
{
    internal class Program
    {
        static void Main(string[] args)
        {
            log($"Hello, World!");
            PacketMaster master = new PacketMaster(false);
            //Utility.Utility.log();
            master.formatPacketForTransmission(new ServerRegisterPacket_00());
        }
    }
    public static class Utility
    {
        public static long UTCTimeAsLong { get { return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(); } }
        public static string timeNowAsString { get { return DateTime.Now.ToString(); } }
        public static void log(string text)
        {
            Console.WriteLine($"{timeNowAsString}: {text}");
        }
    }
    namespace Packetry
    {
        namespace Exceptions
        {

            [Serializable]
            public class FailedtoReadSCRStrmException : Exception // failed to read server-client read stream
            {
                public FailedtoReadSCRStrmException() { }
                public FailedtoReadSCRStrmException(string message) : base(message) { }
                public FailedtoReadSCRStrmException(string message, Exception inner) : base(message, inner) { }
                protected FailedtoReadSCRStrmException(
                  System.Runtime.Serialization.SerializationInfo info,
                  System.Runtime.Serialization.StreamingContext context) { }
            }
            [Serializable]
            public class FailedToUnformatPacketException : Exception
            {
                public FailedToUnformatPacketException() { }
                public FailedToUnformatPacketException(string message) : base(message) { }
                public FailedToUnformatPacketException(string message, Exception inner) : base(message, inner) { }
                protected FailedToUnformatPacketException(
                  System.Runtime.Serialization.SerializationInfo info,
                  System.Runtime.Serialization.StreamingContext context) { }
            }
            [Serializable]
            public class IncorrectTransmissionSideException : Exception
            {
                public IncorrectTransmissionSideException() { }
                public IncorrectTransmissionSideException(string message) : base(message) { }
                public IncorrectTransmissionSideException(string message, Exception inner) : base(message, inner) { }
                protected IncorrectTransmissionSideException(
                  System.Runtime.Serialization.SerializationInfo info,
                  System.Runtime.Serialization.StreamingContext context) { }
            }

            [Serializable]
            public class InvalidPacketIDException : Exception
            {
                public InvalidPacketIDException() { }
                public InvalidPacketIDException(string message) : base(message) { }
                public InvalidPacketIDException(string message, Exception inner) : base(message, inner) { }
                protected InvalidPacketIDException(
                  System.Runtime.Serialization.SerializationInfo info,
                  System.Runtime.Serialization.StreamingContext context) { }
            }
        }
        public interface IPacket
        {
            /// <summary>
            /// The parent 'packet' interface. 
            /// Basic inheritance structure is Packet>Server/ClientPacket>PacketType(data)
            /// </summary>
            /// <value> The ID of the packet. NOT ITS TYPE. Used to identify packets in the log, keeps synchronicity across client-server. Replicated.</value>
            public int packetID { get; internal set; }
            /// <value> The UTC time that the packet was sent, defined as a 'long'. keeps track of ping. Replicated.</value>
            public long timeSent { get; internal set; }
            /// <value>
            /// If the packet is sent by the client or not
            /// </value>
            public bool isClient { get; }
        }
        /// <summary>
        /// PacketMaster is the packet handler. This is the only thing that should be used by external functions.
        /// </summary>
        public class PacketMaster
        {
            private Dictionary<Tuple<bool, int>, IPacket>? _packetDict;
            public static int _cPacketId { get; set; }
            public static bool isRunning { get; private set; }
            public static bool isClient { get; private set; }
            public PacketMaster(bool _isClient)
            {
                isClient = _isClient;
                init();
            }
            private void init()
            {
                var asm = Assembly.GetExecutingAssembly();  /// non-explicit type declarations suck
                var types = asm.GetTypes();
                var serverPackets = types
                    .Where(t => typeof(IServerPacket).IsAssignableFrom(t)
                    && !t.IsAbstract
                    && t != typeof(IServerPacket)
                    ).ToList();
                var clientPackets = types
                    .Where(t => typeof(IClientPacket).IsAssignableFrom(t)
                    && !t.IsAbstract
                    && t != typeof(IClientPacket)
                    ).ToList();
                _packetDict = new Dictionary<Tuple<bool, int>, IPacket>(); // tuple<isclient, id>
                _cPacketId = 0;
                foreach (var t in serverPackets)
                {
                    Console.WriteLine(t);
                    IServerPacket pak = (IServerPacket)Activator.CreateInstance(t);
                    _packetDict.Add(new Tuple<bool, int>(false, pak.packetID), pak);
                    Console.WriteLine(pak);
                }
                foreach (var t in clientPackets)
                {
                    Console.WriteLine(t);
                    IClientPacket pak = (IClientPacket)Activator.CreateInstance(t);
                    _packetDict.Add(new Tuple<bool, int>(true, pak.packetID), pak);
                    Console.WriteLine(pak);
                }
            }
            public byte[] formatPacketForTransmission(IPacket _pak)
            {
                if (_pak.isClient != isClient) {
                    log("Packet master tried to format a packet with incorrect client-server relation. Packet: " + _pak);
                    throw new IncorrectTransmissionSideException(); 
                }
                if (_pak.packetID == -1)
                {
                    log("Packet master tried to format a packet without an ID. Packet: " + _pak);
                    throw new InvalidPacketIDException();
                }
                string __ = _pak.ToString();
                int msglength = __.Length + 8; // data length + 2 + 4 + 2  (packet length will have already been processed)
                Console.WriteLine(msglength);
                byte[] _msglength = BitConverter.GetBytes(msglength);
                
                
                byte[] toTrsmt = new byte[msglength]; // msglength(4)||id(4)||{data}
                Buffer.BlockCopy(_msglength, 0, toTrsmt, 0, 4);
                Buffer.BlockCopy(Encoding.UTF8.GetBytes("||"), 0, toTrsmt, 4, 2);
                Buffer.BlockCopy(BitConverter.GetBytes(_pak.packetID), 0, toTrsmt, 6, 4);
                Buffer.BlockCopy(Encoding.UTF8.GetBytes("||"), 0, toTrsmt, 10, 2);
                Buffer.BlockCopy(Encoding.UTF8.GetBytes(__), 0, toTrsmt, 12, msglength - 12);
                log($"\nRe-encoded: {Encoding.UTF8.GetString(toTrsmt)}");
                log(BitConverter.ToInt32(toTrsmt.Take(4).ToArray()).ToString());
                log(BitConverter.ToInt32(toTrsmt.Skip(6).Take(4).ToArray()).ToString());
                return toTrsmt;
            }
            public IPacket unformatPacketFromTransmission(byte[] rec)
            {
                try
                {
                    // get packet class(?)
                    IPacket _pc;
                    _packetDict.TryGetValue(new Tuple<bool, int>(isClient, BitConverter.ToInt32((byte[])rec.Skip(2).Take(4))), out _pc);
                    rec = (byte[])rec.Skip(8);
                    var c = JsonConvert.DeserializeAnonymousType(Encoding.UTF8.GetString(rec), _pc.GetType());
                    return (IPacket) c;
                }
                catch (Exception e)
                {
                    throw new FailedToUnformatPacketException(e.ToString());
                }
                throw new FailedToUnformatPacketException();
            }
        }

    }
    namespace Server
    {
        public class IServerPacket: IPacket // a packet which the server sends
        {
            public virtual long timeRecieved { get; internal set; } // the UTC time that the packet was recieved, defined as a 'long'. keeps track of ping. Server sided.
            public virtual int packetID { get; set; }
            public virtual long timeSent { get; set; }
            public virtual int type { get { return -1; } }
            public bool isClient { get { return false; } }
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
            public class ServerRegisterPacket_00 : IServerPacket
            {
                public override long timeRecieved { get; internal set; }
                public override int packetID { get; set; }
                public override long timeSent { get; set; }
                public override int type { get { return 0; } }
                
                public ServerRegisterPacket_00()
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
                    packetRecieved((IClientPacket)packetMaster.unformatPacketFromTransmission(unf));
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
            public class ClientRegisterPacket_00: IClientPacket
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
