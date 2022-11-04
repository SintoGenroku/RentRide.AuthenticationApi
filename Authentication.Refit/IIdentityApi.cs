using Refit;
using RentRide.AuthenticationApi.Models;
using RentRide.AuthenticationApi.Models.Requests;

namespace Authentication.Refit;

public interface IIdentityApi
{
    [Headers("Content-Type: application/x-www-form-urlencoded")]
    [Post("/connect/token")]
    Task<JwtToken> LoginAsync([Body(BodySerializationMethod.UrlEncoded)]LoginRequestModel user);
}