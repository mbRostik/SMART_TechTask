using MediatR;
using Microsoft.EntityFrameworkCore;
using OutOfOffice.Application.Contracts.DTOs.GiveDTOs;
using OutOfOffice.Application.UseCases.Queries;
using OutOfOffice.Domain.Employees.Enums;
using OutOfOffice.Domain.Employees;
using OutOfOffice.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassTransit.Initializers;
using MassTransit;
using OutOfOffice.Domain.Projects;
using MassTransit.Mediator;

namespace OutOfOffice.Application.UseCases.Handlers.QueryHandlers
{
    public class GetProjectByIdHandler : IRequestHandler<GetProjectByIdQuery, GiveProjectWithDetailsDTO>
    {

        private readonly OutOfOfficeDbContext dbContext;
        private readonly MediatR.IMediator mediator;

        public GetProjectByIdHandler(OutOfOfficeDbContext dbContext, MediatR.IMediator mediator)
        {
            this.dbContext = dbContext;
            this.mediator = mediator;
        }

        public async Task<GiveProjectWithDetailsDTO> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var project = await dbContext.Projects.FirstOrDefaultAsync(x => x.Id == request.projectId);

                if (project == null)
                {
                    return null;
                }

                GiveProjectWithDetailsDTO result = new GiveProjectWithDetailsDTO
                {
                    Id = project.Id,
                    ProjectManagerId = project.ProjectManagerId,
                    StartDate = project.StartDate,
                    EndDate = project.EndDate,
                    ProjectType = project.ProjectType.ToString(),
                    Comment = project.Comment,
                    Status = project.Status.ToString()
                };

                var employeesIds = await dbContext.EmployeeProjects
                            .Where(x => x.ProjectId == project.Id)
                            .Select(x => x.EmployeeId)
                            .ToListAsync();

                var employees = await dbContext.Employees
                    .Where(x => employeesIds.Contains(x.Id))
                    .ToListAsync();

                List<GiveUserProfileDTO> giveEmp = new List<GiveUserProfileDTO>();

                foreach (var employee in employees)
                {
                    GiveUserProfileDTO temp = await mediator.Send(new GetUserProfileQuery(employee.Id));
                    giveEmp.Add(temp);
                }

                result.Employees = giveEmp;
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Pe4al kolu popalu v GetSortedEmployeeTableHandler: {ex.Message}");
                return null;
            }

        }

    }
}