using ServerChat.Model;
using ServerChat.Service.Intf;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServerChat.Service
{
    public class MessageProducerService : IMessageProducerService
    {
        private MessageProducerService() { }

        public List<string> GetAllCommands()
        {
            return new List<string>()
            {
                "-AJUDA => Lista comandos disponíveis",
                "-LISTAR => Lista todos os usuários conectados no chat",
                "-TODOS [mensagem] => Envia a mensagem com destino a todos os usuários do chat",
                "-PARA -[apelido destinatário] [mensagem] => Define o destinatário da mensagem, mas todos podem ver",
                "-PRIVADO -[apelido destinatário] [mensagem]=> Define o destinatário da mensagem e somente ele vê",
                "-SAIR =>  Sai do chat, permitindo trocar de apelido e entrar novamente",
                "-FECHAR => Sai do chat, e encerra o aplicação cliente do chat"
            };
        }

        public async Task<(bool, string)> SendMessage(IConnectionManagerService connectionManagerService, MessageDTO message)
        {
            if (message.Private)
            {
                string formatedMessage = $"De {message.From} para {message.To} : {message.Message}";
                await connectionManagerService.SendMessagePrivate(message.To, formatedMessage);
            }
            else
            {
                string formatedMessage = $"De {message.From} para {message.To ?? "TODOS"} : {message.Message}";
                await connectionManagerService.SendMessageEveryOne(formatedMessage);
            }

            return (true, "SUCCESS");
        }
    }
}
