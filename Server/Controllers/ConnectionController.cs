using Microsoft.AspNetCore.Mvc;
using ServerChat.Service.Intf;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace ServerChat.Controllers
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
        public async Task<IActionResult> ConnectChat(string nickName)
        {
            var context = ControllerContext.HttpContext;
            var isSocketRequest = context.WebSockets.IsWebSocketRequest;

            if (isSocketRequest)
            {
                if (connectionManagerService.GetSocketByNickName(nickName) != null)
                {
                    return BadRequest("Este apelido já está em uso!");
                }
                
                connectionManagerService.ConnectChat(context.WebSockets, nickName);

                return new EmptyResult();
            }
            else
            {
                return BadRequest("Protocolo inválido para webSocket!");
            }
        }

        // Sai do Chat
        [HttpGet("stop")]
        public async Task DisconnectChat(string nickName)
        {
            await connectionManagerService.DisconnectChat(nickName);
        }

        [HttpGet("users")]
        public ActionResult<IEnumerable<string>> GetAllUsers()
        {
            return Ok(connectionManagerService.GetAllSockets().Keys);
        }

        [HttpGet("exists")]
        public ActionResult<IEnumerable<string>> GetUserExists(string nickName)
        {
            if (connectionManagerService.GetSocketByNickName(nickName) != null)
            {
                return BadRequest(connectionManagerService.GetAllSockets().Keys);
            }
            return Ok();
        }

    }
}
