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

        [HttpGet("user={channel}")]
        public BotSettings GetChannel(string channel)
        {
            return _service.GetChannel(channel);
        }

        [HttpGet("users")]
        public BotSettings[] GetChannel()
        {
            return _service.GetChannel();
        }

        [HttpPost("{channel}/command")]
        public IActionResult AddCommand(string channel)
        {
            //_service.AddCommand(channel, cmd);
            return Ok();
        }
    }
}
