// Importing necessary namespaces for packet handling, JSON serialization, and networking
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
        /// <summary>
        /// Represents a packet sent by the server (inherits from IPacket).
        /// </summary>
        public class IServerPacket : IPacket
        {
            /// <summary>
            /// Gets or sets the time the packet was received.
            /// </summary>
            public virtual long timeRecieved { get; set; }

            /// <summary>
            /// Gets or sets the packet type.
            /// </summary>
            public virtual int packetType { get; set; }

            /// <summary>
            /// Gets or sets the packet ID.
            /// </summary>
            public virtual int packetID { get; set; }

            /// <summary>
            /// Gets or sets the time the packet was sent.
            /// </summary>
            public virtual long timeSent { get; set; }

            /// <summary>
            /// Gets or sets a boolean indicating if the packet is from a client.
            /// </summary>
            public virtual bool isClient { get; set; }

            /// <summary>
            /// Returns a JSON string representation of the packet.
            /// </summary>
            /// <returns>A JSON string representing the packet.</returns>
            public override string ToString()
            {
                return JsonConvert.SerializeObject(this, Formatting.Indented);
            }

            /// <summary>
            /// Initializes the packet with default values for time, packet ID, and client status.
            /// </summary>
            public void __init()
            {
                timeSent = UTCTimeAsLong; // Set the time sent to the current UTC time in long format
                timeRecieved = 0; // Time received is initially set to 0
                packetID = PacketMaster.cPKTID; // Set the packet ID from the packet master
                PacketMaster.cPKTID++; // Increment the packet ID for future packets
                isClient = false; // Indicate that the packet is from the server, not the client
            }
        }

        namespace Packets
        {
            /// <summary>
            /// Acknowledgment packet sent when the client is registered (Packet type 1000).
            /// </summary>
            public class ServerClientRegisterAck_1000 : IServerPacket
            {
                /// <summary>
                /// Initializes the packet with packet type 1000 and default values.
                /// </summary>
                public ServerClientRegisterAck_1000()
                {
                    packetType = 1000; // Set packet type to 1000 (Server Client Register Acknowledgment)
                    __init(); // Initialize the packet with default values
                }
            }

            /// <summary>
            /// Acknowledgment packet sent when the client's life is acknowledged (Packet type 1001).
            /// </summary>
            public class ServerClientLifeAck_1001 : IServerPacket
            {
                /// <summary>
                /// Initializes the packet with packet type 1001 and default values.
                /// </summary>
                public ServerClientLifeAck_1001()
                {
                    packetType = 1001; // Set packet type to 1001 (Server Client Life Acknowledgment)
                    __init(); // Initialize the packet with default values
                }
            }
        }

        /// <summary>
        /// Represents the server responsible for handling client connections and communication.
        /// </summary>
        public class Server
        {
            /// <summary>
            /// Static client ID counter to track unique client IDs.
            /// </summary>
            public static int cClientID;

            /// <summary>
            /// Indicates whether the server is alive (running).
            /// </summary>
            public static bool alive;

            /// <summary>
            /// List of connected clients.
            /// </summary>
            public List<ServerClient> ClientList;

            /// <summary>
            /// The packet master used for creating and managing packets.
            /// </summary>
            private PacketMaster packetMaster;

            /// <summary>
            /// The port on which the server listens for connections.
            /// </summary>
            private int _port;

            /// <summary>
            /// The server socket used for accepting client connections.
            /// </summary>
            private Socket _socket;

            /// <summary>
            /// The thread that listens for incoming client connections.
            /// </summary>
            private Thread _listeningForConnectionThread;

            /// <summary>
            /// Initializes a new instance of the <see cref="Server"/> class with a specified port.
            /// </summary>
            /// <param name="port">The port on which the server will listen for connections. Default is 443.</param>
            public Server(int port = 443)
            {
                _port = port; // Set the port for the server to listen on
                cClientID = 0; // Initialize the client ID counter
                packetMaster = new PacketMaster(false); // Create a packet master instance
                ClientList = new List<ServerClient>(); // Initialize the client list
                __init(); // Call the private initialization method
            }

            /// <summary>
            /// Initializes the server's socket and starts listening for client connections.
            /// </summary>
            private void __init()
            {
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); // Create a TCP socket
                _socket.Bind(new IPEndPoint(IPAddress.Any, _port)); // Bind the socket to any IP address and the given port
                _socket.Listen(22); // Start listening with a maximum of 22 connections in the queue
                alive = true; // Mark the server as alive
                log($"Server opened on port {_port}..."); // Log that the server has started
                _listeningForConnectionThread = new Thread(__listenForConnections); // Create a new thread for listening for connections
                _listeningForConnectionThread.Start(); // Start the connection listener thread
            }

            /// <summary>
            /// Sends a packet to a specific client using a <see cref="ServerClient"/> instance.
            /// </summary>
            /// <param name="scInst">The instance of <see cref="ServerClient"/> representing the client.</param>
            /// <param name="toSend">The packet to send to the client.</param>
            public void sendToClient(ServerClient scInst, IServerPacket toSend) => scInst.addToSendQueue(toSend);

            /// <summary>
            /// Sends a packet to a specific client by client ID.
            /// </summary>
            /// <param name="clientID">The client ID of the client to send the packet to.</param>
            /// <param name="toSend">The packet to send to the client.</param>
            public void sendToClient(int clientID, IServerPacket toSend) => sendToClient(getClient(clientID), toSend);

            /// <summary>
            /// Sends a packet to a specific client by IP address.
            /// </summary>
            /// <param name="iP">The IP address of the client to send the packet to.</param>
            /// <param name="toSend">The packet to send to the client.</param>
            public void sendToClient(IPAddress iP, IServerPacket toSend) => sendToClient(getClient(iP), toSend);

            /// <summary>
            /// Retrieves a client from the client list by their client ID.
            /// </summary>
            /// <param name="clientID">The client ID of the client to retrieve.</param>
            /// <returns>The <see cref="ServerClient"/> associated with the given client ID.</returns>
            public ServerClient getClient(int clientID) => (ServerClient)ClientList.Where(t => t.ClientID == clientID);

            /// <summary>
            /// Retrieves a client from the client list by their IP address.
            /// </summary>
            /// <param name="iP">The IP address of the client to retrieve.</param>
            /// <returns>The <see cref="ServerClient"/> associated with the given IP address.</returns>
            public ServerClient getClient(IPAddress iP) => (ServerClient)ClientList.Where(t => t.clientEndPoint.Address == iP);

            /// <summary>
            /// Listens for incoming client connections in a separate thread.
            /// </summary>
            private void __listenForConnections()
            {
                while (alive) // Keep listening while the server is alive
                {
                    try
                    {
                        Socket soc = _socket.Accept(); // Accept an incoming client connection
                        IPEndPoint _IEP = (IPEndPoint)soc.RemoteEndPoint; // Get the remote endpoint (client IP and port)
                        log("New client has been registered at " + _IEP.Address + ":" + _IEP.Port + "."); // Log the new client connection
                        ClientList.Add(new ServerClient(soc, packetMaster)); // Add the new client to the client list
                    }
                    catch (Exception exc) // Catch any exception that occurs while accepting a connection
                    {
                        throw new ConnectionFailure(exc.ToString()); // Throw a custom exception if connection fails
                    }
                }
            }
        }
    }
}
