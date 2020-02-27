using Microsoft.AspNetCore.Mvc;

namespace Actio.Api.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class HomeController : ControllerBase
  {
    [HttpGet("")]
    public IActionResult Get() => Content("Hello from the other side");
  }
}