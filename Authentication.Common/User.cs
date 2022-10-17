namespace Authentication.Common;

public class User 
{
    public Guid Id { get; set; }

    public string? Username { get; set; }
    
    public string? Fullname { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public string? PasswordHash { get; set; }
    
    public ICollection<Role>? Roles { get; set; }
    
    public string? PhoneNumber { get; set; }
    
    public string? MailAddress { get; set; }
}