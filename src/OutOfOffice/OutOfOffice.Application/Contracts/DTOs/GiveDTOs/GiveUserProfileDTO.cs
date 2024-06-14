using OutOfOffice.Domain.ApprovalRequests;
using OutOfOffice.Domain.Employees.Enums;
using OutOfOffice.Domain.Employees;
using OutOfOffice.Domain.Leave_Requests;
using OutOfOffice.Domain.Projects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutOfOffice.Application.Contracts.DTOs.GiveDTOs
{
    public class GiveUserProfileDTO
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string? PeoplePartnerID { get; set; }
        public int? OutOfOfficeBalance { get; set; }
        public byte[]? Photo { get; set; }
        public string? Subdivision { get; set; }
        public string? Position { get; set; }
        public string? Status { get; set; }

        public bool FullyRegistered { get; set; } = false;

    }
}
