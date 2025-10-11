namespace Core.Entity;

public class Users : EntityBase
{
    public required string Name  { get; set; }
    public required string Email  { get; set; }
    public required string Password  { get; set; }
    public required string AccessType  { get; set; }
    
}