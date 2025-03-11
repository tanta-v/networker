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
        public class IClientPacket : IPacket // a packet which the client sends
        {
            public virtual long timeRecieved { get;  set; }
            public virtual int packetType { get { return -1; } }
            public virtual int packetID { get; private set; }
            public virtual long timeSent { get; set; }
            public virtual int type { get { return -1; } }
            public bool isClient { get { return true; } }
            public override string ToString()
            {
                return JsonConvert.SerializeObject(this, Formatting.Indented);
            }
            public void __init()
            {
                timeSent = UTCTimeAsLong;
                timeRecieved = 0;
                packetID = PacketMaster.cPKTID;
                PacketMaster.cPKTID++;
            }
        }
        namespace Packets
        {
            public class ClientRegisterPacket_1000 : IClientPacket
            {
                public override int packetType { get { return 1000; } }
                public ClientRegisterPacket_1000()
                {
                    __init(); // required for packetID
                }
            }
            public class ClientLifeCheckPacket_1001 : IClientPacket
            {
                public override int packetType { get { return 1001; } }
                public ClientLifeCheckPacket_1001()
                {
                    __init(); // required for packetID
                }
            }
        }
        public class Client
        {
            public static bool alive;
            public int ping { get {
                    int total = 0, i = 0;
                    foreach (Tuple<int, long> a in _pingLst.ToList<Tuple<int,long>>())
                    {
                        i++;
                        total += a.Item1;
                    }
                    return total / i;
                } }
            private Queue<Tuple<int,long>> _pingLst; 
            private static PacketMaster packetMaster;
            private System.Timers.Timer lifeCheckTimer;
            private System.Timers.Timer dQTimer;
            private bool waitingForResponse = false;
            private Queue<IClientPacket> __sendPacketQueue;
            private Socket __socket;
            private Thread __recieveThread;
            private Thread __sendThread;
            public Client(IPAddress? _ip, int port = 443)
            {
                if (_ip == null) _ip = IPAddress.Parse("127.0.0.1");
                __socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                try
                {
                    __socket.Connect(new IPEndPoint(_ip, port));
                }
                catch (Exception c)
                {
                    throw new ConnectionFailure(c.ToString());
                }
                __init();
            }
            public void addToSendQueue(IClientPacket toQ) => __sendPacketQueue.Append(toQ);
            private void __init()
            {
                lifeCheckTimer = new System.Timers.Timer(5000);
                lifeCheckTimer.AutoReset = true;
                lifeCheckTimer.Elapsed += lifeCheck;
                dQTimer = new System.Timers.Timer(1000);
                dQTimer.AutoReset = true;
                dQTimer.Elapsed += dQ;
                __sendPacketQueue = new Queue<IClientPacket>();
                addToSendQueue(new ClientRegisterPacket_1000());
                packetMaster = new PacketMaster(true);
                packetRecieved += handlePacket;
                _pingLst = new Queue<Tuple<int, long>>();
                alive = true;
                __recieveThread = new Thread(recieveThread); __recieveThread.Start();
                __sendThread = new Thread(sendThread); __sendThread.Start();
            }
            public void close()
            {

            }
            private void dQ(object? source, ElapsedEventArgs e)
            {
                if (_pingLst.Count > 0)
                {
                    Tuple<int, long> t;
                    _pingLst.TryPeek(out t);
                    if (t.Item2 + 1000 <= UTCTimeAsLong) // if packets time added to queue is greater than a second then remove from q
                    {
                        _pingLst.Dequeue();
                        dQ(null, e); //recursive
                    }
                }
            }
            private void lifeCheck(object? source, ElapsedEventArgs e)
            {
                waitingForResponse = true;
                addToSendQueue(new ClientLifeCheckPacket_1001());
            }
            public delegate void packetRecievedDel(IServerPacket pac);
            public event packetRecievedDel packetRecieved;
            private void handlePacket(IServerPacket pac)
            {
                switch (pac)
                {
                    case ServerClientRegisterAck_1000 pak:
                        
                        break;
                    case ServerClientLifeAck_1001 pak:
                        log("recieved serverclientlifeack");
                        break;
                }
            }
            private void sendThread()
            {
                log(alive.ToString());
                while (alive && __socket.Connected)
                {
                    if (__sendPacketQueue.Count != 0)
                    {
                        log("sending");
                        IClientPacket toSend = __sendPacketQueue.Dequeue();
                        __socket.Send(packetMaster.formatPacketForTransmission(toSend));
                        log(toSend.ToString());
                    }
                }
            }
            private void recieveThread()
            {
                while (alive && __socket.Connected)
                {
                    byte[] __r1 = new byte[4]; /// length

                    try { __socket.Receive(__r1); }
                    catch (SocketException exc) { close(); }
                    int paclength = BitConverter.ToInt32(__r1);
                    if (paclength <= 8) { throw new RecieveFailure("Invalid packet length... "); }
                    log($"Packet length of {paclength} recieved. trying the rest of the packet...");
                    byte[] unf = new byte[paclength];
                    if (__socket.Receive(unf) == 0) { log("Client disconnected whilst packet was being read. Too bad!"); close(); }
                    packetRecieved?.Invoke((IServerPacket)packetMaster.unformatPacketFromTransmission(unf));
                }
            }
        }
    }
}
