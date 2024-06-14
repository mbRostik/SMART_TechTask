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

namespace OutOfOffice.Application.UseCases.Handlers.OperationHandler
{
    public class CreateEmployeeHandler : IRequestHandler<CreateEmployeeCommand, bool>
    {
        private readonly IMediator mediator;

        private readonly OutOfOfficeDbContext dbContext;
        private readonly IPublishEndpoint _publisher;

        public CreateEmployeeHandler(OutOfOfficeDbContext dbContext, IMediator mediator, IPublishEndpoint publisher)
        {
            this.dbContext = dbContext;
            this.mediator = mediator;
            this._publisher = publisher;
        }

        public async Task<bool> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                Employee temp = new Employee
                {
                    Id = GenerateRandomString(),
                    FullName = request.model.FullName,
                    Position = (Position)Enum.Parse(typeof(Position), request.model.Position),
                    Status = (EmployeeStatus)Enum.Parse(typeof(EmployeeStatus), request.model.Status),
                    Subdivision = (Subdivision)Enum.Parse(typeof(Subdivision), request.model.Subdivision),
                    OutOfOfficeBalance = request.model.OutOfOfficeBalance,
                    Photo = new byte[0]
                };

                await dbContext.Employees.AddAsync(temp);
                await dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while creating user: {ex.Message}");
                return false;
            }
        }

        private static string GenerateRandomString()
        {
            string chars = "qwertyuiopasdfghjklzxcvbnm0123456789-";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 35)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
