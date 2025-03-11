using static networker.Utility;
using networker._Client;
using System.Reflection;
using Newtonsoft.Json;
using networker.Packetry.Exceptions;
using System.Text;
using networker._Server;
namespace networker
{
    namespace Packetry
    {
        /// <summary>
        /// PacketMaster is the packet handler. This is the only thing that should be used by external functions.
        /// </summary>
        public class PacketMaster
        {
            private Dictionary<Tuple<bool, int>, IPacket>? _packetDict;
            public static bool isRunning { get; private set; }
            public static bool isClient { get; private set; }
            public static int cPKTID;
            public PacketMaster(bool _isClient)
            {
                isClient = _isClient;
                init();
            }
            private void init()
            {
                cPKTID = 0;
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
                foreach (var t in serverPackets)
                {
                    Console.WriteLine(t);
                    IServerPacket pak = (IServerPacket)Activator.CreateInstance(t);
                    _packetDict.Add(new Tuple<bool, int>(false, pak.packetType), pak);
                    Console.WriteLine(pak);
                }
                foreach (var t in clientPackets)
                {
                    Console.WriteLine(t);
                    IClientPacket pak = (IClientPacket)Activator.CreateInstance(t);
                    _packetDict.Add(new Tuple<bool, int>(true, pak.packetType), pak);
                    Console.WriteLine(pak);
                }
            }
            public byte[] formatPacketForTransmission(IPacket _pak)
            {
                if (_pak.isClient != isClient)
                    throw new FormatFailure("Packet master tried to format a packet with incorrect client-server relation. Packet: " + _pak);
                if (_pak.packetType == -1)
                    throw new FormatFailure("Packet master tried to format a packet without an ID. Packet: " + _pak);
                string __ = _pak.ToString();
                if (__ != null)
                {
                    int msglength = __.Length + 8; // data length + 2 + 4 + 2  (packet length will have already been processed)
                    log(msglength.ToString());
                    byte[] _msglength = BitConverter.GetBytes(msglength);


                    byte[] toTrsmt = new byte[msglength]; // msglength(4)||id(4)||{data}
                    Buffer.BlockCopy(_msglength, 0, toTrsmt, 0, 4);
                    Buffer.BlockCopy(Encoding.UTF8.GetBytes("||"), 0, toTrsmt, 4, 2);
                    Buffer.BlockCopy(BitConverter.GetBytes(_pak.packetType), 0, toTrsmt, 6, 4);
                    Buffer.BlockCopy(Encoding.UTF8.GetBytes("||"), 0, toTrsmt, 10, 2);
                    Buffer.BlockCopy(Encoding.UTF8.GetBytes(__), 0, toTrsmt, 12, msglength - 12);
                    
                    return toTrsmt;
                }
                throw new FormatFailure();
            }
            public IPacket unformatPacketFromTransmission(byte[] rec)
            {
                try
                {
                    // get packet class(?)
                    IPacket _pc;
                    _packetDict.TryGetValue(new Tuple<bool, int>(isClient, BitConverter.ToInt32((byte[])rec.Skip(2).Take(4))), out _pc);
                    rec = (byte[])rec.Skip(8);
                    IPacket pkt = (IPacket)JsonConvert.DeserializeAnonymousType(Encoding.UTF8.GetString(rec), _pc.GetType());
                    pkt.timeRecieved = UTCTimeAsLong;
                    return pkt;
                }
                catch (Exception e)
                {
                    throw new UnformatFailure(e.ToString());
                }
                throw new UnformatFailure();
            }
        }

    }
}
