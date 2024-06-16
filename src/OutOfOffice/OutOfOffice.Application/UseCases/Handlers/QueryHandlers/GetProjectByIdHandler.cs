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
        public readonly Serilog.ILogger logger;

        public GetProjectByIdHandler(OutOfOfficeDbContext dbContext, MediatR.IMediator mediator, Serilog.ILogger logger)
        {
            this.dbContext = dbContext;
            this.mediator = mediator;
            this.logger = logger;
        }

        public async Task<GiveProjectWithDetailsDTO> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
        {
            logger.Information("Handling GetProjectByIdQuery for ProjectId: {ProjectId}", request.projectId);

            try
            {
                var project = await dbContext.Projects.FirstOrDefaultAsync(x => x.Id == request.projectId);
                if (project == null)
                {
                    logger.Warning("Project with Id {ProjectId} not found", request.projectId);
                    return null;
                }

                logger.Information("Project found: {@Project}", project);

                var result = new GiveProjectWithDetailsDTO
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

                logger.Information("EmployeeIds associated with ProjectId {ProjectId}: {@EmployeeIds}", project.Id, employeesIds);

                var employees = await dbContext.Employees
                    .Where(x => employeesIds.Contains(x.Id))
                    .ToListAsync();

                logger.Information("Employees found: {@Employees}", employees);

                var giveEmp = new List<GiveUserProfileDTO>();

                foreach (var employee in employees)
                {
                    var temp = await mediator.Send(new GetUserProfileQuery(employee.Id));
                    giveEmp.Add(temp);
                    logger.Information("Fetched profile for EmployeeId {EmployeeId}: {@Profile}", employee.Id, temp);
                }

                result.Employees = giveEmp;
                logger.Information("Successfully handled GetProjectByIdQuery for ProjectId: {ProjectId}", request.projectId);

                return result;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error occurred while handling GetProjectByIdQuery for ProjectId: {ProjectId}", request.projectId);
                return null;
            }
        }

    }
}