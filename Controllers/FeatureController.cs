using Microsoft.AspNetCore.Mvc;

namespace MySecureApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeatureController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public FeatureController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("hello")]
        public IActionResult SayHello()
        {
            
            return Ok(new { WelcomeNote = "Hi This is my application" });
        }

        [HttpGet("feature-x")]
        public IActionResult GetFeatureStatus()
        {
            bool isFeatureXEnabled = _configuration.GetValue<bool>("FeatureXEnabled");
            return Ok(new { FeatureXEnabled = isFeatureXEnabled });
        }

        [HttpGet("connectionstring")]
        public IActionResult GetConnectionString()
        {
            object? connectionstring = _configuration.GetValue<string>("connectionstring");
            return Ok(new { Connectionstring = connectionstring?.ToString() });
        }

        [HttpGet("welcome")]
        public IActionResult GetWelcomeNote()
        {
            object? welcomenote = _configuration.GetValue<string>("welcomenote");
            return Ok(new { WelcomeNote = string.Concat("Hello ", welcomenote?.ToString()) });
        }

        [HttpGet("autorefresh")]
        public IActionResult GetAutoRefresh()
        {
            object? autorefresh = _configuration.GetValue<string>("autorefresh");
            return Ok(new { AutoRefresh = string.Concat("Hello ", autorefresh?.ToString()) });
        }
    }
}
