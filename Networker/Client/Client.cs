using networker._Client.Packets;
using networker._Server;
using networker._Server.Packets;
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
using System.Timers;
using static networker.Utility;

namespace networker
{
    namespace _Client
    {
        /// <summary>
        /// Represents a packet that is sent from the client to the server.
        /// </summary>
        public class IClientPacket : IPacket
        {
            /// <summary>
            /// The time the packet was received.
            /// </summary>
            public virtual long timeRecieved { get; set; }

            /// <summary>
            /// The type of the packet. Used for serialization and deserialization.
            /// </summary>
            public virtual int packetType { get; set; }

            /// <summary>
            /// The unique identifier for the packet.
            /// </summary>
            public virtual int packetID { get; set; }

            /// <summary>
            /// The time the packet was sent (UTC).
            /// </summary>
            public virtual long timeSent { get; set; }

            /// <summary>
            /// A flag indicating whether the packet was sent by the client.
            /// </summary>
            public virtual bool isClient { get; set; }

            /// <summary>
            /// Converts the packet to a string representation using JSON formatting.
            /// </summary>
            /// <returns>A JSON string representing the packet.</returns>
            public override string ToString()
            {
                return JsonConvert.SerializeObject(this, Formatting.Indented);
            }

            /// <summary>
            /// Initializes the packet with default values.
            /// </summary>
            public void __init()
            {
                timeSent = UTCTimeAsLong;
                timeRecieved = 0;
                packetID = PacketMaster.cPKTID;
                PacketMaster.cPKTID++;
                isClient = true;
            }
        }

        namespace Packets
        {
            /// <summary>
            /// Represents a client register packet (Type 1000).
            /// </summary>
            public class ClientRegisterPacket_1000 : IClientPacket
            {
                /// <summary>
                /// Initializes the packet with the packet type and sets the necessary fields.
                /// </summary>
                public ClientRegisterPacket_1000()
                {
                    packetType = 1000;
                    __init(); // Initialize packetID
                }
            }

            /// <summary>
            /// Represents a client life-check packet (Type 1001).
            /// </summary>
            public class ClientLifeCheckPacket_1001 : IClientPacket
            {
                /// <summary>
                /// Initializes the packet with the packet type and sets the necessary fields.
                /// </summary>
                public ClientLifeCheckPacket_1001()
                {
                    packetType = 1001;
                    __init(); // Initialize packetID
                }
            }
        }

        /// <summary>
        /// Represents a client that connects to a server and sends/receives packets.
        /// </summary>
        public class Client
        {
            /// <summary>
            /// Indicates whether the client is alive or not.
            /// </summary>
            public static bool alive;

            /// <summary>
            /// Gets the average ping of the client based on received packets.
            /// </summary>
            public int ping
            {
                get
                {
                    long total = 0, i = 0;
                    foreach (long a in _pingList.ToList())
                    {
                        i++;
                        total += a;
                    }
                    return (int)(total / i);
                }
            }

            private Queue<long> _pingList; // Stores the ping times for calculating average ping
            private static PacketMaster packetMaster; // The packet master used for serialization/deserialization
            private System.Timers.Timer lifeCheckTimer; // Timer for sending life check packets
            private System.Timers.Timer dQTimer; // Timer for managing ping queue
            private Queue<IClientPacket> __sendPacketQueue; // Queue of packets to be sent to the server
            private Socket __socket; // The socket connection to the server
            private Thread __recieveThread; // Thread for receiving packets
            private Thread __sendThread; // Thread for sending packets

            /// <summary>
            /// Initializes a new client and connects it to the specified server IP and port.
            /// </summary>
            /// <param name="_ip">The IP address of the server. Null=localhost. </param>
            /// <param name="port">The port number to connect to (default is 443).</param>
            public Client(IPAddress? _ip, int port = 443)
            {
                if (_ip == null) _ip = IPAddress.Parse("127.0.0.1"); // Default to local host
                __socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                try
                {
                    __socket.Connect(new IPEndPoint(_ip, port));
                }
                catch (Exception c)
                {
                    throw new ConnectionFailure(c.ToString()); // Throw an exception if connection fails
                }
                __init();
            }

            /// <summary>
            /// Adds a packet to the send queue for transmission.
            /// </summary>
            /// <param name="toQ">The packet to add to the send queue.</param>
            public void addToSendQueue(IClientPacket toQ) => __sendPacketQueue.Enqueue(toQ);

            /// <summary>
            /// Initializes the client, starting timers and threads for packet sending/receiving.
            /// </summary>
            private void __init()
            {
                lifeCheckTimer = new System.Timers.Timer(750); // Send life check packets every 750ms
                lifeCheckTimer.AutoReset = true;
                lifeCheckTimer.Elapsed += lifeCheck;
                lifeCheckTimer.Start();

                dQTimer = new System.Timers.Timer(1000); // Manage the ping queue every 1000ms
                dQTimer.AutoReset = true;
                dQTimer.Elapsed += dQ;
                dQTimer.Start();

                __sendPacketQueue = new Queue<IClientPacket>();
                addToSendQueue(new ClientRegisterPacket_1000()); // Send register packet on start

                packetMaster = new PacketMaster(true); // Initialize the packet master
                packetRecieved += handlePacket;

                _pingList = new Queue<long>(); // Initialize the ping list
                alive = true;

                __recieveThread = new Thread(recieveThread); __recieveThread.Start(); // Start receiving thread
                __sendThread = new Thread(sendThread); __sendThread.Start(); // Start sending thread
            }

            /// <summary>
            /// Sends life check packets to the server at regular intervals.
            /// </summary>
            private void lifeCheck(object? source, ElapsedEventArgs e) => addToSendQueue(new ClientLifeCheckPacket_1001());

            /// <summary>
            /// Removes outdated ping records from the ping list.
            /// </summary>
            private void dQ(object? source, ElapsedEventArgs e)
            {
                if (_pingList.Count > 0)
                {
                    long t;
                    _pingList.TryPeek(out t);
                    if (t + 1000 <= UTCTimeAsLong) // Remove records older than 1 second
                    {
                        _pingList.Dequeue();
                        dQ(null, e); // Recursive call to handle the next item
                    }
                }
            }

            /// <summary>
            /// Delegate for handling received packets.
            /// </summary>
            public delegate void packetRecievedDel(IServerPacket pac);

            /// <summary>
            /// Event triggered when a packet is received.
            /// </summary>
            public event packetRecievedDel packetRecieved;

            /// <summary>
            /// Handles the incoming server packets and calculates the ping time.
            /// </summary>
            private void handlePacket(IServerPacket pac)
            {
                pac.timeRecieved = UTCTimeAsLong;
                log("Packet recieved. Packet: " + pac.GetType().ToString());
                _pingList.Enqueue(pac.timeRecieved - pac.timeSent); // Record the ping time
                switch (pac)
                {
                    case ServerClientRegisterAck_1000 pak:
                        // Handle server acknowledgement for client registration
                        break;

                    case ServerClientLifeAck_1001 pak:
                        // Handle server acknowledgement for life check
                        break;
                }
            }

            /// <summary>
            /// Thread for sending packets to the server.
            /// </summary>
            private void sendThread()
            {
                while (alive && __socket.Connected)
                {
                    if (__sendPacketQueue.Count > 0)
                    {
                        log("sending");
                        IClientPacket toSend = __sendPacketQueue.Dequeue();
                        __socket.Send(packetMaster.formatPacketForTransmission(toSend));
                    }
                }
            }

            /// <summary>
            /// Thread for receiving packets from the server.
            /// </summary>
            private void recieveThread()
            {
                while (alive && __socket.Connected)
                {
                    byte[] __r1 = new byte[4]; // Length of the incoming packet

                    try
                    {
                        log("start recieving...");
                        __socket.Receive(__r1); // Read packet length
                        int paclength = BitConverter.ToInt32(__r1);
                        byte[] unf = new byte[paclength];
                        if (__socket.Receive(unf) == 0) { throw new RecieveFailure("Client disconnected whilst packet was being read. Too bad!"); }
                        packetRecieved?.Invoke((IServerPacket)packetMaster.unformatPacketFromTransmission(unf)); // Process the packet
                    }
                    catch (Exception exc) { log(exc.ToString()); }
                }
            }

            /// <summary>
            /// Closes the client connection.
            /// </summary>
            public void close()
            {
                __socket.Close();
                alive = false;
                dQTimer.Stop();
                lifeCheckTimer.Stop();
            }
        }
    }
}
