using MediatR;
using OutOfOffice.Application.Contracts.DTOs.GiveDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutOfOffice.Application.UseCases.Commands
{
    public record ChangeEmployeeInformationCommand(GiveUserProfileDTO model) : IRequest<bool>;

}
