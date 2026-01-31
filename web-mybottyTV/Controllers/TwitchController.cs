using Microsoft.AspNetCore.Mvc;
using Utils;

namespace web_mybottyTV.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TwitchController : ControllerBase
    {
        private readonly BotSettingsService _service;

        public TwitchController(BotSettingsService service)
        {
            _service = service;
        }

        [HttpGet("users")]
        public IActionResult GetChannel([FromQuery] string? login, [FromQuery] string? command = null)
        {
            if (login == null)
               return Ok(_service.GetAllChannels());
            else if (command is null) 
               return Ok(_service.GetChannel(login));
            else
               return Ok(_service.GetSettings(login, command));
        }

        [HttpPost("{channel}/command")]
        public IActionResult AddCommand(string channel)
        {
            //_service.AddCommand(channel, cmd);
            return Ok();
        }
    }
}
