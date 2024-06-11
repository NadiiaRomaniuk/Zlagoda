using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zlagoda.Server.Database;

namespace Zlagoda.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "manager")]
public class TestController : ControllerBase
{
    private readonly ILogger<TestController> _logger;
    private readonly Db _db;

    public TestController(ILogger<TestController> logger, Db db)
    {
        _logger = logger;
        _db = db;
    }

    [HttpGet]
    public async Task<string> Get()
    {
        var count = await _db.GetEmployeesCount();
        return $"Test OK {count}";
    }
}
