using MediatR;
using Microsoft.EntityFrameworkCore;
using OutOfOffice.Application.Contracts.DTOs.GiveDTOs;
using OutOfOffice.Application.UseCases.Queries;
using OutOfOffice.Domain.Projects.Enums;
using OutOfOffice.Domain.Projects;
using OutOfOffice.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OutOfOffice.Domain.Leave_Requests.Enums;
using OutOfOffice.Domain.Leave_Requests;

namespace OutOfOffice.Application.UseCases.Handlers.QueryHandlers
{
    public class GetSortedUserLeaveRequestsHandler : IRequestHandler<GetSortedUserLeaveRequestsQuery, List<GiveLeaveRequestDTO>>
    {

        private readonly OutOfOfficeDbContext dbContext;
        public readonly Serilog.ILogger _logger;

        public GetSortedUserLeaveRequestsHandler(OutOfOfficeDbContext dbContext, Serilog.ILogger logger)
        {
            this.dbContext = dbContext;
            _logger = logger;
        }

        public async Task<List<GiveLeaveRequestDTO>> Handle(GetSortedUserLeaveRequestsQuery request, CancellationToken cancellationToken)
        {
            _logger.Information("Handle method started with request: {Request}", request);

            try
            {
                var query = dbContext.LeaveRequests.Where(x => x.EmployeeId == request.userId);
                _logger.Information("Filtering by EmployeeId: {EmployeeId}", request.userId);

                if (!string.IsNullOrEmpty(request.model.Status))
                {
                    if (Enum.TryParse<LeaveRequestStatus>(request.model.Status, out var status))
                    {
                        query = query.Where(e => e.Status == status);
                        _logger.Information("Filtering by Status: {Status}", status);
                    }
                }

                if (!string.IsNullOrEmpty(request.model.AbsenceReason))
                {
                    if (Enum.TryParse<AbsenceReason>(request.model.AbsenceReason, out var type))
                    {
                        query = query.Where(e => e.AbsenceReason == type);
                        _logger.Information("Filtering by AbsenceReason: {AbsenceReason}", type);
                    }
                }

                if (!string.IsNullOrEmpty(request.model.ColumnName))
                {
                    var propertyInfo = typeof(LeaveRequest).GetProperty(request.model.ColumnName);
                    if (propertyInfo == null)
                    {
                        var errorMessage = $"Column '{request.model.ColumnName}' does not exist in the LeaveRequests model";
                        _logger.Error(errorMessage);
                        throw new ArgumentException(errorMessage);
                    }

                    if (request.model.Descending)
                    {
                        query = query.OrderByDescending(e => EF.Property<object>(e, request.model.ColumnName));
                        _logger.Information("Sorting by {ColumnName} in descending order", request.model.ColumnName);
                    }
                    else
                    {
                        query = query.OrderBy(e => EF.Property<object>(e, request.model.ColumnName));
                        _logger.Information("Sorting by {ColumnName} in ascending order", request.model.ColumnName);
                    }
                }

                var result = await query
                    .Select(p => new GiveLeaveRequestDTO
                    {
                        Id = p.Id,
                        AbsenceReason = p.AbsenceReason.ToString(),
                        StartDate = p.StartDate,
                        EndDate = p.EndDate,
                        EmployeeId = dbContext.Employees
                            .Where(e => e.Id == p.EmployeeId)
                            .Select(e => e.FullName)
                            .FirstOrDefault(),
                        Comment = p.Comment,
                        Status = p.Status.ToString()
                    })
                    .ToListAsync(cancellationToken);

                _logger.Information("Handle method completed successfully with result count: {Count}", result.Count);

                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred in Handle method");
                return null;
            }
        }
    }
}