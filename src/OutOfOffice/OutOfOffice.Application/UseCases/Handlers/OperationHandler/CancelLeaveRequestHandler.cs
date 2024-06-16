using MassTransit;
using MediatR;
using MessageBus.Messages;
using Microsoft.EntityFrameworkCore;
using OutOfOffice.Application.UseCases.Commands;
using OutOfOffice.Domain.Approval_Requests.Enums;
using OutOfOffice.Domain.Employees.Enums;
using OutOfOffice.Domain.Leave_Requests.Enums;
using OutOfOffice.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutOfOffice.Application.UseCases.Handlers.OperationHandler
{
    public class CancelLeaveRequestHandler : IRequestHandler<CancelLeaveRequestCommand>
    {
        private readonly IMediator mediator;

        private readonly OutOfOfficeDbContext dbContext;
        private readonly IPublishEndpoint _publisher;

        public CancelLeaveRequestHandler(OutOfOfficeDbContext dbContext, IMediator mediator, IPublishEndpoint publisher)
        {
            this.dbContext = dbContext;
            this.mediator = mediator;
            this._publisher = publisher;
        }

        public async Task Handle(CancelLeaveRequestCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await dbContext.Employees.FirstOrDefaultAsync(x => x.Id == request.userId);

                if (user != null)
                {
                    var leaveRequest = await dbContext.LeaveRequests.FirstOrDefaultAsync(x=>x.Id==request.model.Id);
                    if (leaveRequest.Status == LeaveRequestStatus.Submitted)
                    {
                        leaveRequest.Status = LeaveRequestStatus.Canceled;

                        var approvalRequest = await dbContext.ApprovalRequests.FirstOrDefaultAsync(x => x.LeaveRequestId == leaveRequest.Id);
                        approvalRequest.Status = ApprovalRequestStatus.Canceled;
                        await dbContext.SaveChangesAsync();
                    }
                    
                }

              
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while creating user: {ex.Message}");
                throw ex;
            }
        }
    }
}