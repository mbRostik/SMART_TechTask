using MassTransit;
using MessageBus.Messages;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.WebApi.Consumers
{
    public class UserChangeConsumer : IConsumer<UserChangedEvent>
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<IdentityUser> userManager;

        public UserChangeConsumer(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }
        public async Task Consume(ConsumeContext<UserChangedEvent> context)
        {
            Console.WriteLine($"Successfully consumed UserChangedEvent");

            var userId = context.Message.UserId;
            var role = context.Message.Position;
            var name = context.Message.UserName;
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
            var roles = await userManager.GetRolesAsync(user);
            foreach (var userRole in roles)
            {
                var removeFromRoleResult = await userManager.RemoveFromRoleAsync(user, userRole);
                if (!removeFromRoleResult.Succeeded)
                {
                    Console.WriteLine($"Error occurred while removing user from role {userRole}: {string.Join(", ", removeFromRoleResult.Errors.Select(e => e.Description))}");
                    return;
                }
            }

            var addToRoleResult = await userManager.AddToRoleAsync(user, role);
            user.UserName = name;
            user.NormalizedUserName = userManager.NormalizeName(name);
            var updateResult = await userManager.UpdateAsync(user);
            if (!addToRoleResult.Succeeded)
            {
                Console.WriteLine($"Failed to add user {userId} to role {role}: {string.Join(", ", addToRoleResult.Errors.Select(e => e.Description))}");
                return;
            }

            Console.WriteLine($"User {userId} successfully added to role {role}");
        }
    }
}