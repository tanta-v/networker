using static networker.Utility;
using networker.Packetry;
using networker._Server;
using networker._Client;
namespace networker
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            log($"Please press s (to start a server) or c (to start a client)...");
            switch (Console.ReadKey().Key)
            {
                case ConsoleKey.S:
                    Server s = new Server();
                    break;
                case ConsoleKey.C:
                    Client c = new Client(null);
                    break;
                default:
                    throw new Exception("no");
            }
        }
    }    
}
