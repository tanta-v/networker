using networker;
using networker.Utility;
using networker.Packetry;
using networker.Client;
using networker.Server;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using networker.Server.Packets;
using System.Reflection;
using System.Linq;
using networker.Client.Packets;
namespace networker
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"{Utility.Utility.timeNowAsString} || {Utility.Utility.UTCTimeAsLong}: Hello, World!");
            PacketMaster master = new PacketMaster();
        }
    }
    namespace Utility
    {
        public static class Utility
        {
            public static long UTCTimeAsLong { get { return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(); } }
            public static string timeNowAsString { get { return DateTime.Now.ToString(); } }
        }
    }
    namespace Packetry
    {
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
            private Dictionary<Tuple<bool, int>, IPacket> _packetDict;
            public static int _cPacketId { get; private set; }
            public PacketMaster() 
            {
                init();
            }
            private void init()
            {
                var asm = Assembly.GetExecutingAssembly();  /// non-explicit type declarations suck
                var types = asm.GetTypes();
                var packets = types
                    .Where(t => typeof(IPacket).IsAssignableFrom(t) 
                    && !t.IsAbstract
                    && t != typeof(IServerPacket) && t != typeof(IClientPacket)
                    ).ToList();
                _packetDict = new Dictionary<Tuple<bool, int>, IPacket>(); // tuple<isclient, id>
                _cPacketId = 0;
                foreach (var t in packets)
                {
                    Console.WriteLine(t);
                    IServerPacket pak = (IServerPacket) Activator.CreateInstance(t);
                    _packetDict.Add(new Tuple<bool, int>(false, pak.packetID), pak);
                    Console.WriteLine(pak.packetID);
                }
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
                    timeSent = Utility.Utility.UTCTimeAsLong;
                    packetID = PacketMaster._cPacketId;
                }
            }
        }
        public class Server
        {
            public Server()
            {

            }
        }
    }
    namespace Client
    {
        namespace Packets
        {
            public class IClientPacket : IPacket // a packet which the server sends
            {
                public virtual long timeRecieved { get; internal set; } // the UTC time that the packet was recieved, defined as a 'long'. keeps track of ping. Server sided.
                public virtual int packetID { get; set; }
                public virtual long timeSent { get; set; }
                public virtual int type { get { return -1; } }
                public bool isClient { get { return true; } }
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
