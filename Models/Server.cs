using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WebServer.Models
{
    public class Server
    {
        private Socket httpSocket;
        private Task serverTask;
        private CancellationTokenSource token = new CancellationTokenSource();

        public Server(int port = 82)
        {
            try
            {
                httpSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
                if (port > 65535 || port < 1)
                {
                    port = 80;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error al inciar el servidor");
                    Console.ResetColor();
                }
                IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, port);
                httpSocket.Bind(endpoint);
                httpSocket.Listen(1);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error al inciar el servidor");
                Console.WriteLine(ex.StackTrace);
                Console.ResetColor();
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Inicio correcto");
            Console.ResetColor();
        }
        public async Task ListeningConnection()
        {
            byte[] bytes = new byte[2048];
            do
            {
                var client = await httpSocket.AcceptAsync();
                var numBytes = await client.ReceiveAsync(bytes);
                if (numBytes > 0)
                {
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    var data = Encoding.ASCII.GetString(bytes, 0, numBytes);
                    stopwatch.Stop();


                    Console.WriteLine("------Request------");
                    Console.WriteLine(data);
                    Console.WriteLine("------End of Request------");
                    Console.WriteLine($"Execution Time: {stopwatch.ElapsedMilliseconds} ms");

                    string resHeader = "HTTP/1.1 200 Everything is Fine\nServer: my_csharp_server\nContent-Type: text/plain; charset: UTF-8\n\n";
                    string resBody = "Hola mundo desde servidor";

                    string resStr = resHeader + resBody;

                    byte[] resData = Encoding.ASCII.GetBytes(resStr);

                    await client.SendToAsync(resData, client.RemoteEndPoint);
                    client.Shutdown(SocketShutdown.Both);
                    client.Close();
                }
            } while (true);
        }
        public void StopServer() => token.Cancel();
        public void StartServer()
        {
            try
            {
                var factory = new TaskFactory(token.Token);
                serverTask = factory.StartNew(() => ListeningConnection());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
