namespace Core.Entity;

public class User : EntityBase
{
    public required string Name  { get; set; }
    public required string Email  { get; set; }
    public required string Password  { get; set; }
    public required PermissionType Permission  { get; set; }
    
    public virtual ICollection<Order> Orders { get; set; }
    
}