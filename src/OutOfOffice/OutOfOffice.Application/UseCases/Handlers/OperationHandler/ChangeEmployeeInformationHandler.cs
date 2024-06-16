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
        public readonly Serilog.ILogger _logger;

        public ChangeEmployeeInformationHandler(OutOfOfficeDbContext dbContext, IMediator mediator, IPublishEndpoint publisher, Serilog.ILogger logger)
        {
            this.dbContext = dbContext;
            this.mediator = mediator;
            this._publisher = publisher;
            _logger = logger;
        }

        public async Task<bool> Handle(ChangeEmployeeInformationCommand request, CancellationToken cancellationToken)
        {
            _logger.Information("Handle method started with request: {Request}", request);

            try
            {
                var user = await dbContext.Employees.FirstOrDefaultAsync(x => x.Id == request.model.Id, cancellationToken);
                if (user == null)
                {
                    _logger.Warning("User not found with Id: {UserId}", request.model.Id);
                    return false;
                }
                _logger.Information("Fetched user with Id: {UserId}", request.model.Id);

                user.FullName = request.model.FullName;
                user.Status = (EmployeeStatus)Enum.Parse(typeof(EmployeeStatus), request.model.Status);
                user.Subdivision = (Subdivision)Enum.Parse(typeof(Subdivision), request.model.Subdivision);
                user.Position = (Position)Enum.Parse(typeof(Position), request.model.Position);

                if (request.model.OutOfOfficeBalance != null)
                {
                    user.OutOfOfficeBalance = (int)request.model.OutOfOfficeBalance;
                    _logger.Information("Updated OutOfOfficeBalance for UserId: {UserId} to {OutOfOfficeBalance}", request.model.Id, user.OutOfOfficeBalance);
                }

                var partner = await dbContext.Employees.FirstOrDefaultAsync(x => x.Id == request.model.PeoplePartnerID, cancellationToken);
                if (partner != null && partner.Position == Position.HRManager)
                {
                    user.PeoplePartnerID = partner.Id;
                    _logger.Information("Updated PeoplePartnerID for UserId: {UserId} to {PeoplePartnerID}", request.model.Id, partner.Id);
                }

                var userPositionSetEvent = new UserChangedEvent
                {
                    UserId = user.Id,
                    Position = user.Position.ToString(),
                    UserName = user.FullName
                };
                await _publisher.Publish(userPositionSetEvent, cancellationToken);
                _logger.Information("Published UserChangedEvent for UserId: {UserId}", user.Id);

                await dbContext.SaveChangesAsync(cancellationToken);
                _logger.Information("Changes saved to the database for UserId: {UserId}", user.Id);

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while changing employee information for UserId: {UserId}", request.model.Id);
                return false;
            }
        }
    }
}
