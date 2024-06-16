using MassTransit;
using MediatR;
using MessageBus.Messages;
using Microsoft.EntityFrameworkCore;
using OutOfOffice.Application.UseCases.Commands;
using OutOfOffice.Domain.Approval_Requests.Enums;
using OutOfOffice.Domain.Employees.Enums;
using OutOfOffice.Domain.Leave_Requests;
using OutOfOffice.Domain.Leave_Requests.Enums;
using OutOfOffice.Domain.Projects.Enums;
using OutOfOffice.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutOfOffice.Application.UseCases.Handlers.OperationHandler
{
    public class ChangeApprovalRequestHandler : IRequestHandler<ChangeApprovalRequestCommand, bool>
    {
        private readonly IMediator mediator;

        private readonly OutOfOfficeDbContext dbContext;
        private readonly IPublishEndpoint _publisher;

        public ChangeApprovalRequestHandler(OutOfOfficeDbContext dbContext, IMediator mediator, IPublishEndpoint publisher)
        {
            this.dbContext = dbContext;
            this.mediator = mediator;
            this._publisher = publisher;
        }

        public async Task<bool> Handle(ChangeApprovalRequestCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.model.Status == "New")
                {
                    return false;
                }
                var apRequest = await dbContext.ApprovalRequests.FirstOrDefaultAsync(x => x.Id == request.model.Id);

                apRequest.Comment = request.model.Comment;
                apRequest.Status = (ApprovalRequestStatus)Enum.Parse(typeof(ApprovalRequestStatus), request.model.Status);
                apRequest.ApproverId=request.userId;

                var leavRequest = await dbContext.LeaveRequests.FirstOrDefaultAsync(x => x.Id == apRequest.LeaveRequestId);
                leavRequest.Status = (LeaveRequestStatus)Enum.Parse(typeof(LeaveRequestStatus), request.model.Status);
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