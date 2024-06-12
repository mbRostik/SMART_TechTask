using OutOfOffice.Domain.Employees;
using OutOfOffice.Domain.Leave_Requests.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OutOfOffice.Domain.ApprovalRequests;

namespace OutOfOffice.Domain.Leave_Requests
{
    public class LeaveRequest
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public AbsenceReason AbsenceReason { get; set; }
        public DateTime StartDate { get; set; }  
        public DateTime EndDate { get; set; }  
        public string? Comment { get; set; }  
        public LeaveRequestStatus Status { get; set; }
        public Employee Employee { get; set; }
        public List<ApprovalRequest> ApprovalRequests { get; set; }

    }

}
