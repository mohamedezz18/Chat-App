using DatingApp.Core.Domain.RepositoryContracts;
using DatingApp.Core.DTO;
using DatingApp.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserWithRoleDto>> GetUsersWithRolesAsync()
        {
            return await (
                 from us in _context.Users.AsNoTracking()

                 join ur in _context.UserRoles.AsNoTracking()
                     on us.Id equals ur.UserId into urGroup

                 from ur in urGroup.DefaultIfEmpty()

                 join r in _context.Roles.AsNoTracking()
                     on ur.RoleId equals r.Id into rGroup

                 from r in rGroup.DefaultIfEmpty()

                 group r by new
                 {
                     us.Id,
                     us.UserName,
                     us.Email
                 }
                 into g

                 select new UserWithRoleDto
                 {
                     Id = g.Key.Id,
                     Email = g.Key.Email!,
                     Roles = g
                         .Where(x => x != null)
                         .Select(x => x.Name!)
                         .ToList()
                 }

                ).ToListAsync();
        }
    }
}
