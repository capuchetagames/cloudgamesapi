using Core.Entity;
using Core.Repository;

namespace Infrastructure.Repository;

public class UserRepository : EfRepository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }
}