using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;

namespace Authentication.Api.Configurations;

public class Configuration
{
    public static IEnumerable<ApiScope> ApiScopes =>
        new List<ApiScope>
        {
            new("UserInfoScope", new []
            {
                JwtClaimTypes.Name,
                JwtClaimTypes.Role,
                JwtClaimTypes.ClientId
            }),
            new("ApiScope")
        };

    public static IEnumerable<IdentityResource> IdentityResources =>
        new List<IdentityResource>
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Email(),
            new IdentityResources.Phone(),
        };

    public static IEnumerable<ApiResource> ApiResources =>
        new List<ApiResource>()
        {
            new("Rent.Api")
            {
                ApiSecrets =
                {
                    new Secret("rent-secret".Sha256())
                },
                Scopes =
                {
                    "UserInfoScope",
                    "ApiScope"
                } 
            },
            new("CarMaintenance.Api")
            {
                ApiSecrets =
                {
                    new Secret("maintenance-secret".Sha256())
                },
                Scopes =
                {
                    "UserInfoScope",
                    "ApiScope"
                } 
            },
            new("Notification.Api")
            {
                ApiSecrets =
                {
                    new Secret("notification-secret".Sha256())
                },
                Scopes =
                {
                    "UserInfoScope",
                    "ApiScope"
                } 
            },
            new("Salon.Api")
            {
                ApiSecrets =
                {
                    new Secret("salon-secret".Sha256())
                },
                Scopes =
                {
                    "UserInfoScope",
                    "ApiScope"
                } 
            },
            new("Rating.Api")
            {
                ApiSecrets =
                {
                    new Secret("rating-secret".Sha256())
                },
                Scopes =
                {
                    "UserInfoScope",
                    "ApiScope"
                } 
            }
        };
    
    public static IEnumerable<Client> Clients => 
        new List<Client>
    {
        new()
        {
            ClientId = "client",
            AllowedGrantTypes = GrantTypes.Code,
            ClientSecrets =
            {
                new Secret("client-secret".Sha256()),     
            },
            AllowedScopes =
            {
                IdentityServerConstants.StandardScopes.OpenId, 
                IdentityServerConstants.StandardScopes.Phone,
                IdentityServerConstants.StandardScopes.Email,
                "UserInfoScope"

            },
            AllowOfflineAccess    = true
        },
        new ()
        {
            ClientId = "api",
            AllowedGrantTypes = GrantTypes.ClientCredentials,
            ClientSecrets =
            {
                new Secret("api-secret".Sha256()),     
            },
            AllowedScopes =
            {
                "ApiScope"
            },
            AllowOfflineAccess    = true
        }
    };
}