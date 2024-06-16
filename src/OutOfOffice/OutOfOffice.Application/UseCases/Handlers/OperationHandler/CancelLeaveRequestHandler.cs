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
        public readonly Serilog.ILogger _logger;

        private readonly OutOfOfficeDbContext dbContext;
        private readonly IPublishEndpoint _publisher;

        public CancelLeaveRequestHandler(OutOfOfficeDbContext dbContext, IMediator mediator, IPublishEndpoint publisher, Serilog.ILogger logger)
        {
            this.dbContext = dbContext;
            this.mediator = mediator;
            this._publisher = publisher;
            _logger = logger;
        }

        public async Task Handle(CancelLeaveRequestCommand request, CancellationToken cancellationToken)
        {
            _logger.Information("Handle method started with request: {Request}", request);

            try
            {
                var user = await dbContext.Employees.FirstOrDefaultAsync(x => x.Id == request.userId, cancellationToken);
                if (user == null)
                {
                    _logger.Warning("User not found with Id: {UserId}", request.userId);
                    return;
                }
                _logger.Information("Fetched user with Id: {UserId}", request.userId);

                var leaveRequest = await dbContext.LeaveRequests.FirstOrDefaultAsync(x => x.Id == request.model.Id, cancellationToken);
                if (leaveRequest == null)
                {
                    _logger.Warning("Leave request not found with Id: {LeaveRequestId}", request.model.Id);
                    return;
                }
                _logger.Information("Fetched leave request with Id: {LeaveRequestId}", request.model.Id);

                if (leaveRequest.Status == LeaveRequestStatus.Submitted)
                {
                    leaveRequest.Status = LeaveRequestStatus.Canceled;
                    _logger.Information("Leave request status changed to Canceled for Id: {LeaveRequestId}", leaveRequest.Id);

                    var approvalRequest = await dbContext.ApprovalRequests.FirstOrDefaultAsync(x => x.LeaveRequestId == leaveRequest.Id, cancellationToken);
                    if (approvalRequest != null)
                    {
                        approvalRequest.Status = ApprovalRequestStatus.Canceled;
                        _logger.Information("Approval request status changed to Canceled for LeaveRequestId: {LeaveRequestId}", leaveRequest.Id);
                    }
                    else
                    {
                        _logger.Warning("Approval request not found for LeaveRequestId: {LeaveRequestId}", leaveRequest.Id);
                    }

                    await dbContext.SaveChangesAsync(cancellationToken);
                    _logger.Information("Changes saved to the database for LeaveRequestId: {LeaveRequestId}", leaveRequest.Id);
                }
                else
                {
                    _logger.Warning("Leave request status is not Submitted for LeaveRequestId: {LeaveRequestId}", leaveRequest.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while canceling leave request for UserId: {UserId} and LeaveRequestId: {LeaveRequestId}", request.userId, request.model.Id);
                throw;
            }
        }
    }
}