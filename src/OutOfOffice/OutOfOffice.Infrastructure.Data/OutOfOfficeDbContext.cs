using Microsoft.EntityFrameworkCore;
using OutOfOffice.Domain.Employees;
using OutOfOffice.Domain.Employees.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutOfOffice.Infrastructure.Data
{
    public class OutOfOfficeDbContext : DbContext
    {
        public OutOfOfficeDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Employee> Employees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(OutOfOfficeDbContext).Assembly);
            modelBuilder.Entity<Employee>().HasData(
                new Employee { Id =1, FullName="Rostik Daskaliuk", OutOfOfficeBalance =2, Subdivision=Subdivision.HR, Position=Position.HRManager, Status=EmployeeStatus.Active}
                );
            base.OnModelCreating(modelBuilder);
        }
    }
}
