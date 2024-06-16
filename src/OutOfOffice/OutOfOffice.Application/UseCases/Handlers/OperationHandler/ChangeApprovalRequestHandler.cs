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
        public readonly Serilog.ILogger _logger;

        public ChangeApprovalRequestHandler(OutOfOfficeDbContext dbContext, IMediator mediator, IPublishEndpoint publisher, Serilog.ILogger logger)
        {
            this.dbContext = dbContext;
            this.mediator = mediator;
            this._publisher = publisher;
            _logger = logger;
        }

        public async Task<bool> Handle(ChangeApprovalRequestCommand request, CancellationToken cancellationToken)
        {
            _logger.Information("Handle method started with request: {Request}", request);

            try
            {
                if (request.model.Status == "New")
                {
                    _logger.Warning("Status is 'New'. Cannot change approval request for Id: {ApprovalRequestId}", request.model.Id);
                    return false;
                }

                var apRequest = await dbContext.ApprovalRequests.FirstOrDefaultAsync(x => x.Id == request.model.Id, cancellationToken);
                if (apRequest == null)
                {
                    _logger.Warning("Approval request not found with Id: {ApprovalRequestId}", request.model.Id);
                    return false;
                }
                _logger.Information("Fetched approval request with Id: {ApprovalRequestId}", request.model.Id);

                apRequest.Comment = request.model.Comment;
                apRequest.Status = (ApprovalRequestStatus)Enum.Parse(typeof(ApprovalRequestStatus), request.model.Status);
                apRequest.ApproverId = request.userId;

                var leaveRequest = await dbContext.LeaveRequests.FirstOrDefaultAsync(x => x.Id == apRequest.LeaveRequestId, cancellationToken);
                if (leaveRequest == null)
                {
                    _logger.Warning("Leave request not found with Id: {LeaveRequestId}", apRequest.LeaveRequestId);
                    return false;
                }
                _logger.Information("Fetched leave request with Id: {LeaveRequestId}", apRequest.LeaveRequestId);

                leaveRequest.Status = (LeaveRequestStatus)Enum.Parse(typeof(LeaveRequestStatus), request.model.Status);
                await dbContext.SaveChangesAsync(cancellationToken);
                _logger.Information("Approval request and leave request statuses updated and changes saved for ApprovalRequestId: {ApprovalRequestId} and LeaveRequestId: {LeaveRequestId}", request.model.Id, apRequest.LeaveRequestId);

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while changing approval request for ApprovalRequestId: {ApprovalRequestId} and UserId: {UserId}", request.model.Id, request.userId);
                return false;
            }
        }
    }
}