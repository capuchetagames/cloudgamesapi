namespace Core.Entity;

public class Game : EntityBase
{
    public required string Name { get; set; }
    public required string Category { get; set; }
    public bool Active { get; set; }
    public decimal Price { get; set; }
    
    public virtual ICollection<Order> Orders { get; set; }
}