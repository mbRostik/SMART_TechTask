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

        private readonly OutOfOfficeDbContext dbContext;
        private readonly IPublishEndpoint _publisher;

        public AddEmployeeToTheProjectHandler(OutOfOfficeDbContext dbContext, IMediator mediator, IPublishEndpoint publisher)
        {
            this.dbContext = dbContext;
            this.mediator = mediator;
            this._publisher = publisher;
        }

        public async Task<bool> Handle(AddEmployeeToTheProjectCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var employee = await dbContext.Employees
                    .Where(x => x.FullName.Contains(request.model.UserName))
                    .FirstOrDefaultAsync();

                if(employee == null)
                {
                    return false;
                }

                var project = await dbContext.Projects.FirstOrDefaultAsync(x=>x.Id==request.model.Id);

                EmployeeProject temp = new EmployeeProject
                {
                    EmployeeId = employee.Id,
                    ProjectId = project.Id
                };
                await dbContext.EmployeeProjects.AddAsync(temp);
                await dbContext.SaveChangesAsync(cancellationToken);
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
