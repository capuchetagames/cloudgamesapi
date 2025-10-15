using Core.Entity;
using Core.Repository;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository;

public class UserRepository : EfRepository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }
    public User GetUserGames(int id)
    {
        var user = _dbSet.Include(u=>u.Orders)
            .ThenInclude(g=>g.Game)
            .FirstOrDefault(u=>u.Id == id) ?? throw new Exception("User not found");
        
        
        
        user.Orders = user.Orders.Where(o=>o.UserId == id)
            .Select(o =>
            {
                o.User = null;
                o.Game.Orders = null;
                return o;
            }).ToList();
        
        return user;
    }
}