using Microsoft.AspNetCore.Mvc;

namespace web_mybottyTV.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TwitchController : ControllerBase
    {
        private readonly ILogger<TwitchController> _logger;

        public TwitchController(ILogger<TwitchController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetTwitch")]
        public string Get()
        {
            return "aaaaa";
        }
    }
}
