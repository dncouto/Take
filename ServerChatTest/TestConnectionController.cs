using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NUnit.Framework;
using ServerChat.Controllers;
using ServerChat.Model;
using ServerChat.Service;
using ServerChat.Service.Intf;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Assert = NUnit.Framework.Assert;

namespace ServerChatTest
{
    [TestClass]
    public class TestConnectionController
    {
        ConnectionController connectionController;
        Mock<WebSocketManager> mockWebSocketManager;

        [SetUp]
        public void Setup()
        {
            connectionController = new ConnectionController(new ConnectionManagerService());
            mockWebSocketManager = new Mock<WebSocketManager>();

            var httpResponseMock = new Mock<HttpResponse>();
            httpResponseMock.Setup(x => x.StatusCode).Returns(0);

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.SetupGet(x => x.WebSockets).Returns(mockWebSocketManager.Object);
            httpContextMock.SetupGet(x => x.Response).Returns(httpResponseMock.Object);

            ControllerContext controlerContextMock = new ControllerContext();
            controlerContextMock.HttpContext = httpContextMock.Object;

            connectionController.ControllerContext = controlerContextMock;
        }

        [Test]
        public void ConnectChatNotRequestWebSocketProtocol()
        {
            // Arrange
            mockWebSocketManager.SetupGet(x => x.IsWebSocketRequest).Returns(false);

            // Act
            var actionResult = connectionController.ConnectChat("nickName");

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOf(typeof(ObjectResult), actionResult.Result);
            ObjectResult objectResult = actionResult.Result as ObjectResult;
            Assert.AreEqual(objectResult.StatusCode, 400);
            Assert.IsInstanceOf(typeof(string), objectResult.Value);
            string value = objectResult.Value as string;
            Assert.AreEqual(value, "Protocolo inválido para webSocket!");
        }

        [Test]
        public void ConnectChatSuccess()
        {
            // Arrange
            Mock<WebSocket> mockWebSocket = new Mock<WebSocket>();
            mockWebSocket.SetupGet(x => x.State).Returns(WebSocketState.Open);
            mockWebSocket.Setup(x => x.SendAsync(new ArraySegment<byte>(Encoding.ASCII.GetBytes("")), WebSocketMessageType.Text, true, CancellationToken.None)).Returns(Task.FromResult(mockWebSocket.Object));
            mockWebSocketManager.Setup(x => x.AcceptWebSocketAsync()).Returns(Task.FromResult(mockWebSocket.Object));
            mockWebSocketManager.SetupGet(x => x.IsWebSocketRequest).Returns(true);

            // Act
            var actionResult = connectionController.ConnectChat("nickNameTest");

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOf(typeof(OkResult), actionResult.Result);
            OkResult okResult = actionResult.Result as OkResult;
            Assert.AreEqual(okResult.StatusCode, 200);
        }

        [Test]
        public void ConnectChatUserExists()
        {
        }
    }
}