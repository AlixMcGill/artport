using Microsoft.AspNetCore.Mvc;

namespace myApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase {
    [HttpGet]
    public IActionResult GetAll() {
        return Ok(new[] {"Alice", "bob", "Charlie"});
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        return Ok($"User {id}");
    }

    [HttpPost]
    public IActionResult Create([FromBody] string name)
    {
        return Ok($"Created user: {name}");
    }
}
