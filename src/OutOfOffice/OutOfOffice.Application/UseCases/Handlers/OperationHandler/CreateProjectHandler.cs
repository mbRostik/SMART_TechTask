using MassTransit;
using MediatR;
using OutOfOffice.Application.UseCases.Commands;
using OutOfOffice.Domain.Employees.Enums;
using OutOfOffice.Domain.Employees;
using OutOfOffice.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OutOfOffice.Domain.Projects;
using Microsoft.EntityFrameworkCore;
using OutOfOffice.Domain.Projects.Enums;

namespace OutOfOffice.Application.UseCases.Handlers.OperationHandler
{
    public class CreateProjectHandler : IRequestHandler<CreateProjectCommand, bool>
    {
        private readonly IMediator mediator;
        private readonly Serilog.ILogger _logger;

        private readonly OutOfOfficeDbContext dbContext;
        private readonly IPublishEndpoint _publisher;

        public CreateProjectHandler(OutOfOfficeDbContext dbContext, IMediator mediator, IPublishEndpoint publisher, Serilog.ILogger logger)
        {
            this.dbContext = dbContext;
            this.mediator = mediator;
            this._publisher = publisher;
            _logger = logger;
        }

        public async Task<bool> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
        {
            _logger.Information("Handle method started with request: {Request}", request);

            try
            {
                var manager = await dbContext.Employees
                    .Where(x => x.FullName.Contains(request.model.ProjectManagerId))
                    .FirstOrDefaultAsync(cancellationToken);

                if (manager == null || manager.Position != Position.PMManager)
                {
                    _logger.Warning("The project cannot be created. No manager found or the manager is not a PMManager");
                    return false;
                }
                _logger.Information("Fetched manager with Id: {ManagerId} and Position: {Position}", manager.Id, manager.Position);

                var temp = new Project
                {
                    ProjectType = (ProjectType)Enum.Parse(typeof(ProjectType), request.model.ProjectType),
                    StartDate = request.model.StartDate,
                    EndDate = request.model.EndDate,
                    ProjectManagerId = manager.Id,
                    Comment = request.model.Comment,
                    Status = (ProjectStatus)Enum.Parse(typeof(ProjectStatus), request.model.Status)
                };
                _logger.Information("Created new project instance with ProjectManagerId: {ManagerId}", manager.Id);

                await dbContext.Projects.AddAsync(temp, cancellationToken);
                await dbContext.SaveChangesAsync(cancellationToken);
                _logger.Information("Project created and saved to the database");

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while creating project with ProjectManagerId: {ProjectManagerId}", request.model.ProjectManagerId);
                return false;
            }
        }
    }
}
