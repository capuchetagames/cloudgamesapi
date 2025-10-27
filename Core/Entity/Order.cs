namespace Core.Entity;

public class Order : EntityBase
{
    public int UserId { get; set; }
    public int GameId { get; set; }
    
    public virtual User User { get; set; }
    
    public virtual Game Game { get; set; }
}