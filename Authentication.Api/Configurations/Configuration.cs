﻿using IdentityModel;
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
            new(
                name: "user-profile",
                userClaims: new[] { "name", "role","email" },
                displayName: "Your profile data")
        };
            
        public static IEnumerable<ApiResource> ApiResources =>
        new List<ApiResource>()
        {
            new("Client")
            {
                ApiSecrets =
                {
                    new Secret("client-secret".Sha256())
                },
                Scopes =
                {
                    "UserInfoScope"
                } 
            },
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
            new("Cars.Api")
            {
                ApiSecrets =
                {
                    new Secret("cars-secret".Sha256())
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
            new("Users.Api")
            {
                ApiSecrets =
                {
                    new Secret("users-secret".Sha256())
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
                IdentityServerConstants.StandardScopes.OfflineAccess,
                "UserInfoScope", "user-profile", "Client" 

            },
            AllowOfflineAccess = true,
            RequirePkce = true,
            AllowAccessTokensViaBrowser = true,
            RedirectUris = new List<string>{ "https://localhost:7035/signin-oidc" },
            PostLogoutRedirectUris = {"https://localhost:7035/signout-callback-oidc"},
            RequireConsent = false,
            AccessTokenLifetime = 180,
            UpdateAccessTokenClaimsOnRefresh = true,
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
            AllowOfflineAccess = true
        }
    };
}