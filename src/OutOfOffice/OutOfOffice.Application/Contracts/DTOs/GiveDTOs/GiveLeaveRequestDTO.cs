using OutOfOffice.Domain.ApprovalRequests;
using OutOfOffice.Domain.Employees;
using OutOfOffice.Domain.Leave_Requests.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutOfOffice.Application.Contracts.DTOs.GiveDTOs
{
    public class GiveLeaveRequestDTO
    {
        public int Id { get; set; }
        public string EmployeeId { get; set; }
        public string AbsenceReason { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string? Comment { get; set; }
        public string Status { get; set; }

    }
}
