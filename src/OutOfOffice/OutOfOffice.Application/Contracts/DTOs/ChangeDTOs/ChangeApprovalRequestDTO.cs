using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutOfOffice.Application.Contracts.DTOs.ChangeDTOs
{
    public class ChangeApprovalRequestDTO
    {
        public int Id { get; set; }
        public string Status { get; set;}
        public string Comment { get; set;}
    }
}
