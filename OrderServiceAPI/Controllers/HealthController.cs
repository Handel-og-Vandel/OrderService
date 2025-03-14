using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace OrderServiceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        public ILogger<HealthController> _logger { get; }

        public HealthController(ILogger<HealthController> logger)
        {
            _logger = logger;
            _logger.LogInformation("HealthController initialized.");    
        }
     
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public IActionResult GetHealth()
        { 
            return Ok("Service is healthy");
        }
    }
}
