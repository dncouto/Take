using Microsoft.AspNetCore.Mvc;
using ServerChat.Model;
using ServerChat.Service.Intf;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServerChat.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly IMessageProducerService messageProducerService;
        private readonly IConnectionManagerService connectionManagerService;

        public MessageController(IMessageProducerService messageProducerService, IConnectionManagerService connectionManagerService)
        {
            this.messageProducerService = messageProducerService;
            this.connectionManagerService = connectionManagerService;
        }

        [HttpGet("help")]
        public ActionResult<IEnumerable<string>> GetCommands()
        {
            return Ok(messageProducerService.GetAllCommands());
        }

        [HttpPost("send")]
        public async Task<ActionResult> Send(MessageDTO message)
        {
            (bool Success, string msgResult) result = await messageProducerService.SendMessage(connectionManagerService, message);
            if (result.Success)
                return Ok(result.msgResult);
            else
                return BadRequest(result.msgResult);
        }
    }
}
