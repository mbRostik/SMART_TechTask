using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OutOfOffice.Application.Contracts.DTOs.ChangeDTOs;
using OutOfOffice.Application.Contracts.DTOs.GetDTOs;
using OutOfOffice.Application.Contracts.DTOs.GiveDTOs;
using OutOfOffice.Application.UseCases.Commands;
using OutOfOffice.Application.UseCases.Queries;
using System.Data;
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



        [HttpPost("UploadProfilePhoto")]
        public async Task<ActionResult> UploadProfilePhoto([FromBody] ChangeProfilePhotoDTO data)
        {
            Console.WriteLine("keghbkwhbglwgrwrlkg");
            var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine("UploadProfilePhoto called but user ID is missing.");
                return Unauthorized("User ID is required.");
            }

            try
            {
                Console.WriteLine("Attempting to upload profile photo for user");

                data.Id = userId;
                await mediator.Send(new ChangeUserAvatarCommand(data));

                Console.WriteLine("Profile photo updated successfully for user Fetching updated user profile.");

                var result = await mediator.Send(new GetUserProfileQuery(data.Id));

                if (result == null)
                {
                    Console.WriteLine("Failed to fetch updated profile for user after uploading photo.");
                    return NotFound("User profile not found.");
                }

                Console.WriteLine("Successfully retrieved updated profile for user after photo upload.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while uploading profile photo for user." + ex);
                return BadRequest("Something went wrong.");
            }
        }


        [HttpGet("GetUserProfile")]
        public async Task<ActionResult<GiveUserProfileDTO>> GetUserProfile()
        {

            var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
          

            var result = await mediator.Send(new GetUserProfileQuery(userId));

            if (result == null)
            {
                return NotFound("There is no information.");
            }
            return Ok(result);
        }

        [HttpPost("UpdateProfile")]
        public async Task<ActionResult> UpdateProfile([FromBody] ChangeProfileDTO data)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;


            if (string.IsNullOrEmpty(userId))
            {
                return NotFound("User ID not found.");
            }

            data.Id= userId;

            var result = await mediator.Send(new ChangeProfileCommand(data));

            if (result)
            {
                return Ok();
            }
            return BadRequest();

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
                return NotFound("Smth went wrong.");
            }
            return Ok();
        }


        [Authorize(Policy = "RequireHROrPMManagerRole")]
        [HttpPost("GetSortedEmployeeTable")]
        public async Task<ActionResult<List<GiveUserProfileDTO>>> GetSortedEmployeeTable([FromBody] GetSortedEmployeesDTO data)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;


            if (string.IsNullOrEmpty(userId))
            {
                return NotFound("User ID not found.");
            }

            var result = await mediator.Send(new GetSortedEmployeeTableQuery(data));

            return Ok(result);
        }

        [Authorize(Policy = "RequireHRManagerRole")]
        [HttpPost("ChangeUserInformation")]
        public async Task<ActionResult> ChangeUserInformation([FromBody] GiveUserProfileDTO data)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;


            if (string.IsNullOrEmpty(userId))
            {
                return NotFound("User ID not found.");
            }

            var result = await mediator.Send(new ChangeEmployeeInformationCommand(data));

            return Ok(result);
        }

        [Authorize(Policy = "RequireHRManagerRole")]
        [HttpPost("CreateEmployee")]
        public async Task<ActionResult> CreateEmployee([FromBody] AddEmployeeDTO data)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;


            if (string.IsNullOrEmpty(userId))
            {
                return NotFound("User ID not found.");
            }

            var result = await mediator.Send(new CreateEmployeeCommand(data));

            return Ok(result);
        }


        [Authorize(Policy = "RequireHROrPMManagerRole")]
        [HttpPost("GetSortedProjectTable")]
        public async Task<ActionResult<List<GiveProjectDTO>>> GetSortedProjectTable([FromBody] GetSortedProjectsDTO data)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;


            if (string.IsNullOrEmpty(userId))
            {
                return NotFound("User ID not found.");
            }

            var result = await mediator.Send(new GetSortedProjectTableQuery(data));

            return Ok(result);
        }

        [Authorize(Policy = "RequirePMManagerRole")]
        [HttpPost("ChangeProject")]
        public async Task<ActionResult<List<GiveProjectDTO>>> ChangeProject([FromBody] ChangeProjectDTO data)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;


            if (string.IsNullOrEmpty(userId))
            {
                return NotFound("User ID not found.");
            }

            var result = await mediator.Send(new ChangeProjectCommand(data));

            return Ok(result);
        }


        [Authorize(Policy = "RequirePMManagerRole")]
        [HttpPost("CreateProject")]
        public async Task<ActionResult<List<GiveProjectDTO>>> CreateProject([FromBody] AddProjectDTO data)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;


            if (string.IsNullOrEmpty(userId))
            {
                return NotFound("User ID not found.");
            }

            var result = await mediator.Send(new CreateProjectCommand(data));
            if (result)
            {
                return Ok();

            }

            return BadRequest();
        }
    }
}
