using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OutOfOffice.Domain.Employees.Enums;
using OutOfOffice.Domain.Leave_Requests;
using OutOfOffice.Domain.ApprovalRequests;
using OutOfOffice.Domain.Projects;

namespace OutOfOffice.Domain.Employees
{
    public class Employee
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public int? PeoplePartnerID { get; set; }
        public int OutOfOfficeBalance { get; set; }
        public byte[]? Photo { get; set; }
        public Subdivision Subdivision { get; set; } 
        public Position Position { get; set; }  
        public EmployeeStatus Status { get; set; }
        public Employee PeoplePartner { get; set; }

        public List<LeaveRequest> LeaveRequests { get; set; }
        public List<ApprovalRequest> ApprovalRequests { get; set; }
        public List<Project> Projects { get; set; }


    }
}
