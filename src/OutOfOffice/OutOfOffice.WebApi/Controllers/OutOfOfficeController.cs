using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OutOfOffice.Application.Contracts.DTOs.GiveDTOs;
using OutOfOffice.Application.UseCases.Queries;
using System.Security.Claims;

namespace OutOfOffice.WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class OutOfOfficeController : ControllerBase
    {
        private readonly IMediator mediator;
        public OutOfOfficeController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet("GetUserProfile")]
        [AllowAnonymous]
        public async Task<ActionResult<GiveUserProfileDTO>> GetUserProfile()
        {

            var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return NotFound("User ID not found.");
            }

            var result = await mediator.Send(new GetUserProfileQuery(userId));

            if (result == null)
            {
                return NotFound("There is no information.");
            }
            return Ok(result);
        }
    }
}
