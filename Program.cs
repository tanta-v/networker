namespace networker
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
        }
    }
    public struct Packet
    {
        public int packetID
        {
            get; private set;
        }
        public Packet(int _packetID) 
        {
            packetID = _packetID;
        }
    }
    namespace Server
    {
        public class Server
        {
            public Server()
            {

            }
        }
    }
    namespace Client
    {
        public class Client
        {
            public Client()
            {

            }
        }
    }
}
