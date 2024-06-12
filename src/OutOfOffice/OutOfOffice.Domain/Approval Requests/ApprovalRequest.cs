using OutOfOffice.Domain.Approval_Requests.Enums;
using OutOfOffice.Domain.Employees;
using OutOfOffice.Domain.Leave_Requests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutOfOffice.Domain.ApprovalRequests
{
    public class ApprovalRequest
    {
        public int Id { get; set; } 
        public int ApproverId { get; set; }  
        public int LeaveRequestId { get; set; }  
        public ApprovalRequestStatus Status { get; set; }
        public string? Comment { get; set; }

        public LeaveRequest LeaveRequest { get; set; }
        public Employee Approver { get; set; }

    }
}
