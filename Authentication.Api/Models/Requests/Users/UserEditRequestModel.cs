namespace Authentication.Api.Models.Requests.Users;

public class UserEditRequestModel
{
    public Guid Id { get; set; }
    
    public string Password { get; set; }
    
    public string? PhoneNumber { get; set; }
    
    public string? MailAddress { get; set; }
}