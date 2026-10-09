namespace Entities;

public class User
{
    
    public long Id {get; set;} 
    public string Username {get; set;} = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash {get; set;} = string.Empty;
    public DateTime CreatedAt {get; set;} = DateTime.UtcNow;
    public int MMR {get; set;} = 100;
    public string Nickname {get; set;} = string.Empty;

}