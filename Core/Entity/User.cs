namespace Core.Entity;

public class User : EntityBase
{
    public required string Name  { get; set; }
    public required string Email  { get; set; }
    public required string Password  { get; set; }
    public required string Permission  { get; set; }
    
    public ICollection<Game> Games { get; set; }
    
}