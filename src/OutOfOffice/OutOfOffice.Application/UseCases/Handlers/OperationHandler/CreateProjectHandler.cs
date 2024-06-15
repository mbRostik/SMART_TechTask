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

        private readonly OutOfOfficeDbContext dbContext;
        private readonly IPublishEndpoint _publisher;

        public CreateProjectHandler(OutOfOfficeDbContext dbContext, IMediator mediator, IPublishEndpoint publisher)
        {
            this.dbContext = dbContext;
            this.mediator = mediator;
            this._publisher = publisher;
        }

        public async Task<bool> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var manager = await dbContext.Employees
                   .Where(x => x.FullName.Contains(request.model.ProjectManagerId))
                   .FirstOrDefaultAsync();

                if (manager == null)
                {
                    Console.WriteLine("The project can not be created. There is no manager");
                    return false;
                }
                Project temp = new Project
                {
                    ProjectType = (ProjectType)Enum.Parse(typeof(ProjectType), request.model.ProjectType),
                    StartDate = request.model.StartDate,
                    EndDate = request.model.EndDate,
                    ProjectManagerId=manager.Id,
                    Comment = request.model.Comment,
                    Status = (ProjectStatus)Enum.Parse(typeof(ProjectStatus), request.model.Status)
                };

                await dbContext.Projects.AddAsync(temp);
                await dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while creating user: {ex.Message}");
                return false;
            }
        }
    }
}
