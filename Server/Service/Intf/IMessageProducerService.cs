using ServerChat.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServerChat.Service.Intf
{
    public interface IMessageProducerService
    {
        List<string> GetAllCommands();

        Task<(bool, string)> SendMessage(IConnectionManagerService connectionManagerService, MessageDTO message);
    }
}