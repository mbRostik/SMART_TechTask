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

        public ProfilePhotoChangedHandler(OutOfOfficeDbContext dbContext, IMediator mediator)
        {
            this.dbContext = dbContext;
            this.mediator = mediator;
        }

        public async Task Handle(ChangeUserAvatarCommand request, CancellationToken cancellationToken)
        {
            try
            {
                Console.WriteLine("Attempting to change avatar for user ID");

                var user = await dbContext.Employees.FindAsync(new object[] { request.model.Id }, cancellationToken);
                if (user != null)
                {
                    user.Photo = Convert.FromBase64String(request.model.Avatar);
                    await dbContext.SaveChangesAsync();

                    Console.WriteLine("Avatar changed successfully for user ID. Fetching updated user profile.");

                }
                else
                {
                    Console.WriteLine("User with ID not found. Cannot change avatar.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while changing avatar for user ID" + ex);
                throw;
            }
        }
    }
}
