using Microsoft.AspNetCore.Http;
using ProvaTake.Data;
using ProvaTake.Service.Intf;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProvaTake.Service
{
    public class ConnectionManagerService : IConnectionManagerService
    {
        public IEnumerable<TopicArea> GetAllTopicAreas()
        {
            return new List<TopicArea>
            {
                new TopicArea {Name =".NET Core" },
                new TopicArea {Name ="Docker" },
                new TopicArea { Name ="C#" }
            };
        }


        private ConcurrentDictionary<string, WebSocket> _sockets = new ConcurrentDictionary<string, WebSocket>();

        public async Task ConnectChat(WebSocketManager webSocketManager, string nickName)
        {
            WebSocket webSocket = await webSocketManager.AcceptWebSocketAsync();

            if (!_sockets.TryAdd(nickName, webSocket))
            {
                await DisconnectChat(nickName);
                _sockets.AddOrUpdate(nickName, webSocket, (key, oldValue) => webSocket);
            }

            await SendMessageEveryOne($"\n{nickName} entrou na sala...\n");

            PingClient(webSocket, nickName);
        }

        public async Task DisconnectChat(string nickName)
        {
            WebSocket socket;
            if (_sockets.TryRemove(nickName, out socket)) 
            {
                if (socket.State == WebSocketState.Open)
                {
                    await socket.CloseAsync(closeStatus: WebSocketCloseStatus.NormalClosure,
                                            statusDescription: "Closed by the ConnectionManager",
                                            cancellationToken: CancellationToken.None);
                }
                await SendMessageEveryOne($"\n{nickName} saiu da sala!\n");
            }
        }

        public async Task SendMessageEveryOne(string message)
        {
            await SendMessageEveryOne(message, null);
        }

        public async Task SendMessageEveryOne(string message, string nickName)
        {
            ConcurrentDictionary<string, WebSocket> allUsers = GetAllSockets();
            foreach (WebSocket webSocket in allUsers.Values)
            {
                await SendMessageToClient(webSocket, message);
            }
        }

        public async Task SendMessagePrivate(string nickName, string message)
        {
            WebSocket webSocket = GetSocketByNickName(nickName);
            await SendMessageToClient(webSocket, message);
        }

        public WebSocket GetSocketByNickName(string nickName)
        {
            return _sockets.FirstOrDefault(p => p.Key == nickName).Value;
        }

        public ConcurrentDictionary<string, WebSocket> GetAllSockets()
        {
            return _sockets;
        }

        private async Task SendMessageToClient(WebSocket webSocket, string message)
        {
            if (webSocket != null && webSocket.State == WebSocketState.Open)
            {
                var bytes = Encoding.ASCII.GetBytes(message);
                var arraySegment = new ArraySegment<byte>(bytes);
                await webSocket.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        private async Task<string> ReceiveMessageFromClient(WebSocket webSocket)
        {
            if (webSocket != null)
            {
                var buffer = new byte[4096];
                var arraySegment = new ArraySegment<byte>(buffer);
                await webSocket.ReceiveAsync(arraySegment, CancellationToken.None);
                return Encoding.UTF8.GetString(arraySegment.Array, 0, arraySegment.Count);
            }
            return null;
        }

        private async Task PingClient(WebSocket webSocket, string nickName)
        {
            if (webSocket.State == WebSocketState.Open)
            {
                try
                {
                    //PING na conexão para saber se cliente está ativo ainda
                    await SendMessageToClient(webSocket, string.Empty);
                    ReceiveMessageFromClient(webSocket).GetAwaiter().GetResult();
                }
                finally
                {
                    await DisconnectChat(nickName);
                }
                
            }
        }
    }
}
