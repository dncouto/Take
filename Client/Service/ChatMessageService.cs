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
            HttpResponseMessage response = base.Get("message/help");
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

        public string ProcessCommand(string nickNameOrigin, string commandLine)
        {
            if (commandLine.StartsWith('-'))
            {
                if (commandLine.Contains(' '))
                {
                    string command = commandLine.Substring(2, commandLine.IndexOf(" ")-1);
                    string text = commandLine.Substring(commandLine.IndexOf(" "));
                    switch (command.ToUpper().Trim())
                    {
                        case "TODOS":

                            break;
                        default:
                            break;
                    }
                }
            }
            return "Comando inválido! Use o comando -AJUDA para obter a lista de comandos válidos...;";
        }
    }
}
