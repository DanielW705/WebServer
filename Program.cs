using System.Diagnostics;
using System.Threading;
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

            Stopwatch stopwatch = Stopwatch.StartNew();
            server.OnListenigConnection();
            stopwatch.Stop();
            Console.WriteLine($"Execution Time: {stopwatch.ElapsedMilliseconds} ms");
            if (Console.KeyAvailable)
                key = Console.ReadKey(true);

        } while (key.Key != ConsoleKey.Q);

    }
}