using MassTransit;
using MediatR;
using MessageBus.Messages;
using Microsoft.EntityFrameworkCore;
using OutOfOffice.Application.UseCases.Commands;
using OutOfOffice.Domain.Employees.Enums;
using OutOfOffice.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutOfOffice.Application.UseCases.Handlers.OperationHandler
{
    public class ChangeProfileHandler : IRequestHandler<ChangeProfileCommand, bool>
    {
        private readonly IMediator mediator;

        private readonly OutOfOfficeDbContext dbContext;
        private readonly IPublishEndpoint _publisher;

        public ChangeProfileHandler(OutOfOfficeDbContext dbContext, IMediator mediator, IPublishEndpoint publisher)
        {
            this.dbContext = dbContext;
            this.mediator = mediator;
            this._publisher = publisher;
        }

        public async Task<bool> Handle(ChangeProfileCommand request, CancellationToken cancellationToken)
        {
            try
            {
               var user = await dbContext.Employees.FirstOrDefaultAsync(x=>x.Id==request.model.Id);
               
               var partner = await dbContext.Employees
                    .Where(x => x.FullName.Contains(request.model.Partner))
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    return false;
                }

                user.PeoplePartnerID = partner.Id;
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
