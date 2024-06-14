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

        public GetSortedEmployeeTableHandler(OutOfOfficeDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<GiveUserProfileDTO>> Handle(GetSortedEmployeeTableQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var query = dbContext.Employees.AsQueryable();

                if (!string.IsNullOrEmpty(request.model.Status))
                {
                    if (Enum.TryParse<EmployeeStatus>(request.model.Status, out var status))
                    {
                        query = query.Where(e => e.Status == status);
                    }
                }

                if (!string.IsNullOrEmpty(request.model.Position))
                {
                    if (Enum.TryParse<Position>(request.model.Position, out var position))
                    {
                        query = query.Where(e => e.Position == position);
                    }
                }

                if (!string.IsNullOrEmpty(request.model.Subdivision))
                {
                    if (Enum.TryParse<Subdivision>(request.model.Subdivision, out var subdivision))
                    {
                        query = query.Where(e => e.Subdivision == subdivision);
                    }
                }

                if (!string.IsNullOrEmpty(request.model.ColumnName))
                {
                    var propertyInfo = typeof(Employee).GetProperty(request.model.ColumnName);
                    if (propertyInfo == null)
                    {
                        throw new ArgumentException($"Column '{request.model.ColumnName}' does not exist in the Employee model :(((");
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