using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutOfOffice.Application.Contracts.DTOs.ChangeDTOs
{
    public class AddEmployeeDTO
    {
        public string FullName { get; set; }
        public string? PeoplePartnerID { get; set; }
        public int OutOfOfficeBalance { get; set; } = 0;
        public string Status { get; set; } = "Active";
        public string Position { get; set; }
        public string Subdivision { get; set; }

    }
}
