using MediatR;
using OutOfOffice.Application.UseCases.Commands;
using OutOfOffice.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutOfOffice.Application.UseCases.Handlers.OperationHandler
{
    public class UnRegisteredUserCreateHandler : IRequestHandler<CreateUnRegisteredUserCommand>
    {
        private readonly IMediator mediator;

        private readonly OutOfOfficeDbContext dbContext;

        public UnRegisteredUserCreateHandler(OutOfOfficeDbContext dbContext, IMediator mediator)
        {
            this.dbContext = dbContext;
            this.mediator = mediator;
        }

        public async Task Handle(CreateUnRegisteredUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var model = await dbContext.UnRegisteredUsers.AddAsync(request.model, cancellationToken);
                await dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred while creating user");   
                throw;
            }
        }
    }
}
