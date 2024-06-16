using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OutOfOffice.Application.UseCases.Commands;
using OutOfOffice.Domain;
using OutOfOffice.Domain.Approval_Requests.Enums;
using OutOfOffice.Domain.ApprovalRequests;
using OutOfOffice.Domain.Employees.Enums;
using OutOfOffice.Domain.Leave_Requests;
using OutOfOffice.Domain.Leave_Requests.Enums;
using OutOfOffice.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutOfOffice.Application.UseCases.Handlers.OperationHandler
{
    public class AddLeaveRequestHandler : IRequestHandler<AddLeaveRequestCommand, bool>
    {
        private readonly IMediator mediator;

        private readonly OutOfOfficeDbContext dbContext;
        private readonly IPublishEndpoint _publisher;

        public AddLeaveRequestHandler(OutOfOfficeDbContext dbContext, IMediator mediator, IPublishEndpoint publisher)
        {
            this.dbContext = dbContext;
            this.mediator = mediator;
            this._publisher = publisher;
        }

        public async Task<bool> Handle(AddLeaveRequestCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await dbContext.Employees.FirstOrDefaultAsync(X => X.Id == request.model.Id);
 
                LeaveRequest leaveRequest = new LeaveRequest
                {
                    EmployeeId=user.Id,
                    AbsenceReason = (AbsenceReason)Enum.Parse(typeof(AbsenceReason), request.model.AbsenceReason),
                    StartDate = request.model.StartDate,
                    EndDate = request.model.EndDate,
                    Comment = request.model.Comment,
                    Status = LeaveRequestStatus.Submitted,
                };

                var model = await dbContext.LeaveRequests.AddAsync(leaveRequest);
                await dbContext.SaveChangesAsync();


                ApprovalRequest approvalRequest = new ApprovalRequest
                {
                    LeaveRequestId = model.Entity.Id,
                    Status = ApprovalRequestStatus.New
                };

                await dbContext.ApprovalRequests.AddAsync(approvalRequest);
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