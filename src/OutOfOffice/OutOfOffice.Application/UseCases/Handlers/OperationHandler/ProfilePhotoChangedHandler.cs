using MediatR;
using OutOfOffice.Application.UseCases.Commands;
using OutOfOffice.Application.UseCases.Queries;
using OutOfOffice.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutOfOffice.Application.UseCases.Handlers.OperationHandler
{
    public class ProfilePhotoChangedHandler : IRequestHandler<ChangeUserAvatarCommand>
    {
        private readonly IMediator mediator;

        private readonly OutOfOfficeDbContext dbContext;
        private readonly Serilog.ILogger _logger;

        public ProfilePhotoChangedHandler(OutOfOfficeDbContext dbContext, IMediator mediator, Serilog.ILogger logger)
        {
            this.dbContext = dbContext;
            this.mediator = mediator;
            _logger = logger;
        }

        public async Task Handle(ChangeUserAvatarCommand request, CancellationToken cancellationToken)
        {
            _logger.Information("Attempting to change avatar for user ID: {UserId}", request.model.Id);

            try
            {
                var user = await dbContext.Employees.FindAsync(new object[] { request.model.Id }, cancellationToken);
                if (user != null)
                {
                    user.Photo = Convert.FromBase64String(request.model.Avatar);
                    await dbContext.SaveChangesAsync(cancellationToken);

                    _logger.Information("Avatar changed successfully for user ID: {UserId}", request.model.Id);
                }
                else
                {
                    _logger.Warning("User with ID: {UserId} not found. Cannot change avatar.", request.model.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while changing avatar for user ID: {UserId}", request.model.Id);
                throw;
            }
        }
    }
}
