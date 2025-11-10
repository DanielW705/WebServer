using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using WebServer.Models;

public class Program
{
    public static void Main(string[] args)
    {
        Server server = new Server();
        server.StartServer();
        ConsoleKeyInfo key = new ConsoleKeyInfo();
        do
        {
            if (Console.KeyAvailable)
                key = Console.ReadKey(true);

        } while (key.Key != ConsoleKey.Q);
        server.StopServer();
    }
}