using System.Diagnostics;
using System.Threading;
using WebServer.Models;

public class Program
{
    public static void Main(string[] args)
    {
        Server server = new Server();
        ConsoleKeyInfo key = new ConsoleKeyInfo();
        server.StartServer();
        do
        {
            if (server.isStoped)
                server.RestartServer();

            server.OnListenigConnection();

            if (Console.KeyAvailable)
                key = Console.ReadKey(true);

        } while (key.Key != ConsoleKey.Q);

    }
}