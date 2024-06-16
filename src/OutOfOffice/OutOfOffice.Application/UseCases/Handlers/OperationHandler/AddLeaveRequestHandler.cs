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
        public readonly Serilog.ILogger _logger;

        public AddLeaveRequestHandler(OutOfOfficeDbContext dbContext, IMediator mediator, IPublishEndpoint publisher, Serilog.ILogger logger)
        {
            this.dbContext = dbContext;
            this.mediator = mediator;
            this._publisher = publisher;
            _logger = logger;
        }

        public async Task<bool> Handle(AddLeaveRequestCommand request, CancellationToken cancellationToken)
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

                var leaveRequest = new LeaveRequest
                {
                    EmployeeId = user.Id,
                    AbsenceReason = (AbsenceReason)Enum.Parse(typeof(AbsenceReason), request.model.AbsenceReason),
                    StartDate = request.model.StartDate,
                    EndDate = request.model.EndDate,
                    Comment = request.model.Comment,
                    Status = LeaveRequestStatus.Submitted,
                };

                var model = await dbContext.LeaveRequests.AddAsync(leaveRequest, cancellationToken);
                await dbContext.SaveChangesAsync(cancellationToken);
                _logger.Information("Leave request created for UserId: {UserId} with LeaveRequestId: {LeaveRequestId}", user.Id, model.Entity.Id);

                var approvalRequest = new ApprovalRequest
                {
                    LeaveRequestId = model.Entity.Id,
                    Status = ApprovalRequestStatus.New
                };

                await dbContext.ApprovalRequests.AddAsync(approvalRequest, cancellationToken);
                await dbContext.SaveChangesAsync(cancellationToken);
                _logger.Information("Approval request created for LeaveRequestId: {LeaveRequestId}", model.Entity.Id);

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while creating leave request for UserId: {UserId}", request.model.Id);
                return false;
            }
        }
    }
}