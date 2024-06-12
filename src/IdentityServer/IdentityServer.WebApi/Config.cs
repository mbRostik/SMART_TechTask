using Duende.IdentityServer.Models;

namespace IdentityServer.WebApi
{
    public class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
           new IdentityResource[]
           {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResource("roles", new [] { "Employee", "HRManager", "PMManager", "Administrator" })
           };

        public static IEnumerable<ApiScope> ApiScopes =>
           new ApiScope[]
           {
               new ApiScope("OutOfOffice.WebApi.Scope")

           };

        public static IEnumerable<ApiResource> ApiResources => new[] {
            new ApiResource("OutOfOffice.WebApi")
             {
                 Scopes=new List<string>{ "OutOfOffice.WebApi.Scope"},
                 ApiSecrets=new List<Secret>{new Secret("OutOfOffice.WebApi.Secret".Sha256())},
                 UserClaims = { "Employee", "HRManager", "PMManager", "Administrator" }
             }
        };

        public static IEnumerable<Client> Clients =>
           new Client[]
           {
                new Client
                {
                    ClientId = "interactive",
                    ClientSecrets = {new Secret("OnlyUserKnowsThisSecret".Sha256())},
                    AllowedGrantTypes = GrantTypes.Code,
                    RedirectUris = { "https://localhost:5173/signin-oidc" },
                    FrontChannelLogoutUri="https://localhost:5173/signout-oidc",
                    PostLogoutRedirectUris={ "https://localhost:5173/signout-callback-oidc" },
                    AllowOfflineAccess = true,
                    AllowedScopes = 
                    {"openid", "profile", "OutOfOffice.WebApi.Scope"},
                    RequireConsent = true,
                    RequirePkce=true,
                    AllowPlainTextPkce=true,
                    AllowedCorsOrigins = { "https://localhost:5173" }
                }
           };
    }
}