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

    public User? GetUserByEmail(string email)
    {
        var user = _dbSet.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());

        return user;

    }

    public User? GetUserByUsername(string username)
    {
        var user = _dbSet.FirstOrDefault(u=>u.Name.ToLower() == username.ToLower());
        
        return user;
    }
}