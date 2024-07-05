using Microsoft.EntityFrameworkCore;
using DOT_hiring.Model;

namespace DOT_hiring.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.department)
                .WithMany(d => d.Employees)
                .HasForeignKey(e => e.departmentid)
                .IsRequired();
        }
    }
}