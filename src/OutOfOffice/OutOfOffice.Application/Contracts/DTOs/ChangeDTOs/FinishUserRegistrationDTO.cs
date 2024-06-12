using OutOfOffice.Domain.Employees.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutOfOffice.Application.Contracts.DTOs.ChangeDTOs
{
    public class FinishUserRegistrationDTO
    {
        public string EmployeeStatus { get; set; }
        public string Position { get; set; }

        public string Subdivision { get; set; }

        public string? PartnerName { get; set; } = "";

        public int DayOffCount { get; set; } = 0;
    }
}
