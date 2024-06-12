using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OutOfOffice.Application.Contracts.DTOs.ChangeDTOs;
using OutOfOffice.Application.Contracts.DTOs.GiveDTOs;
using OutOfOffice.Application.UseCases.Commands;
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
        public async Task<ActionResult<GiveUserProfileDTO>> GetUserProfile()
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            foreach (var c in HttpContext.User.Claims)
            {
                Console.WriteLine(c.Value +" "+c.Value);
            }
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

        [HttpPost("FinishRegistration")]
        public async Task<ActionResult> FinishRegistration([FromBody] FinishUserRegistrationDTO data)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            
            if (string.IsNullOrEmpty(userId))
            {
                return NotFound("User ID not found.");
            }

            var result = await mediator.Send(new FinishUserRegistrationCommand(data, userId));

            if (result == false)
            {
                return NotFound("There is no information.");
            }
            return Ok();
        }
    }
}
