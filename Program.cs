using networker;
using networker.Utility;
using networker.Packetry;
using networker.Client;
using networker.Server;
namespace networker
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"{Utility.Utility.timeNowAsString}: Hello, World!");
        }
    }
    namespace Utility
    {
        public static class Utility
        {
            public static long UTCTimeAsLong { get { return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(); } }
            public static string timeNowAsString { get { return DateTimeOffset.Now.ToString(); } }
        }
    }
    namespace Packetry
    {
        
        public interface IPacket
        {
            /// <summary>
            /// The parent 'packet' interface. 
            /// Basic inheritance structure is Packet>PacketType(data)
            /// </summary>
            public int packetID { get; set; } // the ID of the packet. Used to identify packets in the log, keeps synchronicity across client-server. Replicated.
            public long timeSent { get; internal set; } //the UTC time that the packet was sent, defined as a 'long'. keeps track of ping. Replicated.
        }
        public class PacketMaster
        {
            public 
        }

    }
    namespace Server
    {
        public class Server
        {
            public Server()
            {

            }
        }
    }
    namespace Client
    {
        public class Client
        {
            public Client()
            {

            }
        }
    }
}
