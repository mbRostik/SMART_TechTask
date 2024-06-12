using MassTransit;
using MassTransit.Initializers;
using MediatR;
using MessageBus.Messages;
using Microsoft.EntityFrameworkCore;
using OutOfOffice.Application.UseCases.Commands;
using OutOfOffice.Domain.Employees;
using OutOfOffice.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutOfOffice.Application.UseCases.Handlers.OperationHandler
{
    public class FinishUserRegistrationHandler : IRequestHandler<FinishUserRegistrationCommand, bool>
    {
        private readonly IMediator mediator;

        private readonly OutOfOfficeDbContext dbContext;
        private readonly IPublishEndpoint _publisher;

        public FinishUserRegistrationHandler(OutOfOfficeDbContext dbContext, IMediator mediator, IPublishEndpoint publisher)
        {
            this.dbContext = dbContext;
            this.mediator = mediator;
            this._publisher = publisher;
        }

        public async Task<bool> Handle(FinishUserRegistrationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var unRegUser = await dbContext.UnRegisteredUsers.FirstOrDefaultAsync(x => x.Id == request.userId);
                if (unRegUser == null)
                {
                    throw new Exception("Unregistered user not found.");
                }

                var partner = await dbContext.Employees.FirstOrDefaultAsync(x => x.FullName == request.model.ParnerName);
                if (partner == null)
                {
                    throw new Exception("Partner not found.");
                }

                Employee employee = new Employee
                {
                    Id = request.userId,
                    FullName = unRegUser.UserName,
                    PeoplePartnerID = partner.Id,
                    Position = request.model.Position,
                    Status = request.model.EmployeeStatus,
                    Subdivision = request.model.Subdivision,
                    OutOfOfficeBalance = request.model.DayOffCount,
                    Photo = new byte[0]
                };

                var model = await dbContext.Employees.AddAsync(employee);

                UserPositionSetEvent userPositionSetEvent = new UserPositionSetEvent
                {
                    Position = model.Entity.Position.ToString()
                };
                await _publisher.Publish(userPositionSetEvent);

                dbContext.UnRegisteredUsers.Remove(unRegUser);

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
