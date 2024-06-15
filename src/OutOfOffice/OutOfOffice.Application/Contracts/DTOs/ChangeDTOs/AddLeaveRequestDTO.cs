using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutOfOffice.Application.Contracts.DTOs.ChangeDTOs
{
    public class AddLeaveRequestDTO
    {
        public string Id { get; set; } = "";
        public string AbsenceReason { get; set; } = "";

        public DateOnly StartDate {  get; set; }

        public DateOnly EndDate { get; set; }

        public string Comment { get; set; } = "";

    }
}
