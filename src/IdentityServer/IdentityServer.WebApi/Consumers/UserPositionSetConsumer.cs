using MassTransit.Mediator;
using MassTransit;
using MessageBus.Messages;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MassTransit.Initializers;

namespace IdentityServer.WebApi.Consumers
{
    public class UserPositionSetConsumer : IConsumer<UserPositionSetEvent>
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<IdentityUser> userManager;

        public UserPositionSetConsumer(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }
        public async Task Consume(ConsumeContext<UserPositionSetEvent> context)
        {
            Console.WriteLine($"Successfully consumed UserPositionSetEvent");

            var userId = context.Message.UserId;
            var role = context.Message.Position;

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                Console.WriteLine($"User with ID {userId} not found.");
                return;
            }

            var roleExists = await roleManager.RoleExistsAsync(role);
            if (!roleExists)
            {
                var existingRole = await roleManager.Roles.FirstOrDefaultAsync();
                if (existingRole != null)
                {
                    role = existingRole.Name;
                }
            }

            var addToRoleResult = await userManager.AddToRoleAsync(user, role);
            if (!addToRoleResult.Succeeded)
            {
                Console.WriteLine($"Failed to add user {userId} to role {role}: {string.Join(", ", addToRoleResult.Errors.Select(e => e.Description))}");
                return;
            }

            Console.WriteLine($"User {userId} successfully added to role {role}");
        }
    }
}