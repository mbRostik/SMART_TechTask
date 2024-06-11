using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using Duende.IdentityServer.Models;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace IdentityServer.WebApi
{
    public class SeedData
    {
        public static async Task EnsureSeedData(WebApplication app)
        {
            using (var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider.GetService<PersistedGrantDbContext>().Database.Migrate();

                var context = scope.ServiceProvider.GetService<ConfigurationDbContext>();
                context.Database.Migrate();
                EnsureSeedData(context);
                await EnsureRoles(scope);
            }
        }

        private static void EnsureSeedData(ConfigurationDbContext context)
        {
            if (context.Clients.Any())
            {
                context.Clients.RemoveRange(context.Clients);
                context.SaveChanges();
            }
            foreach (var client in Config.Clients.ToList())
            {
                context.Clients.Add(client.ToEntity());
            }
            context.SaveChanges();

            if (context.IdentityResources.Any())
            {
                context.IdentityResources.RemoveRange(context.IdentityResources);
                context.SaveChanges();
            }
            foreach (var resource in Config.IdentityResources.ToList())
            {
                context.IdentityResources.Add(resource.ToEntity());
            }
            context.SaveChanges();

            if (context.ApiScopes.Any())
            {
                context.ApiScopes.RemoveRange(context.ApiScopes);
                context.SaveChanges();
            }
            foreach (var resource in Config.ApiScopes.ToList())
            {
                context.ApiScopes.Add(resource.ToEntity());
            }
            context.SaveChanges();

            if (context.ApiResources.Any())
            {
                context.ApiResources.RemoveRange(context.ApiResources);
                context.SaveChanges();
            }
            foreach (var resource in Config.ApiResources.ToList())
            {
                context.ApiResources.Add(resource.ToEntity());
            }
            context.SaveChanges();

            if (context.IdentityProviders.Any())
            {
                context.IdentityProviders.RemoveRange(context.IdentityProviders);
                context.SaveChanges();
            }
            context.SaveChanges();
        }


        private static async Task EnsureRoles(IServiceScope scope)
        {
            string[] roleNames = { "Employee", "HRManager", "PMManager", "Administrator" };
            IdentityResult roleResult;
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }
    }
}
