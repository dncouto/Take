﻿using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClientChat.Service
{
    public static class WebsocketClientService
    {
        private static ClientWebSocket Socket;
        private static CancellationTokenSource SocketLoopTokenSource;

        public static bool ActiveSocket
        {
            get; private set;
        }

        private static bool NotifyCloseFriendly
        {
            get; set;
        }

        public static void NotifyClose() { NotifyCloseFriendly = true;  }

        public static async Task StartAsync(string urlPort, string nickName)
        {
            string connectStringSocket = $"ws://{urlPort}/connection/start?nickname={nickName}";

            SocketLoopTokenSource = new CancellationTokenSource();

            try
            {
                Socket = new ClientWebSocket();
                Console.WriteLine("\nEntrando no chat...");
                await Socket.ConnectAsync(new Uri(connectStringSocket), CancellationToken.None);

                ActiveSocket = true;
                NotifyCloseFriendly = false;

                Task.Run(async () => await Receive(Socket));

            }
            catch (WebSocketException ex)
            {
                Console.WriteLine($"\nSOCKET ERROR - {ex.Message}");
            }
        }

        public static WebSocketState State
        {
            get => Socket?.State ?? WebSocketState.None;
        }

        private static async Task Send(ClientWebSocket socket, string data, CancellationToken stoppingToken) =>
            await socket.SendAsync(Encoding.UTF8.GetBytes(data), WebSocketMessageType.Text, true, stoppingToken);

        private static async Task Receive(ClientWebSocket socket)
        {
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
            }
            finally
            {
                socket.Dispose();
                SocketLoopTokenSource.Cancel();
                if (!NotifyCloseFriendly)
                {
                    Console.WriteLine($"\nConexão perdida com o chat!\n");
                    Console.WriteLine($"Pressione ENTER para tentar novamente...\n");
                }
                else
                {
                    Console.WriteLine($"\nVocê saiu do chat!\n");
                }
                ActiveSocket = false;
            }
        }

    }
}
