using Authentication.Services.Abstracts;
using IdentityServer4.Models;
using IdentityServer4.Validation;

namespace Authentication.Api.Validators;

public class CustomResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IUserService _userService;

    public CustomResourceOwnerPasswordValidator( IUserService userService, IAuthenticationService authenticationService)
    {
        _userService = userService;
        _authenticationService = authenticationService;
    }
    public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
    {
        var user = await _userService.GetByNameAsync(context.UserName);
        if (user != null)
        {
            
            var result = await _authenticationService.SignInAsync(user, context.Password);
            // https://docs.identityserver.io/en/latest/reference/grant_validation_result.html#refgrantvalidationresult
            if (result.IsSuccessfull)
            {
                context.Result = new GrantValidationResult(subject: user.Id.ToString(), authenticationMethod: "password");
            }
            else
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "invalid password");
            }
        }
        else
        {
            context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "invalid login");
        }
    }
}