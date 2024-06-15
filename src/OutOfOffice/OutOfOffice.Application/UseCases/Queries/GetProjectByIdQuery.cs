using MediatR;
using OutOfOffice.Application.Contracts.DTOs.GiveDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutOfOffice.Application.UseCases.Queries
{

    public record GetProjectByIdQuery(int projectId) : IRequest<GiveProjectWithDetailsDTO>;

}
