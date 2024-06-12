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
        public DateTime StartDate { get; set; } 

        public DateTime? EndDate { get; set; } 
        public int ProjectManagerId { get; set; }  

        public string? Comment { get; set; }  
        public ProjectStatus Status { get; set; }
        public Employee ProjectManager { get; set; }

    }
}
