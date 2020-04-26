using ClientChat.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;

namespace ClientChat
{
    class Program
    {
        private static string NickName;
        private static string UrlPortServer;
        private static ChatMessageService MessageService;

        static void Main(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                Console.WriteLine("Informe uma URL:PORTA para iniciar o cliente!");
                return;
            }
            UrlPortServer = args[0];

            Console.Clear();
            MessageService = ChatMessageService.GetInstance(UrlPortServer);
            ConnectChat();
            RunPromptCommands();
        }

        private static void RunPromptCommands()
        {
            string command = string.Empty;
            while (!command.ToUpper().Trim().Equals("-FECHAR"))
            {
                command = Console.ReadLine();

                if (WebsocketClientService.ActiveSocket && string.IsNullOrEmpty(command)) continue;

                try
                {
                    switch (command.ToUpper().Trim())
                    {
                        case "-FECHAR":
                            break;
                        case "-SAIR":
                            ExitChat();
                            break;
                        case "-LISTAR":
                            PrintList("Usuários no chat:", MessageService.ListAllUsers());
                            break;
                        case "-AJUDA":
                            PrintList("Comandos disponíveis no chat:", MessageService.ListAllCommands());
                            break;
                        default:
                            if (WebsocketClientService.ActiveSocket)
                                Console.WriteLine(MessageService.ProcessCommand(NickName, command).Result);
                            else
                                ConnectChat();
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            MessageService.Disconnect(NickName).Wait();
        }

        private static void PrintList(String title, List<string> list)
        {
            Console.WriteLine($"\n{title}");
            foreach (var item in list)
            {
                Console.WriteLine($"{item}");
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

                    List<string> usersAlreadyExists = null;
                    if (MessageService.UserAlreadyExists(name, out usersAlreadyExists))
                    {
                        if (usersAlreadyExists?.Any() ?? false)
                        {
                            Console.WriteLine($"\nApelido já existe e está em uso!");
                            Console.WriteLine($"\nVeja lista de apelidos em uso:");
                            foreach (var user in usersAlreadyExists)
                            {
                                Console.WriteLine($"{user}");
                            }
                        }
                        continue;
                    }

                    WebsocketClientService.StartAsync(UrlPortServer, name).Wait();

                    if (WebsocketClientService.State == WebSocketState.Open)
                        NickName = name;
                }
            } while (WebsocketClientService.State != WebSocketState.Open);
        }

        private static void ExitChat()
        {
            WebsocketClientService.NotifyClose();
            MessageService.Disconnect(NickName).Wait();
            ConnectChat();
        }
    }
}
