using networker._Client;
using networker.Packetry.Exceptions;
using networker.Packetry;
using System.Net.Sockets;
using System.Net;
using static networker.Utility;
using networker._Client.Packets;
using networker._Server.Packets;

namespace networker
{
    namespace _Server
    {
        /// <summary>
        /// The server-sided representation of the client. When a client connects to the server, a new instance of this is created which handles the sending and recieving of packets from the client.
        /// </summary>
        public class ServerClient
        {
            public int ClientID;
            public IPEndPoint clientEndPoint;
            private PacketMaster packetMaster;
            private Socket __socket;
            private Thread recieveThread;
            private Thread sendThread;
            private Queue<IServerPacket> __sendPacketQueue;

            private bool hasRecievedAckPkt = false;
            private bool hasSentAckPkt = false;

            public void Close()
            {
                log($"Client at {clientEndPoint.Address}:{clientEndPoint.Port} closing...");
                __socket.Close();
            }
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

                packetRecieved += handlePacket;
            }
            private void handlePacket(IClientPacket _pkt)
            {
                switch (_pkt)
                {
                    case ClientRegisterPacket_1000 pkt:
                        hasSentAckPkt = true;
                        addToSendQueue(new ServerClientRegisterAck_1000());
                        break;
                }
            }
            private void recieveThreadFunc()
            {
                while (Server.alive && __socket.Connected)
                {
                    byte[] __r1 = new byte[4]; /// length

                    try { __socket.Receive(__r1); }
                    catch (SocketException exc) { Close(); }
                    int paclength = BitConverter.ToInt32(__r1);
                    if (paclength <= 8) { throw new RecieveFailure("Invalid packet length... "); }
                    log($"Packet length of {paclength} recieved. trying the rest of the packet...");
                    byte[] unf = new byte[paclength];
                    if (__socket.Receive(unf) == 0) { log("Client disconnected whilst packet was being read. Too bad!"); Close(); }
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
    }
}
