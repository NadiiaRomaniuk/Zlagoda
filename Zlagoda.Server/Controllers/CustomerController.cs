using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zlagoda.Server.Database;

namespace Zlagoda.Server.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "manager")]
public class CustomerController : ControllerBase
{
    private readonly ILogger<CustomerController> _logger;
    private readonly Db _db;

    public CustomerController(ILogger<CustomerController> logger, Db db)
    {
        _logger = logger;
        _db = db;
    }

    [HttpGet]
    public async Task<List<CustomerShort>> GetCustomers()
    {
        return await _db.GetCustomersShort();
    }

    [HttpGet("{id}")]
    public async Task<Customer?> GetCustomer(string id)
    {
        return await _db.GetCustomer(id); ;
    }

    [HttpPost]
    public async Task<bool> AddCustomer(Customer customer)
    {
        return await _db.AddCustomer(customer);
    }

    [HttpPut]
    public async Task<bool> UpdateCustomer(Customer customer)
    {
        return await _db.UpdateCustomer(customer);
    }

    [HttpDelete("{id}")]
    public async Task<bool> DeleteCustomers(string id)
    {
        return await _db.DeleteCustomer(id);
    }
}
