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

namespace OutOfOffice.Application.UseCases.Handlers.QueryHandlers
{
    public class GetSortedApprovalRequestHandler : IRequestHandler<GetSortedApprovalRequestQuery, List<GiveApprovalRequestDTO>>
    {

        private readonly OutOfOfficeDbContext dbContext;

        public GetSortedApprovalRequestHandler(OutOfOfficeDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<GiveApprovalRequestDTO>> Handle(GetSortedApprovalRequestQuery request, CancellationToken cancellationToken)
        {
            try
            {

                var query = dbContext.ApprovalRequests.Where(x => x.ApproverId == request.userId);
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
                        throw new ArgumentException($"Column '{request.model.ColumnName}' does not exist in the LeaveRequests model :(((");
                    }

                    if (request.model.Descending)
                    {
                        query = query.OrderByDescending(e => EF.Property<object>(e, request.model.ColumnName));
                    }
                    else
                    {
                        query = query.OrderBy(e => EF.Property<object>(e, request.model.ColumnName));
                    }
                }

                var result = await query
                    .Select(p => new GiveApprovalRequestDTO
                    {
                        Id = p.Id,
                        ApproverId = p.ApproverId,
                        LeaveRequestId=p.LeaveRequestId,
                        GiveLeaveRequestDTO = dbContext.LeaveRequests
                        .Where(e => e.Id == p.LeaveRequestId)
                        .Select(e => new GiveLeaveRequestDTO 
                        {
                            Id = e.Id,
                            EmployeeId = dbContext.Employees.Where(x => x.Id == e.EmployeeId)
                                .Select(g => g.FullName)
                                .FirstOrDefault(),
                            AbsenceReason = e.AbsenceReason.ToString(),
                            StartDate= e.StartDate,
                            EndDate=e.EndDate,
                            Comment = e.Comment,
                            Status = e.Status.ToString()
                        })
                        .FirstOrDefault(),
                        Comment = p.Comment,
                        Status = p.Status.ToString()
                    })
                    .ToListAsync();

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Pe4al kolu popalu v GetSortedUserLeaveRequestsHandler: {ex.Message}");
                return null;
            }
        }

    }
}