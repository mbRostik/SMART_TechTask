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

        private readonly OutOfOfficeDbContext dbContext;
        private readonly IPublishEndpoint _publisher;

        public ChangeProjectHandler(OutOfOfficeDbContext dbContext, IMediator mediator, IPublishEndpoint publisher)
        {
            this.dbContext = dbContext;
            this.mediator = mediator;
            this._publisher = publisher;
        }

        public async Task<bool> Handle(ChangeProjectCommand request, CancellationToken cancellationToken)
        {
            try
            {
                Console.WriteLine(request.model.ProjectType);

                var project = await dbContext.Projects.FirstOrDefaultAsync(x=>x.Id==request.model.Id);


                var manager = await dbContext.Employees
                     .Where(x => x.FullName.Contains(request.model.ProjectManagerId))
                     .FirstOrDefaultAsync();

                project.StartDate=request.model.StartDate;
                project.EndDate=request.model.EndDate;
                project.Comment = request.model.Comment;
                project.ProjectType = (ProjectType)Enum.Parse(typeof(ProjectType), request.model.ProjectType);

                if (manager!=null) { project.ProjectManagerId = manager.Id; };

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
