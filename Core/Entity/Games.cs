namespace Core.Entity;

public class Games : EntityBase
{
    public required string Name { get; set; }
    public required string Category { get; set; }
    public bool Active { get; set; }
    public bool Price { get; set; }
}