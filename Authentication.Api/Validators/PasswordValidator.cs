using System.Text.RegularExpressions;
using Authentication.Common;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;

namespace Authentication.Api.Validators;

public class PasswordValidator : IPasswordValidator<User>
{
    public Task<IdentityResult> ValidateAsync(UserManager<User> manager, User user, string password)
    {
        var regexes = new Dictionary<Regex, string>()
        {
            {new Regex(@"\d"), "Add digits(minimum 1)"},
            {new Regex(@"[A-Z]"), "Add uppercase"},
            {new Regex(@"[a-z]"), "Add lowercase"},
            { new Regex(@"^[a-zA-Z0-9!@#$%^&*()_+|]+$"), "Such characters are not allowed." },
        };
        
        foreach (var regex in regexes)
        {
            if (!regex.Key.IsMatch(password))
            {
                var error = new IdentityError()
                {
                    Description = regex.Value
                };

                var result = IdentityResult.Failed(error);

                return Task.FromResult(result);
            }
        }

        return Task.FromResult(IdentityResult.Success);
    }
}