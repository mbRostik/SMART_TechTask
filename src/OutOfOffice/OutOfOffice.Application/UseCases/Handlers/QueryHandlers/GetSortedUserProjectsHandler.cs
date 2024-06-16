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

namespace OutOfOffice.Application.UseCases.Handlers.QueryHandlers
{
    public class GetSortedUserProjectsHandler : IRequestHandler<GetSortedUserProjectsQuery, List<GiveProjectDTO>>
    {

        private readonly OutOfOfficeDbContext dbContext;
        public readonly Serilog.ILogger _logger;

        public GetSortedUserProjectsHandler(OutOfOfficeDbContext dbContext, Serilog.ILogger logger)
        {
            this.dbContext = dbContext;
            _logger = logger;
        }

        public async Task<List<GiveProjectDTO>> Handle(GetSortedUserProjectsQuery request, CancellationToken cancellationToken)
        {
            _logger.Information("Handle method started with request: {Request}", request);

            try
            {
                var projectsIds = await dbContext.EmployeeProjects
                    .Where(x => x.EmployeeId == request.userId)
                    .Select(x => x.ProjectId)
                    .ToListAsync(cancellationToken);

                _logger.Information("Fetched project IDs for user {UserId}: {ProjectIds}", request.userId, projectsIds);

                if (projectsIds == null || projectsIds.Count == 0)
                {
                    _logger.Warning("No projects found for user {UserId}", request.userId);
                    return new List<GiveProjectDTO>();
                }

                var query = dbContext.Projects.Where(p => projectsIds.Contains(p.Id));

                if (!string.IsNullOrEmpty(request.model.Status))
                {
                    if (Enum.TryParse<ProjectStatus>(request.model.Status, out var status))
                    {
                        query = query.Where(e => e.Status == status);
                        _logger.Information("Filtering by Status: {Status}", status);
                    }
                }

                if (!string.IsNullOrEmpty(request.model.ProjectType))
                {
                    if (Enum.TryParse<ProjectType>(request.model.ProjectType, out var type))
                    {
                        query = query.Where(e => e.ProjectType == type);
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