using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zlagoda.Server.Database;

namespace Zlagoda.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "manager")]
public class CategoryController : ControllerBase
{
    private readonly ILogger<CategoryController> _logger;
    private readonly Db _db;

    public CategoryController(ILogger<CategoryController> logger, Db db)
    {
        _logger = logger;
        _db = db;
    }

    [HttpGet]
    public async Task<List<Category>> GetCategories()
    {
        return await _db.GetCategories();
    }

    [HttpGet("{id}")]
    public async Task<Category?> GetCategory(int id)
    {
        return await _db.GetCategory(id);
    }

    [HttpPost]
    public async Task<int?> AddCategory(Category category)
    {
        return await _db.AddCategory(category);
    }

    [HttpPut]
    public async Task<bool> UpdateCategory(Category category)
    {
        return await _db.UpdateCategory(category);
    }

    [HttpDelete("{id}")]
    public async Task<bool> DeleteCategory(int id)
    {
        return await _db.DeleteCategory(id);
    }
}
