namespace Authentication.Common;

public class User 
{
    public Guid Id { get; set; }

    public string? Username { get; set; }
    
    public string? PasswordHash { get; set; }
    
    public ICollection<Role>? Roles { get; set; }
    
    public bool IsActive { get; set; }
    
    public bool IsDeleted { get; set; }

}