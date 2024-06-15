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

        public GetSortedUserProjectsHandler(OutOfOfficeDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<GiveProjectDTO>> Handle(GetSortedUserProjectsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var projectsIds = await dbContext.EmployeeProjects.Where(x=>x.EmployeeId==request.userId).Select(x=>x.ProjectId).ToListAsync();
                if (projectsIds == null)
                {
                    return null;
                }
                var query = dbContext.Projects.Where(p => projectsIds.Contains(p.Id));

                if (!string.IsNullOrEmpty(request.model.Status))
                {
                    if (Enum.TryParse<ProjectStatus>(request.model.Status, out var status))
                    {
                        query = query.Where(e => e.Status == status);
                    }
                }

                if (!string.IsNullOrEmpty(request.model.ProjectType))
                {
                    if (Enum.TryParse<ProjectType>(request.model.ProjectType, out var type))
                    {
                        query = query.Where(e => e.ProjectType == type);
                    }
                }



                if (!string.IsNullOrEmpty(request.model.ColumnName))
                {
                    var propertyInfo = typeof(Project).GetProperty(request.model.ColumnName);
                    if (propertyInfo == null)
                    {
                        throw new ArgumentException($"Column '{request.model.ColumnName}' does not exist in the Project model :(((");
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
                    .ToListAsync();

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Pe4al kolu popalu v GetSortedEmployeeTableHandler: {ex.Message}");
                return null;
            }
        }

    }
}