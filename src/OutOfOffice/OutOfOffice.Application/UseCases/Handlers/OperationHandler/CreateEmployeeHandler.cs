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
    public class CreateEmployeeHandler : IRequestHandler<CreateEmployeeCommand, bool>
    {
        private readonly IMediator mediator;
        private readonly Serilog.ILogger _logger;

        private readonly OutOfOfficeDbContext dbContext;
        private readonly IPublishEndpoint _publisher;

        public CreateEmployeeHandler(OutOfOfficeDbContext dbContext, IMediator mediator, IPublishEndpoint publisher, Serilog.ILogger logger)
        {
            this.dbContext = dbContext;
            this.mediator = mediator;
            this._publisher = publisher;
            _logger = logger;
        }

        public async Task<bool> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
        {
            _logger.Information("Handle method started with request: {Request}", request);

            try
            {
                var temp = new Employee
                {
                    Id = GenerateRandomString(),
                    FullName = request.model.FullName,
                    Position = (Position)Enum.Parse(typeof(Position), request.model.Position),
                    Status = (EmployeeStatus)Enum.Parse(typeof(EmployeeStatus), request.model.Status),
                    Subdivision = (Subdivision)Enum.Parse(typeof(Subdivision), request.model.Subdivision),
                    OutOfOfficeBalance = request.model.OutOfOfficeBalance,
                    Photo = new byte[0]
                };
                _logger.Information("Created new employee instance with Id: {EmployeeId}", temp.Id);

                var partner = await dbContext.Employees.FirstOrDefaultAsync(x => x.Id == request.model.PeoplePartnerID, cancellationToken);
                if (partner != null && partner.Position == Position.HRManager)
                {
                    temp.PeoplePartnerID = partner.Id;
                    _logger.Information("Assigned PeoplePartnerID: {PeoplePartnerID} to employee with Id: {EmployeeId}", partner.Id, temp.Id);
                }

                await dbContext.Employees.AddAsync(temp, cancellationToken);
                await dbContext.SaveChangesAsync(cancellationToken);
                _logger.Information("Employee created and saved to the database with Id: {EmployeeId}", temp.Id);

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while creating employee with FullName: {FullName}", request.model.FullName);
                return false;
            }
        }

        private static string GenerateRandomString()
        {
            const string chars = "qwertyuiopasdfghjklzxcvbnm0123456789-";
            var random = new Random();
            var randomString = new string(Enumerable.Repeat(chars, 35)
              .Select(s => s[random.Next(s.Length)]).ToArray());
            return randomString;
        }
    }
}