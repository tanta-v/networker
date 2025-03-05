using networker;
using static networker.Utility;
using networker.Packetry;
using networker.Client;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using networker.Server.Packets;
using System.Linq;
using networker.Client.Packets;
using Newtonsoft.Json;
using networker.Packetry.Exceptions;
using System.Net.Sockets;
using System.Net;
using System.Runtime.ConstrainedExecution;
using System.Data.SqlTypes;
namespace networker
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            log($"Hello, World!");
            PacketMaster master = new PacketMaster(false);
            //Utility.Utility.log();
            master.formatPacketForTransmission(new ServerClientRegisterAck_00());
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
    
    
}
