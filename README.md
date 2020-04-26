# Processo TAKE Dev .Net Backend
O projeto é divido em módulos cliente e servidor:

O servidor é o projeto "ServerChat": serviço que gerencia o roteamento de mensagens do chat.
O cliente é o projeto "ClientChat": aplicação console que se conecta no servidor para envio e recebimento de mensagens.

Seguem abaixo instruções para execução da aplicação.

---

### Pré-requisitos
Visual Studio ou Docker Desktop instalado no ambiente que for executar.

---

### Iniciando a aplicação pelo Visual Studio
Server:
Compilar e executar o projeto "ServerChat" primeiramente e deixar rodando.

Client:
Compilar e executar o projeto "ClientChat" e seguir instruções na tela do console que abrirá.

### Iniciando o servidor pelo Docker e o cliente pelo .EXE
Server:
Dentro da pasta raiz do projeto existe o arquivo "Run Server.bat" que contém os comandos para criar uma imagem no Docker, e subir um container dessa imagem. Executar este arquivo.

Frontend:
Dentro da pasta raiz do projeto existe o arquivo "Run Client.bat" que contém os comandos para executar o BUILD e iniciar a aplicação client Console .Net. Executar este arquivo.

O servidor é iniciado na url e porta: http://localhost:3000/

### Instruções para uso do chat
Após informar seu apelido e entrar no chat você pode digitar o comando -AJUDA para listar todos os comandos disponíveis.

Os comandos possíveis são:

-AJUDA => Lista comandos disponíveis",
-LISTAR => Lista todos os usuários conectados no chat
-TODOS [mensagem] => Envia a mensagem com destino a todos os usuários do chat
[somente mensagem] => Envia a mensagem com destino a todos os usuários do chat (igual comando TODOS)
-PARA -[apelido destinatário] [mensagem] => Define o destinatário da mensagem, mas todos podem ver
-PRIVADO -[apelido destinatário] [mensagem]=> Define o destinatário da mensagem e somente ele vê
-SAIR =>  Sai do chat, permitindo trocar de apelido e entrar novamente
-FECHAR => Sai do chat, e encerra o aplicação cliente do chat

