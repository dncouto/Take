using ServerChat.Model;
using ServerChat.Service.Intf;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServerChat.Service
{
    public class MessageProducerService : IMessageProducerService
    {
        public List<string> GetAllCommands()
        {
            return new List<string>()
            {
                "-AJUDA => Lista comandos disponíveis",
                "-LISTAR => Lista todos os usuários conectados no chat",
                "-TODOS [mensagem] => Envia a mensagem com destino a todos os usuários do chat",
                "[mensagem] => Envia a mensagem com destino a todos os usuários do chat",
                "-PARA -[apelido destinatário] [mensagem] => Define o destinatário da mensagem, mas todos podem ver",
                "-PRIVADO -[apelido destinatário] [mensagem]=> Define o destinatário da mensagem e somente ele vê",
                "-SAIR =>  Sai do chat, permitindo trocar de apelido e entrar novamente",
                "-FECHAR => Sai do chat, e encerra o aplicação cliente do chat"
            };
        }

        public async Task<(bool, string)> SendMessage(IConnectionManagerService connectionManagerService, MessageDTO message)
        {
            if (message.To != null && connectionManagerService.GetSocketByNickName(message.To) == null)
            {
                return (false, "Destinatário inválido ou não está mais no chat!");
            }

            if (String.IsNullOrEmpty(message.Message))
            {
                return (false, "Mensagem não pode ser vazia!");
            }

            try
            {
                if (message.Private)
                {
                    if (String.IsNullOrEmpty(message.To))
                    {
                        return (false, "Para mensagens privada o destinatário é OBRIGATÓRIO!");
                    }
                    string formatedMessage = $"[{message.From}] disse SOMENTE para [{message.To}]: {message.Message}";
                    await connectionManagerService.SendMessagePrivate(message.To, formatedMessage);
                    await connectionManagerService.SendMessagePrivate(message.From, formatedMessage);
                }
                else
                {
                    string formatedMessage = $"[{message.From}] disse para [{message.To ?? "TODOS"}]: {message.Message}";
                    await connectionManagerService.SendMessageEveryOne(formatedMessage);
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }

            return (true, "SUCCESS");
        }
    }
}
