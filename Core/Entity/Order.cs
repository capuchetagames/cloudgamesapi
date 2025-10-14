namespace Core.Entity;

public class Order : EntityBase
{
    public int UserId { get; set; }
    public int GameId { get; set; }
    
    public User User { get; set; }
    
    public  Game Game { get; set; }
}