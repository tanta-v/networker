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

            private System.Timers.Timer dQTimer;
            private Queue<long> _pingList;
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

                dQTimer = new System.Timers.Timer(1000);
                dQTimer.AutoReset = true;
                dQTimer.Elapsed += dQ;
                dQTimer.Start();
                _pingList = new Queue<long>();
            }
            private void handlePacket(IClientPacket _pkt)
            {
                _pingList.Enqueue(_pkt.timeRecieved - _pkt.timeSent);
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
            private void recieveThreadFunc()
            {
                while (Server.alive && __socket.Connected)
                {
                    
                    byte[] __r1 = new byte[4]; /// length
                    try {
                        log("start recieving...");
                        __socket.Receive(__r1);
                        int paclength = BitConverter.ToInt32(__r1);
                        log($"Packet length of {paclength} recieved. trying the rest of the packet...");
                        byte[] unf = new byte[paclength];
                        if (__socket.Receive(unf) == 0) { throw new RecieveFailure("Client disconnected whilst packet was being read. Too bad!"); }
                        packetRecieved?.Invoke((IClientPacket)packetMaster.unformatPacketFromTransmission(unf));
                    }
                    catch (Exception exc) { log(exc.ToString()); }
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
            private void dQ(object? source, ElapsedEventArgs e)
            {
                if (_pingList.Count > 0)
                {
                    long t;
                    _pingList.TryPeek(out t);
                    if (t + 1000 <= UTCTimeAsLong) // if packets time added to queue is greater than a second then remove from q
                    {
                        _pingList.Dequeue();
                        dQ(null, e); //recursive
                    }
                }
            }
            public void addToSendQueue(IServerPacket toQ) => __sendPacketQueue.Enqueue(toQ);
            public delegate void packetRecievedDel(IClientPacket recievedPacket);
            public event packetRecievedDel packetRecieved;
        }
    }
}
