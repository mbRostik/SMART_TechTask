using MassTransit;
using MediatR;
using MessageBus.Messages;
using OutOfOffice.Application.UseCases.Commands;
using OutOfOffice.Domain.Employees.Enums;
using OutOfOffice.Domain.Employees;
using OutOfOffice.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace OutOfOffice.Application.UseCases.Handlers.OperationHandler
{
    public class ChangeEmployeeInformationHandler : IRequestHandler<ChangeEmployeeInformationCommand, bool>
    {
        private readonly IMediator mediator;

        private readonly OutOfOfficeDbContext dbContext;
        private readonly IPublishEndpoint _publisher;

        public ChangeEmployeeInformationHandler(OutOfOfficeDbContext dbContext, IMediator mediator, IPublishEndpoint publisher)
        {
            this.dbContext = dbContext;
            this.mediator = mediator;
            this._publisher = publisher;
        }

        public async Task<bool> Handle(ChangeEmployeeInformationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await dbContext.Employees.FirstOrDefaultAsync(x=>x.Id==request.model.Id);

                if (user == null) 
                {
                    return false;
                }

                user.FullName = request.model.FullName;
                user.Status=(EmployeeStatus)Enum.Parse(typeof(EmployeeStatus), request.model.Status);
                user.Subdivision = (Subdivision)Enum.Parse(typeof(Subdivision), request.model.Subdivision);
                user.Position= (Position)Enum.Parse(typeof(Position), request.model.Position);
                if (request.model.OutOfOfficeBalance != null)
                {
                    user.OutOfOfficeBalance = (int)request.model.OutOfOfficeBalance;
                }

                var partner = await dbContext.Employees.FirstOrDefaultAsync(x=>x.Id== request.model.PeoplePartnerID);
                if (partner != null) { user.PeoplePartnerID = partner.Id; }

                UserChangedEvent userPositionSetEvent = new UserChangedEvent
                {
                    UserId = user.Id,
                    Position = user.Position.ToString(),
                    UserName=user.FullName
                };
                await _publisher.Publish(userPositionSetEvent);

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
