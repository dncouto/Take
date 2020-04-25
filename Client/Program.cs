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

        static void Main()
        {
            ConnectChat();
            RunPromptCommands();
        }

        private static void RunPromptCommands()
        {
            string command = string.Empty;
            while (!command.ToUpper().Trim().Equals("-FECHAR"))
            {
                command = Console.ReadLine();

                if (string.IsNullOrEmpty(command)) continue;

                try
                {
                    switch (command.ToUpper().Trim())
                    {
                        case "-SAIR":
                            ExitChat();
                            break;
                        case "-LISTAR":
                            PrintList("Usuários no chat:", ChatMessageService.Instance.ListAllUsers());
                            break;
                        case "-AJUDA":
                            PrintList("Comandos disponíveis no chat:", ChatMessageService.Instance.ListAllCommands());
                            break;
                        default:
                            Console.WriteLine(ChatMessageService.Instance.ProcessCommand(NickName, command).Result);
                            if (!WebsocketClientService.ActiveSocket)
                                ConnectChat();
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            ChatMessageService.Instance.Disconnect(NickName).Wait();
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
                    if (ChatMessageService.Instance.UserAlreadyExists(name, out usersAlreadyExists))
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
