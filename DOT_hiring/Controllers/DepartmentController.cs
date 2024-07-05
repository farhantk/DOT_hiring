using DOT_hiring.Data;
using DOT_hiring.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.ComponentModel.DataAnnotations;

namespace DOT_hiring.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IMemoryCache _cache;
        public DepartmentController(DataContext context, IMemoryCache memoryCache)
        {
            _context = context;
            _cache = memoryCache;
        }

        public class DepartmentCreateDto
        {
            [Required]
            [StringLength(50, MinimumLength = 3)]
            [RegularExpression(@"^[a-zA-Z0-9 ]*$", ErrorMessage = "Only letters, numbers, and spaces are allowed.")]
            public string name { get; set; }

            [Required]
            [StringLength(50, MinimumLength = 5)]
            [RegularExpression(@"^[a-zA-Z0-9 ]*$", ErrorMessage = "Only letters, numbers, and spaces are allowed.")]
            public string head { get; set; }

            [System.Text.Json.Serialization.JsonIgnore]
            public ICollection<Employee> Employees { get; set; } = new List<Employee>();
        }

        [HttpGet]
        public async Task<ActionResult<List<Department>>> GetAllDepartment()
        {
            
            if (!_cache.TryGetValue("departments", out List<Department> departments))
            {
                
                departments = await _context.Departments.ToListAsync();

                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
                    SlidingExpiration = TimeSpan.FromMinutes(2)
                };

                _cache.Set("departments", departments, cacheOptions);
            }

            return Ok(departments);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Department>> GetDepartment(int id)
        {
            var department = await _context.Departments.FindAsync(id);

            if (department == null)
            {
                return NotFound("Department not found.");
            }

            return Ok(department);
        }

        [HttpPost]
        public async Task<ActionResult<List<Department>>> AddDepartment(DepartmentCreateDto departmentDto)
        {
            // Check for duplicate department name
            var existingDepartmentName = await _context.Departments
                .AnyAsync(d => d.name == departmentDto.name);

            if (existingDepartmentName)
            {
                return BadRequest(new { error = "Department name already exists." });
            }

            // Check for duplicate department head
            var existingDepartmentHead = await _context.Departments
                .AnyAsync(d => d.head == departmentDto.head);

            if (existingDepartmentHead)
            {
                return BadRequest(new { error = "Department head already exists." });
            }

            var department = new Department
            {
                name = departmentDto.name,
                head = departmentDto.head,
            };

            _context.Departments.Add(department);
            await _context.SaveChangesAsync();

            _cache.Remove("departments");
            return Ok(await _context.Departments.ToListAsync());
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<List<Department>>> UpdateDepartment(DepartmentCreateDto departmentDto, int id)
        {
            // Check for duplicate department name
            var existingDepartmentName = await _context.Departments
                .AnyAsync(d => d.name == departmentDto.name && d.id != id);

            if (existingDepartmentName)
            {
                return BadRequest(new { error = "Department name already exists." });
            }

            // Check for duplicate department head
            var existingDepartmentHead = await _context.Departments
                .AnyAsync(d => d.head == departmentDto.head && d.id != id);

            if (existingDepartmentHead)
            {
                return BadRequest(new { error = "Department head already exists." });
            }

            var department = await _context.Departments.FindAsync(id);
            if (department == null)
                return NotFound("Department not found.");

            department.name = departmentDto.name;
            department.head = departmentDto.head;

            await _context.SaveChangesAsync();

            _cache.Remove("departments");
            return Ok(await _context.Departments.ToListAsync());
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<List<Department>>> DeleteDepartment(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department == null)
                return NotFound("Department not found.");

            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();

            _cache.Remove("departments");
            return Ok(await _context.Departments.ToListAsync());
        }
    }
}
