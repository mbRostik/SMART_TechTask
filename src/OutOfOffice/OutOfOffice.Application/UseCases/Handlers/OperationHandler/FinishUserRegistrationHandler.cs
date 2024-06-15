using MassTransit;
using MassTransit.Initializers;
using MediatR;
using MessageBus.Messages;
using Microsoft.EntityFrameworkCore;
using OutOfOffice.Application.UseCases.Commands;
using OutOfOffice.Domain.Employees;
using OutOfOffice.Domain.Employees.Enums;
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

                var partnerId = await dbContext.Employees.FirstOrDefaultAsync(x => x.FullName == request.model.PartnerName).Select(x=>x.Id);
                var partner = await dbContext.Employees
                    .Where(x => x.FullName.Contains(request.model.PartnerName))
                    .FirstOrDefaultAsync();
                Employee employee = new Employee
                {
                    Id = request.userId,
                    FullName = unRegUser.UserName,
                    Position = (Position)Enum.Parse(typeof(Position), request.model.Position),
                    Status = (EmployeeStatus)Enum.Parse(typeof(EmployeeStatus), request.model.EmployeeStatus),
                    Subdivision = (Subdivision)Enum.Parse(typeof(Subdivision), request.model.Subdivision),
                    OutOfOfficeBalance = request.model.DayOffCount,
                    Photo = new byte[0]
                };

                if (partner != null && partner.Position == Position.HRManager)
                {
                    employee.PeoplePartnerID = partner.Id;
                }
                if (partner == null)
                {
                    employee.PeoplePartnerID =null;

                }
                var model = await dbContext.Employees.AddAsync(employee);

                UserPositionSetEvent userPositionSetEvent = new UserPositionSetEvent
                {
                    UserId = request.userId,
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
