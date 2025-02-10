using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EmployeeManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // All endpoints require authentication
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly ILogger<EmployeesController> _logger;

        public EmployeesController(IEmployeeService employeeService, ILogger<EmployeesController> logger)
        {
            _employeeService = employeeService;
            _logger = logger;
        }

        // GET: api/Employees
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var employees = await _employeeService.GetAllEmployeesAsync();
            return Ok(employees);
        }

        // GET: api/Employees/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee == null)
                return NotFound();
            return Ok(employee);
        }

        // POST: api/Employees
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateEmployeeRequest request)
        {
            // Extract the creator's role from JWT claims.
            string? roleClaimValue = User.Claims.FirstOrDefault(
                c => c.Type == ClaimTypes.Role || c.Type.Equals("role", StringComparison.OrdinalIgnoreCase)
            )?.Value;

            if (string.IsNullOrEmpty(roleClaimValue))
            {
                _logger.LogWarning("Creator role not found in token.");
                return Unauthorized("User role not found in token.");
            }

            if (!Enum.TryParse<Role>(roleClaimValue, true, out Role creatorRole))
            {
                _logger.LogWarning("Failed to parse creator role from token: {RoleClaim}", roleClaimValue);
                return Unauthorized("User role in token is invalid.");
            }

            var result = await _employeeService.CreateEmployeeAsync(
                firstName: request.FirstName,
                lastName: request.LastName,
                email: request.Email,
                documentNumber: request.DocumentNumber,
                phones: request.Phones,
                role: request.Role,
                password: request.Password,
                birthDate: request.BirthDate,
                managerId: request.ManagerId,
                creatorRole: creatorRole
            );

            if (!result.IsSuccess)
                return BadRequest(result.Errors);

            return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
        }

        // PUT: api/Employees/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEmployeeRequest request)
        {
            // Extract the updater's role from JWT claims.
            string? roleClaimValue = User.Claims.FirstOrDefault(
                c => c.Type == ClaimTypes.Role || c.Type.Equals("role", StringComparison.OrdinalIgnoreCase)
            )?.Value;

            if (string.IsNullOrEmpty(roleClaimValue))
            {
                _logger.LogWarning("Updater role not found in token.");
                return Unauthorized("User role not found in token.");
            }

            if (!Enum.TryParse<Role>(roleClaimValue, true, out Role updaterRole))
            {
                _logger.LogWarning("Failed to parse updater role from token: {RoleClaim}", roleClaimValue);
                return Unauthorized("User role in token is invalid.");
            }

            var result = await _employeeService.UpdateEmployeeAsync(
                id: id,
                firstName: request.FirstName,
                lastName: request.LastName,
                email: request.Email,
                phones: request.Phones,
                role: request.Role,
                birthDate: request.BirthDate,
                managerId: request.ManagerId,
                updaterRole: updaterRole
            );

            if (!result.IsSuccess)
                return BadRequest(result.Errors);

            return Ok(result.Value);
        }

        // DELETE: api/Employees/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _employeeService.DeleteEmployeeAsync(id);
            return NoContent();
        }
    }

    // DTO for creating an employee
    public class CreateEmployeeRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string DocumentNumber { get; set; } = string.Empty;
        public List<string> Phones { get; set; } = new();
        public Role Role { get; set; }
        public string Password { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public Guid? ManagerId { get; set; }
    }

    // DTO for updating an employee
    public class UpdateEmployeeRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<string> Phones { get; set; } = new();
        public Role Role { get; set; }
        public DateTime BirthDate { get; set; }
        public Guid? ManagerId { get; set; }
    }
}
