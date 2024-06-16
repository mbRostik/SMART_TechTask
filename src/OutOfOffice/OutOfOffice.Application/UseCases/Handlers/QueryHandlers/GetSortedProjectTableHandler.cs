using MediatR;
using Microsoft.EntityFrameworkCore;
using OutOfOffice.Application.Contracts.DTOs.GiveDTOs;
using OutOfOffice.Application.UseCases.Queries;
using OutOfOffice.Domain.Employees.Enums;
using OutOfOffice.Domain.Employees;
using OutOfOffice.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OutOfOffice.Domain.Projects.Enums;
using OutOfOffice.Domain.Projects;

namespace OutOfOffice.Application.UseCases.Handlers.QueryHandlers
{
    public class GetSortedProjectTableHandler : IRequestHandler<GetSortedProjectTableQuery, List<GiveProjectDTO>>
    {

        private readonly OutOfOfficeDbContext dbContext;
        public readonly Serilog.ILogger _logger;

        public GetSortedProjectTableHandler(OutOfOfficeDbContext dbContext, Serilog.ILogger logger)
        {
            this.dbContext = dbContext;
            _logger = logger;
        }

        public async Task<List<GiveProjectDTO>> Handle(GetSortedProjectTableQuery request, CancellationToken cancellationToken)
        {
            _logger.Information("Handle method started with request: {Request}", request);

            try
            {
                var query = dbContext.Projects.AsQueryable();

                if (!string.IsNullOrEmpty(request.model.Status))
                {
                    if (Enum.TryParse<ProjectStatus>(request.model.Status, out var status))
                    {
                        query = query.Where(p => p.Status == status);
                        _logger.Information("Filtering by Status: {Status}", status);
                    }
                }

                if (!string.IsNullOrEmpty(request.model.ProjectType))
                {
                    if (Enum.TryParse<ProjectType>(request.model.ProjectType, out var type))
                    {
                        query = query.Where(p => p.ProjectType == type);
                        _logger.Information("Filtering by ProjectType: {ProjectType}", type);
                    }
                }

                if (!string.IsNullOrEmpty(request.model.ColumnName))
                {
                    var propertyInfo = typeof(Project).GetProperty(request.model.ColumnName);
                    if (propertyInfo == null)
                    {
                        var errorMessage = $"Column '{request.model.ColumnName}' does not exist in the Project model";
                        _logger.Error(errorMessage);
                        throw new ArgumentException(errorMessage);
                    }

                    if (request.model.Descending)
                    {
                        query = query.OrderByDescending(p => EF.Property<object>(p, request.model.ColumnName));
                        _logger.Information("Sorting by {ColumnName} in descending order", request.model.ColumnName);
                    }
                    else
                    {
                        query = query.OrderBy(p => EF.Property<object>(p, request.model.ColumnName));
                        _logger.Information("Sorting by {ColumnName} in ascending order", request.model.ColumnName);
                    }
                }

                var result = await query
                    .Select(p => new GiveProjectDTO
                    {
                        Id = p.Id,
                        ProjectType = p.ProjectType.ToString(),
                        StartDate = p.StartDate,
                        EndDate = p.EndDate,
                        ProjectManagerId = dbContext.Employees
                            .Where(e => e.Id == p.ProjectManagerId)
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