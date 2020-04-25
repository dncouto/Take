using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProvaTake.Data;
using ProvaTake.Service.Intf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProvaTake.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConnectionController : Controller
    {

        private readonly IConnectionManagerService connectionManagerService;

        public ConnectionController(IConnectionManagerService connectionManagerService)
        {
            this.connectionManagerService = connectionManagerService;
        }

        // Entra no Chat
        [HttpGet("start")]
        public async Task ConnectChat(string nickName)
        {
            var context = ControllerContext.HttpContext;
            var isSocketRequest = context.WebSockets.IsWebSocketRequest;

            if (isSocketRequest)
            {
                if (connectionManagerService.GetSocketByNickName(nickName) != null)
                {
                    throw new WebSocketException("Este apelido já está em uso!");
                    //context.Response.Body = new MemoryStream(Encoding.UTF8.GetBytes("Este apelido já está em uso!" ?? ""));
                }
                
                connectionManagerService.ConnectChat(context.WebSockets, nickName);

            }
            else
            {
                context.Response.StatusCode = 400;
            }
        }

        // Sai do Chat
        [HttpGet("stop")]
        public async Task DisconnectChat(string nickName)
        {
            await connectionManagerService.DisconnectChat(nickName);

            //return
        }

        [HttpGet("users")]
        public IEnumerable<string> GetAllUsers()
        {
            return  connectionManagerService.GetAllSockets().Keys;
        }

    }
}
