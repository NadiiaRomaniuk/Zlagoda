using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zlagoda.Server.Database;

namespace Zlagoda.Server.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "manager")]
public class ProductController : ControllerBase
{
    private readonly ILogger<ProductController> _logger;
    private readonly Db _db;

    public ProductController(ILogger<ProductController> logger, Db db)
    {
        _logger = logger;
        _db = db;
    }

    [HttpGet]
    public async Task<List<Product>> GetProducts()
    {
        return await _db.GetProducts();
    }

    [HttpGet("{id}")]
    public async Task<Product?> GetProduct(int id)
    {
        return await _db.GetProduct(id);
    }

    [HttpPost]
    public async Task<int?> AddProduct(Product Product)
    {
        return await _db.AddProduct(Product);
    }

    [HttpPut]
    public async Task<bool> UpdateProduct(Product Product)
    {
        return await _db.UpdateProduct(Product);
    }

    [HttpDelete("{id}")]
    public async Task<bool> DeleteProduct(int id)
    {
        return await _db.DeleteProduct(id);
    }
}
