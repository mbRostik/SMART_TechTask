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
        public readonly Serilog.ILogger _logger;

        public GetUserProfileHandler(OutOfOfficeDbContext dbContext, Serilog.ILogger logger)
        {
            this.dbContext = dbContext;
            _logger = logger;
        }

        public async Task<GiveUserProfileDTO> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
        {
            _logger.Information("Handle method started with request: {Request}", request);

            try
            {
                var checkRegModel = await dbContext.UnRegisteredUsers.FirstOrDefaultAsync(x => x.Id == request.userId, cancellationToken);
                _logger.Information("Checked if user is unregistered for UserId: {UserId}", request.userId);

                if (checkRegModel != null)
                {
                    var unRegResult = new GiveUserProfileDTO
                    {
                        Id = checkRegModel.Id,
                        FullName = checkRegModel.UserName
                    };
                    _logger.Information("User is unregistered. Returning unregistered user profile for UserId: {UserId}", request.userId);
                    return unRegResult;
                }

                string partnerName = "";
                var userModel = await dbContext.Employees.FirstOrDefaultAsync(x => x.Id == request.userId, cancellationToken);
                _logger.Information("Fetched employee model for UserId: {UserId}", request.userId);

                if (userModel != null)
                {
                    var partner = await dbContext.Employees.FirstOrDefaultAsync(x => x.Id == userModel.PeoplePartnerID, cancellationToken);
                    if (partner != null)
                    {
                        partnerName = partner.FullName;
                        _logger.Information("Fetched partner name for PeoplePartnerID: {PeoplePartnerID}", userModel.PeoplePartnerID);
                    }
                }

                var result = new GiveUserProfileDTO
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

                _logger.Information("Handle method completed successfully for UserId: {UserId}", request.userId);

                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while fetching user profile for UserId: {UserId}", request.userId);
                return null;
            }
        }
    }
}