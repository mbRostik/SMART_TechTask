using MediatR;
using OutOfOffice.Application.Contracts.DTOs.ChangeDTOs;
using OutOfOffice.Domain.Employees;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutOfOffice.Application.UseCases.Commands
{
    public record FinishUserRegistrationCommand(FinishUserRegistrationDTO model, string userId) : IRequest<bool>;

}
