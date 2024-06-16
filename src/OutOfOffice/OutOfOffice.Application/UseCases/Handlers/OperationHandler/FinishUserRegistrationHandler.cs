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
        private readonly Serilog.ILogger _logger;


        public FinishUserRegistrationHandler(OutOfOfficeDbContext dbContext, IMediator mediator, IPublishEndpoint publisher, Serilog.ILogger logger)
        {
            this.dbContext = dbContext;
            this.mediator = mediator;
            this._publisher = publisher;
            _logger = logger;
        }

        public async Task<bool> Handle(FinishUserRegistrationCommand request, CancellationToken cancellationToken)
        {
            _logger.Information("Handle method started with request: {Request}", request);

            try
            {
                var unRegUser = await dbContext.UnRegisteredUsers.FirstOrDefaultAsync(x => x.Id == request.userId, cancellationToken);
                if (unRegUser == null)
                {
                    _logger.Warning("Unregistered user not found with Id: {UserId}", request.userId);
                    throw new Exception("Unregistered user not found.");
                }
                _logger.Information("Fetched unregistered user with Id: {UserId}", request.userId);

                var employee = new Employee
                {
                    Id = request.userId,
                    FullName = unRegUser.UserName,
                    Position = (Position)Enum.Parse(typeof(Position), request.model.Position),
                    Status = (EmployeeStatus)Enum.Parse(typeof(EmployeeStatus), request.model.EmployeeStatus),
                    Subdivision = (Subdivision)Enum.Parse(typeof(Subdivision), request.model.Subdivision),
                    OutOfOfficeBalance = request.model.DayOffCount,
                    Photo = new byte[0]
                };
                _logger.Information("Created new employee instance with Id: {UserId}", request.userId);

                if (!string.IsNullOrEmpty(request.model.PartnerName))
                {
                    var partner = await dbContext.Employees
                        .Where(x => x.FullName.Contains(request.model.PartnerName))
                        .FirstOrDefaultAsync(cancellationToken);
                    if (partner != null && partner.Position == Position.HRManager)
                    {
                        employee.PeoplePartnerID = partner.Id;
                        _logger.Information("Assigned PeoplePartnerID: {PeoplePartnerID} to employee with Id: {UserId}", partner.Id, request.userId);
                    }
                    else
                    {
                        employee.PeoplePartnerID = null;
                        _logger.Warning("Partner not found or not an HRManager for PartnerName: {PartnerName}", request.model.PartnerName);
                    }
                }
                else
                {
                    employee.PeoplePartnerID = null;
                    _logger.Information("No PartnerName provided, PeoplePartnerID set to null for employee with Id: {UserId}", request.userId);
                }

                var model = await dbContext.Employees.AddAsync(employee, cancellationToken);
                _logger.Information("Employee added to the database with Id: {UserId}", request.userId);

                var userPositionSetEvent = new UserPositionSetEvent
                {
                    UserId = request.userId,
                    Position = model.Entity.Position.ToString()
                };
                await _publisher.Publish(userPositionSetEvent, cancellationToken);
                _logger.Information("Published UserPositionSetEvent for UserId: {UserId}", request.userId);

                dbContext.UnRegisteredUsers.Remove(unRegUser);
                await dbContext.SaveChangesAsync(cancellationToken);
                _logger.Information("Removed unregistered user and saved changes to the database for UserId: {UserId}", request.userId);

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while finishing user registration for UserId: {UserId}", request.userId);
                return false;
            }
        }
    }
}
