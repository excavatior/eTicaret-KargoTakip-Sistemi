using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class MerhabaController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok("Merhaba, Dunya!");
}
