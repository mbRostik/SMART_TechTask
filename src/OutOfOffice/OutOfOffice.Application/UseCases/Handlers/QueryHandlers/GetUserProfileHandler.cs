using MediatR;
using Microsoft.EntityFrameworkCore;
using OutOfOffice.Application.Contracts.DTOs.GiveDTOs;
using OutOfOffice.Application.UseCases.Queries;
using OutOfOffice.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutOfOffice.Application.UseCases.Handlers.QueryHandlers
{
    public class GetUserProfileHandler : IRequestHandler<GetUserProfileQuery, GiveUserProfileDTO>
    {

        private readonly OutOfOfficeDbContext dbContext;

        public GetUserProfileHandler(OutOfOfficeDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<GiveUserProfileDTO> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var checkRegModel = await dbContext.UnRegisteredUsers.FirstOrDefaultAsync(x=>x.Id==request.userId);
                if (checkRegModel != null)
                {
                    GiveUserProfileDTO unRegResult = new GiveUserProfileDTO
                    {
                        Id = checkRegModel.Id,
                        FullName = checkRegModel.UserName
                    };
                    return unRegResult;
                }
                string partnerName = "";
                var userModel = await dbContext.Employees.FirstOrDefaultAsync(x=>x.Id==request.userId);
                if (userModel != null)
                {
                    var partner = await dbContext.Employees.FirstOrDefaultAsync(x => x.Id == userModel.PeoplePartnerID);
                    if (partner != null)
                    {
                        partnerName=partner.FullName;
                    }
                }

                GiveUserProfileDTO result = new GiveUserProfileDTO
                {
                    Id = userModel.Id,
                    FullName = userModel.FullName,
                    PeoplePartnerID = partnerName,
                    OutOfOfficeBalance = userModel.OutOfOfficeBalance,
                    Photo = userModel.Photo,
                    Position = userModel.Position.ToString(),
                    Status = userModel.Status.ToString(),
                    Subdivision = userModel.Subdivision.ToString(),
                    FullyRegistered = true
                };

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while fetching userprofile for UserId: {request.userId} - {ex}");
                return null;
            }
        }
    }
}