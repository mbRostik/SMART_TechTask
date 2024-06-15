using OutOfOffice.Domain.Employees;
using OutOfOffice.Domain.Projects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutOfOffice.Domain
{
    public class EmployeeProject
    {
        public string EmployeeId { get; set; }

        public int ProjectId { get; set; }

        public Employee Employee { get; set; }
        public Project Project { get; set; }
    }
}
