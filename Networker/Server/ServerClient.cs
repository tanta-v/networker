// Importing necessary namespaces for packet handling, networking, and timers
using networker._Client;
using networker.Packetry.Exceptions;
using networker.Packetry;
using System.Net.Sockets;
using System.Net;
using static networker.Utility;
using networker._Client.Packets;
using networker._Server.Packets;
using System.Timers;

namespace networker
{
    namespace _Server
    {
        /// <summary>
        /// Represents the server-side client. Each time a client connects to the server, an instance of this class is created to manage communication between the server and the client.
        /// </summary>
        public class ServerClient
        {
            /// <summary>
            /// Gets or sets the unique identifier for the client.
            /// </summary>
            public int ClientID;

            /// <summary>
            /// Gets or sets the endpoint representing the client’s IP address and port.
            /// </summary>
            public IPEndPoint clientEndPoint;

            /// <summary>
            /// The packet master used for handling packet transmission and formatting.
            /// </summary>
            private PacketMaster packetMaster;

            /// <summary>
            /// The socket used to communicate with the client.
            /// </summary>
            private Socket __socket;

            /// <summary>
            /// The thread responsible for receiving packets from the client.
            /// </summary>
            private Thread recieveThread;

            /// <summary>
            /// The thread responsible for sending packets to the client.
            /// </summary>
            private Thread sendThread;

            /// <summary>
            /// Queue of packets waiting to be sent to the client.
            /// </summary>
            private Queue<IServerPacket> __sendPacketQueue;

            /// <summary>
            /// Flag indicating whether the acknowledgment packet has been received.
            /// </summary>
            private bool hasRecievedAckPkt = false;

            /// <summary>
            /// Flag indicating whether the acknowledgment packet has been sent.
            /// </summary>
            private bool hasSentAckPkt = false;

            /// <summary>
            /// Timer for managing ping timeout checks.
            /// </summary>
            private System.Timers.Timer dQTimer;

            /// <summary>
            /// A queue to store ping times for the client.
            /// </summary>
            private Queue<long> _pingList;

            /// <summary>
            /// Gets the average ping time to the client.
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

            /// <summary>
            /// Closes the socket connection with the client.
            /// </summary>
            public void Close()
            {
                log($"Client at {clientEndPoint.Address}:{clientEndPoint.Port} closing...");
                __socket.Close();
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="ServerClient"/> class.
            /// </summary>
            /// <param name="_socket">The socket through which the client is connected to the server.</param>
            /// <param name="_packetMaster">The packet master used for packet formatting and handling.</param>
            public ServerClient(Socket _socket, PacketMaster _packetMaster)
            {
                __socket = _socket;
                packetMaster = _packetMaster;
                ClientID = Server.cClientID;
                Server.cClientID++;
                clientEndPoint = __socket.RemoteEndPoint as IPEndPoint;
                __sendPacketQueue = new Queue<IServerPacket>();
                recieveThread = new Thread(recieveThreadFunc); recieveThread.Start();
                sendThread = new Thread(sendThreadFunc); sendThread.Start();

                // Subscribing to packet received event
                packetRecieved += handlePacket;

                // Initializing the ping timer
                dQTimer = new System.Timers.Timer(1000);
                dQTimer.AutoReset = true;
                dQTimer.Elapsed += dQ;
                dQTimer.Start();
                _pingList = new Queue<long>();
            }

            /// <summary>
            /// Handles a received packet from the client.
            /// </summary>
            /// <param name="_pkt">The packet received from the client.</param>
            private void handlePacket(IClientPacket _pkt)
            {
                _pingList.Enqueue(_pkt.timeRecieved - _pkt.timeSent); // Calculate round-trip time
                log(_pkt.packetType);
                switch (_pkt)
                {
                    case ClientRegisterPacket_1000 pkt:
                        hasSentAckPkt = true;
                        addToSendQueue(new ServerClientRegisterAck_1000());
                        break;
                    case ClientLifeCheckPacket_1001 pkt:
                        addToSendQueue(new ServerClientLifeAck_1001());
                        break;
                }
            }

            /// <summary>
            /// The thread function that receives packets from the client.
            /// </summary>
            private void recieveThreadFunc()
            {
                while (Server.alive && __socket.Connected)
                {
                    byte[] __r1 = new byte[4]; // length of packet
                    try
                    {
                        log("start receiving...");
                        __socket.Receive(__r1); // Receive packet length
                        int paclength = BitConverter.ToInt32(__r1);
                        log($"Packet length of {paclength} received. Trying to receive the rest of the packet...");
                        byte[] unf = new byte[paclength];
                        if (__socket.Receive(unf) == 0) { throw new RecieveFailure("Client disconnected while packet was being read."); }
                        // Unformat and invoke packet received event
                        packetRecieved?.Invoke((IClientPacket)packetMaster.unformatPacketFromTransmission(unf));
                    }
                    catch (Exception exc)
                    {
                        log(exc.ToString()); // Log any exceptions
                    }
                }
            }

            /// <summary>
            /// The thread function that sends packets to the client.
            /// </summary>
            private void sendThreadFunc()
            {
                while (Server.alive && __socket.Connected)
                {
                    if (__sendPacketQueue.Count > 0)
                    {
                        IServerPacket toSend = __sendPacketQueue.Dequeue(); // Dequeue the next packet to send
                        __socket.Send(packetMaster.formatPacketForTransmission(toSend)); // Send the packet
                        log(toSend.ToString()); // Log the packet sent
                    }
                }
            }

            /// <summary>
            /// A timer event that checks and removes ping values that have exceeded a 1-second threshold.
            /// </summary>
            /// <param name="source">The source of the timer event.</param>
            /// <param name="e">The event arguments.</param>
            private void dQ(object? source, ElapsedEventArgs e)
            {
                if (_pingList.Count > 0)
                {
                    long t;
                    _pingList.TryPeek(out t); // Peek at the front of the ping list
                    if (t + 1000 <= UTCTimeAsLong) // If the ping is older than 1 second, remove it
                    {
                        _pingList.Dequeue();
                        dQ(null, e); // Recursive call to clean up further old pings
                    }
                }
            }

            /// <summary>
            /// Adds a packet to the send queue to be sent to the client.
            /// </summary>
            /// <param name="toQ">The packet to be added to the send queue.</param>
            public void addToSendQueue(IServerPacket toQ) => __sendPacketQueue.Enqueue(toQ);

            /// <summary>
            /// Delegate used to handle the received packet event.
            /// </summary>
            /// <param name="recievedPacket">The packet that was received from the client.</param>
            public delegate void packetRecievedDel(IClientPacket recievedPacket);

            /// <summary>
            /// Event triggered when a packet is received from the client.
            /// </summary>
            public event packetRecievedDel packetRecieved;
        }
    }
}
