using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;

namespace Authentication.Api.Extensions;

public static class AuthenticationConfigExtensions
{
    public static void AddAuthConfiguration(this IServiceCollection services)
    { 
        services.AddAuthentication(options =>
        {
            options.DefaultChallengeScheme = "oidc";
        })
        .AddCookie("cookie", options =>
        {
            options.Events.OnSigningOut = async e =>
            {
                await e.HttpContext.RevokeUserRefreshTokenAsync();
            };
        })
        .AddOpenIdConnect("oidc", options =>
        {
            options.Authority = "https://localhost:7035";
            options.RequireHttpsMetadata = false;
            options.ClientId = "client";
            options.ClientSecret = "client-secret";
            options.ResponseType = "code";
            options.SaveTokens = true;

            options.Scope.Clear();
            options.Scope.Add("openid");
            options.Scope.Add("UserInfoScope");

            // keeps id_token smaller
            options.GetClaimsFromUserInfoEndpoint = true;
            options.SaveTokens = true;

            options.TokenValidationParameters = new TokenValidationParameters
            {
                NameClaimType = "name",
                RoleClaimType = "role",
                
            };
        });
        services.AddAccessTokenManagement(options =>
            {
                options.Client.DefaultClient.Scope = "user-profile";
            })
            .ConfigureBackchannelHttpClient();
    }
}