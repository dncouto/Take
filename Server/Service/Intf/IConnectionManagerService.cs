using Microsoft.AspNetCore.Http;
using ProvaTake.Data;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace ProvaTake.Service.Intf
{
    public interface IConnectionManagerService
    {
        IEnumerable<TopicArea> GetAllTopicAreas();

        Task ConnectChat(WebSocketManager webSocketManager, string nickname);

        Task DisconnectChat(string nickName);

        Task SendMessageEveryOne(string message);

        Task SendMessageEveryOne(string message, string nickName);

        Task SendMessagePrivate(string nickName, string message);

        WebSocket GetSocketByNickName(string nickName);

        ConcurrentDictionary<string, WebSocket> GetAllSockets();
    }
}
