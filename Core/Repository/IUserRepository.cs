using Core.Entity;

namespace Core.Repository;

public interface IUserRepository : IRepository<User>
{
    User GetUserGames(int id);
}