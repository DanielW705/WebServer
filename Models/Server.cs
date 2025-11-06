using System.Net;
using System.Net.Sockets;
using System.Text;

namespace WebServer.Models
{
    public class Server
    {
        private Socket httpSocket;
        private int port = 82;
        private Task serverTask;

        private Timer timer = new Timer();

        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public bool isStoped
        {
            get
            {
                return cancellationTokenSource.IsCancellationRequested;
            }
        }

        public Server(string port)
        {
            this.port = int.Parse(port);
        }
        public Server()
        {

        }
        public void OnListenigConnection()
        {
            DateTime time = DateTime.Now;

            string data = "";

            byte[] bytes = new byte[2048];

            Socket client = httpSocket.Accept();
            int numBytes = client.Receive(bytes);
            if (numBytes > 0)
            {
                data += Encoding.ASCII.GetString(bytes, 0, numBytes);


                Console.WriteLine("------Request------");
                Console.WriteLine(data);
                Console.WriteLine("------End of Request------");

                string resHeader = "HTTP/1.1 200 Everything is Fine\nServer: my_csharp_server\nContent-Type: text/plain; charset: UTF-8\n\n";
                string resBody = "Hola mundo desde servidor";

                string resStr = resHeader + resBody;

                byte[] resData = Encoding.ASCII.GetBytes(resStr);

                client.SendTo(resData, client.RemoteEndPoint);

                client.Close();
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
        public void StartServer()
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
