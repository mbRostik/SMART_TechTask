using MediatR;
using OutOfOffice.Domain;
using OutOfOffice.Domain.Employees;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutOfOffice.Application.UseCases.Commands
{
    public record CreateUnRegisteredUserCommand(UnRegisteredUser model) : IRequest;

}
