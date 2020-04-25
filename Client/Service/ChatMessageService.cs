using ClientChat.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace ClientChat.Service
{
    public class ChatMessageService : HttpClientBaseService
    {
        private static ChatMessageService instance;
        public static ChatMessageService Instance
        {
            get { return instance ?? (instance = new ChatMessageService()); }
        }

        private ChatMessageService() { }

        public async Task Disconnect(string nickName)
        {
            await base.GetAsync("connection/stop?nickname=" + nickName);
        }

        public List<string> ListAllUsers()
        {
            List<string> users = null;
            HttpResponseMessage response = base.Get("connection/users");
            if (response.IsSuccessStatusCode)
            {
                var jsonString = response.Content.ReadAsStringAsync().Result;
                users = JsonConvert.DeserializeObject<List<string>>(jsonString);
            }
            return users;
        }

        public List<string> ListAllCommands()
        {
            List<string> users = null;
            HttpResponseMessage response = Get("message/help");
            if (response.IsSuccessStatusCode)
            {
                var jsonString = response.Content.ReadAsStringAsync().Result;
                users = JsonConvert.DeserializeObject<List<string>>(jsonString);
            }
            return users;
        }

        public bool UserAlreadyExists(string nickName, out List<string> userList)
        {
            userList = null;
            HttpResponseMessage response = base.Get("connection/exists?nickname=" + nickName);
            if (!response.IsSuccessStatusCode)
            {
                var jsonString = response.Content.ReadAsStringAsync().Result;
                userList = JsonConvert.DeserializeObject<List<string>>(jsonString);
            }
            return response.StatusCode != System.Net.HttpStatusCode.OK;
        }

        public async Task<string> ProcessCommand(string nickNameOrigin, string commandLine)
        {
            commandLine = commandLine.Trim();
            if (commandLine.StartsWith('-') && commandLine.Contains(' '))
            {
                string[] parts = commandLine.Split(' ');
                string command = parts[0].Substring(1);
                string recipient = parts[1].StartsWith('-') ? parts[1].Substring(1) : null;
                int startMsg = (String.IsNullOrEmpty(recipient) ? 1 : 2);
                string text = string.Empty;
                for (int i = startMsg; i < parts.Length; i++)
                {
                    text += parts[i];
                }
                
                switch (command.ToUpper().Trim())
                {
                    case "TODOS":
                        return await SendMessageAsync(MessageDTO.Build(false, nickNameOrigin, null, text));
                    case "PARA":
                        return await SendMessageAsync(MessageDTO.Build(false, nickNameOrigin, recipient, text));
                    case "PRIVADO":
                        return await SendMessageAsync(MessageDTO.Build(true, nickNameOrigin, recipient, text));
                    default:
                        return "Comando inválido! Use o comando -AJUDA para obter a lista de comandos válidos...;";
                }
            }

            return await SendMessageAsync(MessageDTO.Build(false, nickNameOrigin, null, commandLine));
        }

        private async Task<string> SendMessageAsync(MessageDTO messageData)
        {
            string errors = string.Empty;
            if (!validateMessage(messageData, out errors))
            {
                return errors;
            }

            HttpResponseMessage response = await PostAsync("message/send", messageData);
            if (!response.IsSuccessStatusCode)
            {
                var jsonString = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<string>(jsonString);
            }
            return string.Empty;
        }

        private bool validateMessage(MessageDTO messageData, out string messageError)
        {
            messageError = null;

            if (String.IsNullOrEmpty(messageData.Message))
            {
                messageError = "Mensagem não pode ser vazia!";
                return false;
            }

            if (messageData.Private && String.IsNullOrEmpty(messageData.To))
            {
                messageError = "Para mensagens privada o destinatário é OBRIGATÓRIO!";
                return false;
            }
           
            return true;
        }
    }
}
