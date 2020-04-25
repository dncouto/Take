using Microsoft.AspNetCore.Http;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace ServerChat.Service.Intf
{
    public interface IConnectionManagerService
    {
        Task ConnectChat(WebSocketManager webSocketManager, string nickname);

        Task DisconnectChat(string nickName);

        Task SendMessageEveryOne(string message);

        Task SendMessagePrivate(string nickName, string message);

        WebSocket GetSocketByNickName(string nickName);

        ConcurrentDictionary<string, WebSocket> GetAllSockets();
    }
}
