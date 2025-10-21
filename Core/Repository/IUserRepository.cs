using Core.Entity;

namespace Core.Repository;

public interface IUserRepository : IRepository<User>
{
    User GetUserGames(int id);
    User? GetUserByEmail(string email);
    User? GetUserByUsername(string username);
}