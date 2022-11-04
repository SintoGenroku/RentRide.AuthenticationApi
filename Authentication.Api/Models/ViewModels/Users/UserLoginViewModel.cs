using System.ComponentModel.DataAnnotations;

namespace Authentication.Api.Models.ViewModels.Users;

public class UserLoginViewModel
{
    public string? Login { get; set; }
    
    public string? Password { get; set; }
    
    public string? ReturnUrl { get; set; }
}