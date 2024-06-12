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
        public EmployeeStatus EmployeeStatus { get; set; }
        public Position Position { get; set; }

        public Subdivision Subdivision { get; set; }

        public string? ParnerName { get; set; } = "";

        public int DayOffCount { get; set; } = 0;
    }
}
