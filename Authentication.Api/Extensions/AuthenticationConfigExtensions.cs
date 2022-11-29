using Microsoft.AspNetCore.Authentication;

namespace Authentication.Api.Extensions;

public static class AuthenticationConfigExtensions
{
    public static void AddAuthConfiguration(this IServiceCollection services)
    { 
        services.AddAuthentication(options =>
        {
            options.DefaultChallengeScheme = "oidc";
        })
        .AddCookie("Identity.Application", options =>
        {
            options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
            options.SlidingExpiration = true;
            options.AccessDeniedPath = "/Forbidden/";
            
            options.Events.OnSigningOut = async e =>
            {
                // revoke refresh token on sign-out
                await e.HttpContext.RevokeUserRefreshTokenAsync();
            };
        })
        .AddOpenIdConnect("oidc", options =>
        {
            options.SignInScheme = "Cookies";
            options.Authority = "https://localhost:7035";
            options.RequireHttpsMetadata = false;
            
            options.ClientId = "client";
            options.ClientSecret = "client-secret";
            options.SaveTokens = true;
            // code flow + PKCE (PKCE is turned on by default)
            options.ResponseType = "code id_token";

            options.Scope.Clear();
            options.Scope.Add("openid");
            options.Scope.Add("offline_access");
            options.Scope.Add("api");
            options.Scope.Add("UserInfoScope");
            options.Scope.Add("user-profile");

            // keeps id_token smaller
            options.GetClaimsFromUserInfoEndpoint = true;
            options.SaveTokens = true;
        });

        // adds user and client access token management
        services.AddAccessTokenManagement(options =>
            {
                // client config is inferred from OpenID Connect settings
                // if you want to specify scopes explicitly, do it here, otherwise the scope parameter will not be sent
                options.Client.DefaultClient.Scope = "user-profile";
            })
            .ConfigureBackchannelHttpClient();
    }
}