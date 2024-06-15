using OutOfOffice.Domain.Employees;
using OutOfOffice.Domain.Projects.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutOfOffice.Application.Contracts.DTOs.GiveDTOs
{
    public class GiveProjectDTO
    {
        public int Id { get; set; }
        public string ProjectType { get; set; }
        public DateOnly StartDate { get; set; }

        public DateOnly? EndDate { get; set; }
        public string ProjectManagerId { get; set; }

        public string? Comment { get; set; }
        public string Status { get; set; }
    }
}
