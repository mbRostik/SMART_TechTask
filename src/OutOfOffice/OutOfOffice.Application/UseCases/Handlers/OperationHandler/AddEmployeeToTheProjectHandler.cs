using MassTransit;
using MediatR;
using MessageBus.Messages;
using Microsoft.EntityFrameworkCore;
using OutOfOffice.Application.UseCases.Commands;
using OutOfOffice.Domain;
using OutOfOffice.Domain.Employees.Enums;
using OutOfOffice.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutOfOffice.Application.UseCases.Handlers.OperationHandler
{
    public class AddEmployeeToTheProjectHandler : IRequestHandler<AddEmployeeToTheProjectCommand, bool>
    {
        private readonly IMediator mediator;
        public readonly Serilog.ILogger _logger;

        private readonly OutOfOfficeDbContext dbContext;
        private readonly IPublishEndpoint _publisher;

        public AddEmployeeToTheProjectHandler(OutOfOfficeDbContext dbContext, IMediator mediator, IPublishEndpoint publisher, Serilog.ILogger logger)
        {
            this.dbContext = dbContext;
            this.mediator = mediator;
            this._publisher = publisher;
            this._logger = logger;
        }

        public async Task<bool> Handle(AddEmployeeToTheProjectCommand request, CancellationToken cancellationToken)
        {
            _logger.Information("Handle method started with request: {Request}", request);

            try
            {
                var employee = await dbContext.Employees
                    .Where(x => x.FullName.Contains(request.model.UserName))
                    .FirstOrDefaultAsync(cancellationToken);
                _logger.Information("Fetched employee with UserName: {UserName}", request.model.UserName);

                if (employee == null)
                {
                    _logger.Warning("Employee not found with UserName: {UserName}", request.model.UserName);
                    return false;
                }

                var project = await dbContext.Projects.FirstOrDefaultAsync(x => x.Id == request.model.Id, cancellationToken);
                _logger.Information("Fetched project with Id: {ProjectId}", request.model.Id);

                if (project == null)
                {
                    _logger.Warning("Project not found with Id: {ProjectId}", request.model.Id);
                    return false;
                }

                var temp = new EmployeeProject
                {
                    EmployeeId = employee.Id,
                    ProjectId = project.Id
                };
                await dbContext.EmployeeProjects.AddAsync(temp, cancellationToken);
                await dbContext.SaveChangesAsync(cancellationToken);
                _logger.Information("Employee {EmployeeId} added to project {ProjectId}", employee.Id, project.Id);

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while adding employee to the project for UserName: {UserName} and ProjectId: {ProjectId}", request.model.UserName, request.model.Id);
                return false;
            }
        }
    }
}
