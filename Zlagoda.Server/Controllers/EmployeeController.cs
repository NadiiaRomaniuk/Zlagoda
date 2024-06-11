using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Zlagoda.Server.Database;
using Zlagoda.Server.Models;

namespace Zlagoda.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "manager")]
public class EmployeeController : ControllerBase
{
    private readonly ILogger<TestController> _logger;
    private readonly JwtOptions _jwtOptions;
    private readonly Db _db;

    public EmployeeController(ILogger<TestController> logger, IOptions<JwtOptions> jwtOptions, Db db)
    {
        _logger = logger;
        _jwtOptions = jwtOptions.Value;
        _db = db;
    }

    [HttpGet]
    public async Task<List<EmployeeShort>> GetEmployees()
    {
        return await _db.GetEmployeesShort();
    }

    [HttpGet("{id}")]
    public async Task<Employee?> GetEmployee(string id)
    {
        var employee = await _db.GetEmployee(id);
        if (employee != null)
            employee.Password = null;
        return employee;
    }

    [HttpPost]
    public async Task<bool> AddEmployee(Employee employee)
    {
        employee.Password = AuthController.GetHash(employee.Password!, _jwtOptions.Key);
        return await _db.AddEmployee(employee);
    }

    [HttpPut]
    public async Task<bool> UpdateEmployee(Employee employee)
    {
        if (!string.IsNullOrEmpty(employee.Password))
            employee.Password = AuthController.GetHash(employee.Password, _jwtOptions.Key);
        return await _db.UpdateEmployee(employee, !string.IsNullOrEmpty(employee.Password));
    }

    [HttpDelete("{id}")]
    public async Task<bool> DeleteEmployees(string id)
    {
        return await _db.DeleteEmployee(id);
    }
}
