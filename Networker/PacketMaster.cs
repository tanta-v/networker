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
        /// PacketMaster is the central packet handler.
        /// This class is responsible for formatting, unformatting, and managing packets.
        /// It should be the only component interacting with packets externally.
        /// </summary>
        public class PacketMaster
        {
            // Dictionary to store packets based on client/server status and packet type.
            private Dictionary<Tuple<bool, int>, IPacket>? _packetDict;

            // Static properties to track client/server and packet status
            public static bool isRunning { get; private set; }
            public static bool isClient { get; private set; }
            public static int cPKTID;

            // Constructor to initialize the PacketMaster for either client or server mode
            public PacketMaster(bool _isClient)
            {
                isClient = _isClient;  // Set the client/server mode
                init();  // Initialize the packet dictionary
            }

            // Initializes the packet dictionary and registers packet types for client and server
            private void init()
            {
                var asm = Assembly.GetExecutingAssembly();  // Get the current executing assembly
                var types = asm.GetTypes();  // Get all types in the assembly
                var serverPackets = types
                    .Where(t => typeof(IServerPacket).IsAssignableFrom(t) && !t.IsAbstract && t != typeof(IServerPacket))
                    .ToList();
                var clientPackets = types
                    .Where(t => typeof(IClientPacket).IsAssignableFrom(t) && !t.IsAbstract && t != typeof(IClientPacket))
                    .ToList();
                _packetDict = new Dictionary<Tuple<bool, int>, IPacket>();  // Initialize the packet dictionary
                foreach (var t in serverPackets)
                {
                    IServerPacket pak = (IServerPacket)Activator.CreateInstance(t);  // Create an instance of the server packet
                    _packetDict.Add(new Tuple<bool, int>(false, pak.packetType), pak);  // Add to dictionary
                }
                foreach (var t in clientPackets)
                {
                    IClientPacket pak = (IClientPacket)Activator.CreateInstance(t);  // Create an instance of the client packet
                    _packetDict.Add(new Tuple<bool, int>(true, pak.packetType), pak);  // Add to dictionary
                }
                foreach (Tuple<bool, int> f in _packetDict.Keys)
                {
                    IPacket? a = null;
                    _packetDict.TryGetValue(f, out a);
                    log($@"{f.Item1}, {f.Item2}, {a.GetType()}");
                }
                cPKTID = 0;  // Reset the packet ID counter, so the .createinstances don't add to it.
            }

            // Formats a packet for transmission (serializes it and adds header information)
            public byte[] formatPacketForTransmission(IPacket _pak)
            {
                if (_pak.isClient != isClient)
                    throw new FormatFailure("Packet master tried to format a packet with incorrect client-server relation. Packet: " + _pak);
                if (_pak.packetType == -1)
                    throw new FormatFailure("Packet master tried to format a packet without an ID. Packet: " + _pak);
                if (!_packetDict.ContainsKey(new Tuple<bool, int>(isClient, _pak.packetType)))
                    throw new FormatFailure("Packet master tried to format a packet with an unregistered ID. Packet: " + _pak);
                string __ = _pak.ToString();
                if (__ != null)
                {
                    int msglength = __.Length + 12;  // data length + 2 + 4 + 2 (packet length already handled)
                    byte[] _msglength = BitConverter.GetBytes(msglength);  // Convert length to bytes
                    byte[] toTrsmt = new byte[msglength];  // msglength(4) || id(4) || {data}
                    Buffer.BlockCopy(_msglength, 0, toTrsmt, 0, 4);  // Copy message length to the start (4 bytes)
                    Buffer.BlockCopy(Encoding.UTF8.GetBytes("||"), 0, toTrsmt, 4, 2);  // Copy separator "||" (2 bytes)
                    Buffer.BlockCopy(BitConverter.GetBytes(_pak.packetType), 0, toTrsmt, 6, 4);  // Copy packet type (4 bytes)
                    Buffer.BlockCopy(Encoding.UTF8.GetBytes("||"), 0, toTrsmt, 10, 2);  // Copy separator "||" (2 bytes)
                    Buffer.BlockCopy(Encoding.UTF8.GetBytes(__), 0, toTrsmt, 12, __.Length);  // Copy actual data
                    return toTrsmt;  // Return the formatted byte array
                }
                throw new FormatFailure();
            }

            // Unformats a received packet, converting it from a byte array to the correct packet type
            public IPacket unformatPacketFromTransmission(byte[] rec)
            {
                if (rec.Count() == 0) throw new UnformatFailure("Empty packet received.");
                try
                {
                    IPacket _pc;
                    int a = BitConverter.ToInt32(rec.Skip(2).Take(4).ToArray());  // Extract packet ID (4 bytes) from index 2
                    _packetDict.TryGetValue(new Tuple<bool, int>(!isClient, a), out _pc);  // Fetch packet class from dictionary
                    rec = rec.Skip(8).ToArray();  // Skip 8 bytes (header info)
                    IPacket pkt = (IPacket)JsonConvert.DeserializeObject(Encoding.UTF8.GetString(rec), _pc.GetType());  // Deserialize packet
                    pkt.timeRecieved = UTCTimeAsLong;  // Set the received time for the packet
                    return pkt;  // Return the deserialized packet
                }
                catch (Exception e)
                {
                    throw new UnformatFailure(e.ToString());  // Catch and throw unformat failure
                }
                throw new UnformatFailure();  // General unformat failure. Not sure how you got here.
            }
        }
    }
}
