using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OutOfOffice.Application.UseCases.Commands;
using OutOfOffice.Domain.Employees.Enums;
using OutOfOffice.Domain.Projects.Enums;
using OutOfOffice.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutOfOffice.Application.UseCases.Handlers.OperationHandler
{
    public class ChangeProjectHandler : IRequestHandler<ChangeProjectCommand, bool>
    {
        private readonly IMediator mediator;
        public readonly Serilog.ILogger _logger;

        private readonly OutOfOfficeDbContext dbContext;
        private readonly IPublishEndpoint _publisher;

        public ChangeProjectHandler(OutOfOfficeDbContext dbContext, IMediator mediator, IPublishEndpoint publisher, Serilog.ILogger logger)
        {
            this.dbContext = dbContext;
            this.mediator = mediator;
            this._publisher = publisher;
            _logger = logger;
        }

        public async Task<bool> Handle(ChangeProjectCommand request, CancellationToken cancellationToken)
        {
            _logger.Information("Handle method started with request: {Request}", request);

            try
            {
                _logger.Information("ProjectType: {ProjectType}", request.model.ProjectType);

                var project = await dbContext.Projects.FirstOrDefaultAsync(x => x.Id == request.model.Id, cancellationToken);
                if (project == null)
                {
                    _logger.Warning("Project not found with Id: {ProjectId}", request.model.Id);
                    return false;
                }
                _logger.Information("Fetched project with Id: {ProjectId}", request.model.Id);

                var manager = await dbContext.Employees
                    .Where(x => x.FullName.Contains(request.model.ProjectManagerId))
                    .FirstOrDefaultAsync(cancellationToken);
                if (manager == null)
                {
                    _logger.Warning("Project manager not found with name containing: {ManagerName}", request.model.ProjectManagerId);
                    return false;
                }
                _logger.Information("Fetched project manager with name containing: {ManagerName}", request.model.ProjectManagerId);

                if (manager.Position != Position.PMManager)
                {
                    _logger.Warning("Manager with Id: {ManagerId} is not a PMManager", manager.Id);
                    return false;
                }

                project.StartDate = request.model.StartDate;
                project.EndDate = request.model.EndDate;
                project.Comment = request.model.Comment;
                project.ProjectType = (ProjectType)Enum.Parse(typeof(ProjectType), request.model.ProjectType);

                if (manager != null)
                {
                    project.ProjectManagerId = manager.Id;
                    _logger.Information("Updated ProjectManagerId for ProjectId: {ProjectId} to {ManagerId}", request.model.Id, manager.Id);
                }

                await dbContext.SaveChangesAsync(cancellationToken);
                _logger.Information("Changes saved to the database for ProjectId: {ProjectId}", project.Id);

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while changing project for ProjectId: {ProjectId}", request.model.Id);
                return false;
            }
        }
    }
}
