using OutOfOffice.Domain.Approval_Requests.Enums;
using OutOfOffice.Domain.Employees;
using OutOfOffice.Domain.Leave_Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutOfOffice.Application.Contracts.DTOs.GiveDTOs
{
    public class GiveApprovalRequestDTO
    {
        public int Id { get; set; }
        public string? ApproverId { get; set; }
        public int LeaveRequestId { get; set; }
        public string Status { get; set; }
        public string? Comment { get; set; }

        public GiveLeaveRequestDTO GiveLeaveRequestDTO { get; set; }
    }
}
