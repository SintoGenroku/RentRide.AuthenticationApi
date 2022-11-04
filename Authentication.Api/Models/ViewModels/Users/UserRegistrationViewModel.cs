using System.ComponentModel.DataAnnotations;

namespace Authentication.Api.Models.ViewModels.Users;

public class UserRegistrationViewModel
{
    public string Login { get; set; }

    public string Fullname { get; set; }
    
    public string Password { get; set; }
    
    public string ConfirmPassword { get; set; }
    
    public string MailAddress { get; set; }
    
    public string PhoneNumber { get; set; }
}