﻿using networker._Client.Packets;
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
            public virtual int packetType { get; set; }
            public virtual int packetID { get; set; }
            public virtual long timeSent { get; set; }
            public virtual bool isClient { get; set; }
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
                isClient = true;
            }
        }
        namespace Packets
        {
            public class ClientRegisterPacket_1000 : IClientPacket
            {
                public ClientRegisterPacket_1000()
                {
                    packetType = 1000;
                    __init(); // required for packetID
                }
            }
            public class ClientLifeCheckPacket_1001 : IClientPacket
            {
                public ClientLifeCheckPacket_1001()
                {
                    packetType = 1001;
                    __init(); // required for packetID
                }
            }
        }
        public class Client
        {
            public static bool alive;
            public int ping { get {
                    long total = 0, i = 0;
                    foreach (long a in _pingList.ToList())
                    {
                        i++;
                        total += a;
                    }
                    return (int)(total / i);
                } }
            private Queue<long> _pingList; 
            private static PacketMaster packetMaster;
            private System.Timers.Timer lifeCheckTimer;
            private System.Timers.Timer dQTimer;
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
            public void addToSendQueue(IClientPacket toQ) => __sendPacketQueue.Enqueue(toQ);
            private void __init()
            {
                lifeCheckTimer = new System.Timers.Timer(750);
                lifeCheckTimer.AutoReset = true;
                lifeCheckTimer.Elapsed += lifeCheck;
                lifeCheckTimer.Start();
                dQTimer = new System.Timers.Timer(1000);
                dQTimer.AutoReset = true;
                dQTimer.Elapsed += dQ;
                dQTimer.Start();
                __sendPacketQueue = new Queue<IClientPacket>();
                addToSendQueue(new ClientRegisterPacket_1000());
                packetMaster = new PacketMaster(true);
                packetRecieved += handlePacket;
                _pingList = new Queue<long>();
                alive = true;
                __recieveThread = new Thread(recieveThread); __recieveThread.Start();
                __sendThread = new Thread(sendThread); __sendThread.Start();
            }
            public void close()
            {

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
            private void lifeCheck(object? source, ElapsedEventArgs e) => addToSendQueue(new ClientLifeCheckPacket_1001());
            public delegate void packetRecievedDel(IServerPacket pac);
            public event packetRecievedDel packetRecieved;
            private void handlePacket(IServerPacket pac)
            {
                pac.timeRecieved = UTCTimeAsLong;
                log("Packet recieved. Packet: " + pac.GetType().ToString());
                _pingList.Enqueue(pac.timeRecieved - pac.timeSent);
                switch (pac)
                {
                    case ServerClientRegisterAck_1000 pak:
                        
                        break;
                    case ServerClientLifeAck_1001 pak:
                        break;
                }
            }
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
            private void recieveThread()
            {
                while (alive && __socket.Connected)
                {
                    byte[] __r1 = new byte[4]; /// length

                    try
                    {
                        log("start recieving...");
                        __socket.Receive(__r1);
                        int paclength = BitConverter.ToInt32(__r1);
                        byte[] unf = new byte[paclength];
                        if (__socket.Receive(unf) == 0) { throw new RecieveFailure("Client disconnected whilst packet was being read. Too bad!"); }
                        packetRecieved?.Invoke((IServerPacket)packetMaster.unformatPacketFromTransmission(unf));
                    }
                    catch (Exception exc) { log(exc.ToString()); }
                }
            }
        }
    }
}
