using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClientChat.Service
{
    public static class WebsocketClientService
    {
        static readonly string socketConnection = "ws://localhost:3000/connection/start?nickname=";

        private static ClientWebSocket Socket;
        private static CancellationTokenSource SocketLoopTokenSource;

        public static bool ActiveSocket
        {
            get; private set;
        }

        public static async Task StartAsync(string nickName)
        {
            SocketLoopTokenSource = new CancellationTokenSource();

            try
            {
                Socket = new ClientWebSocket();
                Console.WriteLine("\nEntrando no chat...");
                await Socket.ConnectAsync(new Uri(socketConnection + nickName), CancellationToken.None);

                ActiveSocket = true;

                Task.Run(async () => await Receive(Socket));

            }
            catch (WebSocketException ex)
            {
                Console.WriteLine($"\nSOCKET ERROR - {ex.Message}");
            }
        }

        public static async Task StopAsync()
        {
            Console.WriteLine($"\nSaindo do chat...\n");
            if (Socket == null || Socket.State != WebSocketState.Open) 
                return;
            await Socket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
            SocketLoopTokenSource.Cancel();
        }

        public static WebSocketState State
        {
            get => Socket?.State ?? WebSocketState.None;
        }

        private static async Task Send(ClientWebSocket socket, string data, CancellationToken stoppingToken) =>
            await socket.SendAsync(Encoding.UTF8.GetBytes(data), WebSocketMessageType.Text, true, stoppingToken);

        private static async Task Receive(ClientWebSocket socket)
        {
            bool error = false;
            var cancellationToken = SocketLoopTokenSource.Token;
            try
            {
                var buffer = WebSocket.CreateClientBuffer(4096, 4096);
                while (Socket.State != WebSocketState.Closed && !cancellationToken.IsCancellationRequested)
                {
                    //Recebe dados do servidor via socket
                    WebSocketReceiveResult result = await Socket.ReceiveAsync(buffer, cancellationToken);

                    // Se token é CANCELADO quando ReceiveAsync, o socket muda para ABORTED e não pode mais ser usado
                    if (!cancellationToken.IsCancellationRequested)
                    {
                        // O servidor do socket avisa que a conexão será fechada
                        if (Socket.State == WebSocketState.CloseReceived && result.MessageType == WebSocketMessageType.Close)
                        {
                            await Socket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "Conexão perdida com o chat", CancellationToken.None);
                        }

                        //Se a conexão está aberta processa mensagem recebida
                        if (Socket.State == WebSocketState.Open && result.MessageType != WebSocketMessageType.Close)
                        {
                            if (result.Count == 0)
                                continue;

                            //Se socket ABERTO e contém dados, imprime na tela
                            string message = Encoding.UTF8.GetString(buffer.Array, 0, result.Count);
                            if (message.Length > 1) 
                            {
                                message = "\n" + message + "\n";
                                Console.Write(message);
                            }
                        }
                    }
                };
            }
            catch (WebSocketException ex)
            {
                Console.WriteLine($"\nSOCKET ERROR - {ex.Message}");
                error = true;
            }
            finally
            {
                socket.Dispose();
                socket = null;
                SocketLoopTokenSource.Cancel();
                ActiveSocket = false;
                if (error)
                {
                    Console.WriteLine($"\nConexão perdida com o chat!\n");
                    Console.WriteLine($"Pressione ENTER para conetar novamente...\n");
                }
                else
                {
                    Console.WriteLine($"\nVocê saiu do chat!\n");
                }
            }
        }

    }
}
