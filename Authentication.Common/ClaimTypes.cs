using Microsoft.IdentityModel.JsonWebTokens;

namespace Authentication.Common;

public static class AuthClaimTypes
{
    public static string Role = "role";

    public static string Id = JwtRegisteredClaimNames.Sub;
}