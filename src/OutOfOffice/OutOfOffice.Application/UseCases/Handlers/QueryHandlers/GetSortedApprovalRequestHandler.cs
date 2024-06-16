using MediatR;
using Microsoft.EntityFrameworkCore;
using OutOfOffice.Application.Contracts.DTOs.GiveDTOs;
using OutOfOffice.Application.UseCases.Queries;
using OutOfOffice.Domain.Leave_Requests.Enums;
using OutOfOffice.Domain.Leave_Requests;
using OutOfOffice.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OutOfOffice.Domain.Approval_Requests.Enums;
using OutOfOffice.Domain.ApprovalRequests;
using OutOfOffice.Domain.Employees.Enums;

namespace OutOfOffice.Application.UseCases.Handlers.QueryHandlers
{
    public class GetSortedApprovalRequestHandler : IRequestHandler<GetSortedApprovalRequestQuery, List<GiveApprovalRequestDTO>>
    {
        public readonly Serilog.ILogger logger;

        private readonly OutOfOfficeDbContext dbContext;

        public GetSortedApprovalRequestHandler(OutOfOfficeDbContext dbContext, Serilog.ILogger logger)
        {
            this.dbContext = dbContext;
            this.logger = logger;
        }

        public async Task<List<GiveApprovalRequestDTO>> Handle(GetSortedApprovalRequestQuery request, CancellationToken cancellationToken)
        {
            logger.Information("Handling GetSortedApprovalRequestQuery for UserId: {UserId}", request.userId);

            try
            {
                var userModel = await dbContext.Employees.FirstOrDefaultAsync(x => x.Id == request.userId);

                if (userModel == null)
                {
                    logger.Warning("User with Id {UserId} not found", request.userId);
                    return null;
                }

                IQueryable<ApprovalRequest> query;

                if (userModel.Position == Position.HRManager)
                {
                    logger.Information("User is HRManager, fetching ApprovalRequests related to HRManager");

                    query = from approvalRequest in dbContext.ApprovalRequests
                            join leaveRequest in dbContext.LeaveRequests on approvalRequest.LeaveRequestId equals leaveRequest.Id
                            join employee in dbContext.Employees on leaveRequest.EmployeeId equals employee.Id
                            where employee.PeoplePartnerID == request.userId
                            select approvalRequest;
                }
                else if (userModel.Position == Position.PMManager)
                {
                    logger.Information("User is PMManager, fetching ApprovalRequests related to PMManager");

                    query = from approvalRequest in dbContext.ApprovalRequests
                            join leaveRequest in dbContext.LeaveRequests on approvalRequest.LeaveRequestId equals leaveRequest.Id
                            join employeeProject in dbContext.EmployeeProjects on leaveRequest.EmployeeId equals employeeProject.EmployeeId
                            join project in dbContext.Projects on employeeProject.ProjectId equals project.Id
                            where project.ProjectManagerId == request.userId
                            select approvalRequest;
                }
                else if (userModel.Position == Position.Administrator)
                {
                    logger.Information("User is Administrator, fetching all ApprovalRequests");
                    query = dbContext.ApprovalRequests.AsQueryable();
                }
                else
                {
                    logger.Warning("User position is not authorized to fetch ApprovalRequests");
                    return null;
                }

                if (!string.IsNullOrEmpty(request.model.Status))
                {
                    if (Enum.TryParse<ApprovalRequestStatus>(request.model.Status, out var status))
                    {
                        query = query.Where(e => e.Status == status);
                    }
                }

                if (!string.IsNullOrEmpty(request.model.ColumnName))
                {
                    var propertyInfo = typeof(ApprovalRequest).GetProperty(request.model.ColumnName);
                    if (propertyInfo == null)
                    {
                        var message = $"Column '{request.model.ColumnName}' does not exist in the ApprovalRequests model";
                        logger.Error(message);
                        throw new ArgumentException(message);
                    }

                    query = request.model.Descending
                        ? query.OrderByDescending(e => EF.Property<object>(e, request.model.ColumnName))
                        : query.OrderBy(e => EF.Property<object>(e, request.model.ColumnName));
                }

                var result = await query
                    .Select(p => new GiveApprovalRequestDTO
                    {
                        Id = p.Id,
                        ApproverId = p.ApproverId,
                        LeaveRequestId = p.LeaveRequestId,
                        GiveLeaveRequestDTO = dbContext.LeaveRequests
                            .Where(e => e.Id == p.LeaveRequestId)
                            .Select(e => new GiveLeaveRequestDTO
                            {
                                Id = e.Id,
                                EmployeeId = dbContext.Employees.Where(x => x.Id == e.EmployeeId)
                                    .Select(g => g.FullName)
                                    .FirstOrDefault(),
                                AbsenceReason = e.AbsenceReason.ToString(),
                                StartDate = e.StartDate,
                                EndDate = e.EndDate,
                                Comment = e.Comment ?? string.Empty,
                                Status = e.Status.ToString()
                            })
                            .FirstOrDefault(),
                        Comment = p.Comment ?? string.Empty,
                        Status = p.Status.ToString()
                    })
                    .ToListAsync(cancellationToken);

                logger.Information("Successfully fetched {Count} ApprovalRequests for UserId: {UserId}", result.Count, request.userId);

                return result;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error occurred while handling GetSortedApprovalRequestQuery for UserId: {UserId}", request.userId);
                return null;
            }
        }
    }
}