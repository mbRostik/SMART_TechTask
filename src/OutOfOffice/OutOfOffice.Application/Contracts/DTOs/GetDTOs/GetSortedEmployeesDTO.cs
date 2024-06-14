using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutOfOffice.Application.Contracts.DTOs.GetDTOs
{
    public class GetSortedEmployeesDTO
    {
        public string? ColumnName { get; set; } = "";
        public bool Descending { get; set; } = true;

        public string Status { get; set; } = "";
        public string Position { get; set; } = "";
        public string Subdivision { get; set; } = "";
    }
}
