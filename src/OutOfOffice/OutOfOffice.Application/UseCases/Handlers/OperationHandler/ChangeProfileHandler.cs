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
        public readonly Serilog.ILogger _logger;

        public ChangeProfileHandler(OutOfOfficeDbContext dbContext, IMediator mediator, IPublishEndpoint publisher, Serilog.ILogger logger)
        {
            this.dbContext = dbContext;
            this.mediator = mediator;
            this._publisher = publisher;
            _logger = logger;
        }

        public async Task<bool> Handle(ChangeProfileCommand request, CancellationToken cancellationToken)
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

                var partner = await dbContext.Employees
                    .Where(x => x.FullName.Contains(request.model.Partner))
                    .FirstOrDefaultAsync(cancellationToken);
                _logger.Information("Fetched partner with name containing: {PartnerName}", request.model.Partner);

                if (partner != null && partner.Position == Position.HRManager)
                {
                    user.PeoplePartnerID = partner.Id;
                    _logger.Information("Updated PeoplePartnerID for UserId: {UserId} to {PeoplePartnerID}", request.model.Id, partner.Id);

                    await dbContext.SaveChangesAsync(cancellationToken);
                    _logger.Information("Changes saved to the database for UserId: {UserId}", user.Id);

                    return true;
                }
                else
                {
                    _logger.Warning("Partner not found or not an HRManager for UserId: {UserId}", request.model.Id);
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while changing profile for UserId: {UserId}", request.model.Id);
                return false;
            }
        }
    }
}
