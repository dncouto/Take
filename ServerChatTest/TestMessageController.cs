using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NUnit.Framework;
using ServerChat.Controllers;
using ServerChat.Model;
using ServerChat.Service;
using ServerChat.Service.Intf;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Assert = NUnit.Framework.Assert;

namespace ServerChatTest
{
    [TestClass]
    public class TestMessageController
    {
        Mock<IConnectionManagerService> connectionManagerService;
        MessageController messageController;
        Mock<WebSocket> mockWebSoquet;

        [SetUp]
        public void Setup()
        {
            connectionManagerService = new Mock<IConnectionManagerService>();
            messageController = new MessageController(new MessageProducerService(), connectionManagerService.Object);
            mockWebSoquet = new Mock<WebSocket>();
        }

        [Test]
        public void GetCommands()
        {
            // Act
            var actionResult = messageController.GetCommands();
            
            // Assert
            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOf(typeof(ObjectResult), actionResult.Result);
            ObjectResult objectResult = actionResult.Result as ObjectResult;
            Assert.AreEqual(objectResult.StatusCode, 200);
            Assert.IsInstanceOf(typeof(List<string>), objectResult.Value, "Objeto de dados retornado incorreto");
            IList<string> values = objectResult.Value as IList<string>;
            Assert.AreEqual(values.Count, 8, "Quantidade incorreta de comandos");
        }

        [Test]
        public void SendUserDestinyNotFound()
        {
            // Arrange
            MessageDTO message = MessageDTO.Build(false, "origin", "recipient", "");
            connectionManagerService
                .Setup(x => x.GetSocketByNickName("NickNameTest"))
                .Returns((WebSocket)null);

            // Act
            var actionResult = messageController.Send(message);

            // Assert
            AssertSendErrorResponseMessage(actionResult, "Destinatário inválido ou não está mais no chat!");
        }

        [Test]
        public void SendUserOriginNull()
        {
            // Arrange
            MessageDTO message = MessageDTO.Build(false, null, null, "");
            
            // Act
            var actionResult = messageController.Send(message);

            // Assert
            AssertSendErrorResponseMessage(actionResult, "O usuário que enviou a mensagem é obrigatório e não foi informado!");
        }

        [Test]
        public void SendMessageNull()
        {
            // Arrange
            MessageDTO message = MessageDTO.Build(false, "origin", null, "");

            // Act
            var actionResult = messageController.Send(message);

            // Assert
            AssertSendErrorResponseMessage(actionResult, "Mensagem não pode ser vazia!");
        }

        [Test]
        public void SendPrivateUserDestinyNull()
        {
            // Arrange
            MessageDTO message = MessageDTO.Build(true, "origin", null, "xxx");
            
            // Act
            var actionResult = messageController.Send(message);

            // Assert
            AssertSendErrorResponseMessage(actionResult, "Para mensagens privada o destinatário é OBRIGATÓRIO!");
        }

        [Test]
        public void SendSuccess()
        {
            // Arrange TODOS
            MessageDTO message = MessageDTO.Build(false, "origin", "recipient", "xxx");
            connectionManagerService
                .Setup(x => x.GetSocketByNickName(message.To))
                .Returns(mockWebSoquet.Object);

            connectionManagerService
                .Setup(x => x.SendMessageEveryOne(message.Message))
                .Returns(Task.FromResult(""));

            // Act TODOS
            var actionResult = messageController.Send(message);

            // Assert TODOS
            AssertSendSuccessResponseMessage(actionResult, "SUCCESS");


            // Arrange PRIVADO
            message = MessageDTO.Build(true, "origin", "recipient", "xxx");
            connectionManagerService
                .Setup(x => x.SendMessagePrivate(message.From, message.Message))
                .Returns(Task.FromResult(""));

            // Act PRIVADO
            actionResult = messageController.Send(message);

            // Assert PRIVADO
            AssertSendSuccessResponseMessage(actionResult, "SUCCESS");
        }

        private void AssertSendErrorResponseMessage(dynamic actionResult, string corretMsg)
        {
            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOf(typeof(ObjectResult), actionResult.Result);
            ObjectResult objectResult = actionResult.Result as ObjectResult;
            Assert.AreEqual(objectResult.StatusCode, 400);
            Assert.IsInstanceOf(typeof(string), objectResult.Value);
            string value = objectResult.Value as string;
            Assert.AreEqual(value, corretMsg);
        }

        private void AssertSendSuccessResponseMessage(dynamic actionResult, string corretMsg)
        {
            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOf(typeof(ObjectResult), actionResult.Result);
            ObjectResult objectResult = actionResult.Result as ObjectResult;
            Assert.AreEqual(objectResult.StatusCode, 200);
            Assert.IsInstanceOf(typeof(string), objectResult.Value);
            string value = objectResult.Value as string;
            Assert.AreEqual(value, corretMsg);
        }
    }
}