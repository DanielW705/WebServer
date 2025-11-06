using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace WebServer.Models
{
    public class Server
    {
        private Socket httpSocket;
        private int port = 82;
        public bool isStoped = true;
        public Server(string port)
        {
            this.port = int.Parse(port);
        }
        public Server()
        {

        }
        public void OnListenigConnection()
        {
            byte[] bytes = new byte[2048];
            if (httpSocket.Poll(1000, SelectMode.SelectRead))
            {
                Socket client = httpSocket.Accept();
                client.Blocking = false;
                try
                {
                    int numBytes = client.Receive(bytes);
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

                        client.SendTo(resData, client.RemoteEndPoint);
                    }
                }
                catch (SocketException ex)
                {
                    if (ex.SocketErrorCode != SocketError.WouldBlock)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(ex.StackTrace);
                        Console.ResetColor();
                    }
                }
                Thread.Sleep(1000);
                client.Close();
                httpSocket.Close();
                isStoped = true;
            }
        }



        private void ServerConnectionTask()
        {
            try
            {
                IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, port);
                httpSocket.Bind(endpoint);
                httpSocket.Listen(1);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void RestartServer()
        {
            try
            {
                httpSocket = new Socket(SocketType.Stream, ProtocolType.Tcp) { Blocking = false };
                isStoped = false;

                ServerConnectionTask();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error al inciar el servidor");
                Console.WriteLine(ex.StackTrace);
                Console.ResetColor();
            }
        }
        public void StartServer()
        {
            try
            {
                httpSocket = new Socket(SocketType.Stream, ProtocolType.Tcp) { Blocking = false };
                isStoped = false;
                if (port > 65535 || port < 1)
                {
                    port = 80;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error al inciar el servidor");
                    Console.ResetColor();
                }
                ServerConnectionTask();
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

    }
}
