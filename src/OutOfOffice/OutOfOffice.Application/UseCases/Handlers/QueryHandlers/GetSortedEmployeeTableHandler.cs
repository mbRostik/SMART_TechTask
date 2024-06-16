using MediatR;
using Microsoft.EntityFrameworkCore;
using OutOfOffice.Application.Contracts.DTOs.GiveDTOs;
using OutOfOffice.Application.UseCases.Queries;
using OutOfOffice.Domain.Employees;
using OutOfOffice.Domain.Employees.Enums;
using OutOfOffice.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutOfOffice.Application.UseCases.Handlers.QueryHandlers
{
    public class GetSortedEmployeeTableHandler : IRequestHandler<GetSortedEmployeeTableQuery, List<GiveUserProfileDTO>>
    {

        private readonly OutOfOfficeDbContext dbContext;
        public readonly Serilog.ILogger _logger;

        public GetSortedEmployeeTableHandler(OutOfOfficeDbContext dbContext, Serilog.ILogger logger)
        {
            this.dbContext = dbContext;
            this._logger = logger;
        }

        public async Task<List<GiveUserProfileDTO>> Handle(GetSortedEmployeeTableQuery request, CancellationToken cancellationToken)
        {
            _logger.Information("Handle method started with request: {Request}", request);

            try
            {
                var query = dbContext.Employees.AsQueryable();

                if (!string.IsNullOrEmpty(request.model.Status))
                {
                    if (Enum.TryParse<EmployeeStatus>(request.model.Status, out var status))
                    {
                        query = query.Where(e => e.Status == status);
                        _logger.Information("Filtering by Status: {Status}", status);
                    }
                }

                if (!string.IsNullOrEmpty(request.model.Position))
                {
                    if (Enum.TryParse<Position>(request.model.Position, out var position))
                    {
                        query = query.Where(e => e.Position == position);
                        _logger.Information("Filtering by Position: {Position}", position);
                    }
                }

                if (!string.IsNullOrEmpty(request.model.Subdivision))
                {
                    if (Enum.TryParse<Subdivision>(request.model.Subdivision, out var subdivision))
                    {
                        query = query.Where(e => e.Subdivision == subdivision);
                        _logger.Information("Filtering by Subdivision: {Subdivision}", subdivision);
                    }
                }

                if (!string.IsNullOrEmpty(request.model.ColumnName))
                {
                    var propertyInfo = typeof(Employee).GetProperty(request.model.ColumnName);
                    if (propertyInfo == null)
                    {
                        var errorMessage = $"Column '{request.model.ColumnName}' does not exist in the Employee model";
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
                    .Select(e => new GiveUserProfileDTO
                    {
                        Id = e.Id,
                        FullName = e.FullName,
                        Position = e.Position.ToString(),
                        Status = e.Status.ToString(),
                        Subdivision = e.Subdivision.ToString(),
                        PeoplePartnerID = e.PeoplePartnerID,
                        OutOfOfficeBalance = e.OutOfOfficeBalance,
                        Photo = e.Photo,
                        FullyRegistered = true
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