using ClientChat.Service;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace ClientChat
{
    public class TopicArea
    {
        public string Name { get; set; }
    }

    class Program
    {
        private static string NickName;

        static void Main()
        {
            ConnectChat();
            RunPromptCommands();
        }

        private static void RunPromptCommands()
        {
            string command = string.Empty;
            while (!command.ToUpper().Trim().Equals("/FECHAR"))
            {
                try
                {
                    switch (command.ToUpper().Trim())
                    {
                        case "/SAIR":
                            ExitChat();
                            break;
                        case "/LISTAR":
                            ListActiveUsers();
                            break;
                        case "/TESTE":
                            List<TopicArea> topicAreas = ChatMessageService.Instance.GetDados("home").Result;
                            foreach (var topicArea in topicAreas)
                            {
                                Console.WriteLine($"Name: {topicArea.Name}");
                            }
                            break;
                        default:
                            if (!WebsocketClientService.ActiveSocket)
                                ConnectChat();
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                Console.WriteLine("");
                command = Console.ReadLine();
            }
            ExitChat();
        }

        private static void ListActiveUsers()
        {
            List<string> listUsers = ChatMessageService.Instance.ListAllUsers().Result;
            foreach (var user in listUsers)
            {
                Console.WriteLine($"\n{user}");
            }
        }

        private static void ConnectChat()
        {
            do
            {
                if (WebsocketClientService.ActiveSocket == false)
                {
                    Console.Write($"\nInforme seu Apelido: ");
                    string name = Console.ReadLine();

                    WebsocketClientService.StartAsync(name).Wait();

                    if (WebsocketClientService.State == WebSocketState.Open)
                        NickName = name;
                }
            } while (WebsocketClientService.State != WebSocketState.Open);
        }

        private static void ExitChat()
        {
            ChatMessageService.Instance.Disconnect(NickName).Wait();
            ConnectChat();
        }
    }
}
