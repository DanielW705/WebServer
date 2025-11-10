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
        private CancellationTokenSource token = new();
        private SemaphoreSlim concurrencyControl = new(5); // controla máximo 5 clientes
        private int LastTaskEjecute = 0;
        public Server(int port = 82)
        {
            if (port > 65535 || port < 1)
            {
                port = 82;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("El puerto no es valido, se ctomara el default");
                Console.ResetColor();
            }

            try
            {
                httpSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, port);
                httpSocket.Bind(endpoint);
                httpSocket.Listen(5); // acepta hasta 100 conexiones pendientes

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Servidor iniciado en puerto {port}");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error al iniciar el servidor");
                Console.WriteLine(ex.Message);
                Console.ResetColor();
            }
        }

        public async Task HandlerAnswer(Socket client)
        {
            //Espera a que haya un espacio para hacer la peticion
            await concurrencyControl.WaitAsync();

            try
            {
                byte[] buffer = new byte[2048];
                int bytesRead = await client.ReceiveAsync(buffer);

                if (bytesRead > 0)
                {
                    Stopwatch sw = Stopwatch.StartNew();
                    string data = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    sw.Stop();

                    Console.WriteLine($"No de concurrencia: {concurrencyControl.CurrentCount}");
                    Console.WriteLine("------Request------");
                    Console.WriteLine(data);
                    Console.WriteLine("------End of Request------");
                    Console.WriteLine($"Tiempo de ejecución: {sw.ElapsedMilliseconds} ms");

                    string resHeader =
                        "HTTP/1.1 200 OK\r\n" +
                        "Server: CSharpServer\r\n" +
                        "Content-Type: text/plain; charset=UTF-8\r\n\r\n";

                    string resBody = $"Hola mundo desde servidor! Fecha: {DateTime.Now}";
                    byte[] responseData = Encoding.ASCII.GetBytes(resHeader + resBody);

                    await client.SendAsync(responseData);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error procesando cliente: {ex.Message}");
            }
            finally
            {
                client.Close();
                concurrencyControl.Release();
            }
        }

        public async Task ListeningConnection()
        {
            while (!token.Token.IsCancellationRequested)
            {
                var client = await httpSocket.AcceptAsync();
                _ = Task.Run(() => HandlerAnswer(client)); // se lanza concurrentemente
            }
        }

        public void StartServer()
        {
            serverTask = Task.Run(() => ListeningConnection());
        }

        public void StopServer()
        {
            token.Cancel();
            httpSocket.Close();
            Console.WriteLine("Servidor detenido");
        }
    }
}
