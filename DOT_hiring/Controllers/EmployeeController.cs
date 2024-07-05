using DOT_hiring.Data;
using DOT_hiring.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace DOT_hiring.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IMemoryCache _cache;
        public EmployeeController(DataContext context, IMemoryCache memoryCache)
        {
            _context = context;
            _cache = memoryCache;
        }
        public class EmployeeCreateDto
        {
            [Required]
            public int departmentid { get; set; }

            [Required]
            [StringLength(100, MinimumLength = 3)]
            [RegularExpression(@"^[a-zA-Z0-9\s]*$", ErrorMessage = "Name can only contain letters, numbers, and spaces")]
            public string name { get; set; }

            [Required]
            [StringLength(50, MinimumLength = 2)]
            [RegularExpression(@"^[a-zA-Z0-9\s]*$", ErrorMessage = "Role can only contain letters, numbers, and spaces")]
            public string role { get; set; }

            [Required]
            [RegularExpression(@"^[a-zA-Z0-9_.+-]+@(gmail|yahoo)\.com$", ErrorMessage = "Email domain must be gmail.com or yahoo.com")]
            public string email { get; set; }
        }

        [HttpGet]
        public async Task<ActionResult<List<Employee>>> GetAllEmployee()
        {
            
            if (!_cache.TryGetValue("employees", out List<Employee> employees))
            {
                
                employees = await _context.Employees.Include(e => e.department).ToListAsync();

                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
                    SlidingExpiration = TimeSpan.FromMinutes(2) 
                };

                _cache.Set("employees", employees, cacheOptions);
            }

            return Ok(employees);
        }

        [HttpPost]
        public async Task<ActionResult<List<Employee>>> AddEmployee(EmployeeCreateDto employeeDto)
        {
            var department = await _context.Departments.FindAsync(employeeDto.departmentid);
            if (department == null)
            {
                return NotFound("Department not found");
            }

            // Check for duplicate email
            var existingEmployee = await _context.Employees
                .FirstOrDefaultAsync(e => e.email == employeeDto.email);
            if (existingEmployee != null)
            {
                return BadRequest("Email already exists");
            }

            var employee = new Employee
            {
                departmentid = employeeDto.departmentid,
                name = employeeDto.name,
                role = employeeDto.role,
                email = employeeDto.email,
                department = department
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            _cache.Remove("employees");
            return Ok(await _context.Employees.Include(e => e.department).ToListAsync());
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<List<Employee>>> UpdateEmployee(EmployeeCreateDto employeeDto, int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
                return NotFound("User not found.");

            var department = await _context.Departments.FindAsync(employeeDto.departmentid);
            if (department == null)
                return NotFound("Department not found.");

            employee.departmentid = employeeDto.departmentid;
            employee.name = employeeDto.name;
            employee.role = employeeDto.role;
            employee.email = employeeDto.email;
            employee.department = department;

            await _context.SaveChangesAsync();

            _cache.Remove("employees");
            return Ok(await _context.Employees.Include(e => e.department).ToListAsync());
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<List<Employee>>> DeleteEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
                return NotFound("Employee not found.");

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            _cache.Remove("employees");
            return Ok(await _context.Employees.Include(e => e.department).ToListAsync());
        }
    }
}
