using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SchedulerWebAPI.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("[controller]")]
    public class PingController : ControllerBase
    {
        //private readonly ILogger<PingController> _logger;

        //public PingController(ILogger<PingController> logger)
        //{
        //    _logger = logger;
        //}

        [HttpGet]
        public IActionResult Get()
        {
            return new OkObjectResult(new { status = "OK" });
        }
    }
}