using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProvaTake.Data;
using ProvaTake.Service.Intf;

namespace ProvaTake.Controllers
{
    [ApiController]
    [Route("Home")]
    public class HomeController : ControllerBase
    {
        private readonly IConnectionManagerService connectionManagerService;

        public HomeController(IConnectionManagerService connectionManagerService)
        {
           this.connectionManagerService = connectionManagerService;
        }

        [HttpGet]
        public async Task<IEnumerable<TopicArea>> GetAllAsync()
        {
            await connectionManagerService.SendMessageEveryOne("mensagem de teste");

            return connectionManagerService.GetAllTopicAreas();
        }

    }
}
