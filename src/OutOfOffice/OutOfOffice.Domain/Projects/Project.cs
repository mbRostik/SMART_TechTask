using OutOfOffice.Domain.Employees;
using OutOfOffice.Domain.Projects.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutOfOffice.Domain.Projects
{
    public class Project
    {
        public int Id { get; set; }
        public ProjectType ProjectType { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public string ProjectManagerId { get; set; }
        public string? Comment { get; set; }
        public ProjectStatus Status { get; set; }
        public Employee ProjectManager { get; set; }

        public List<EmployeeProject> EmployeeProjects { get; set; }
    }

}
