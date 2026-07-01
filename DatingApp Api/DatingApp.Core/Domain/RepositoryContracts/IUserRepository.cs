using DatingApp.Core.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatingApp.Core.Domain.RepositoryContracts
{
    public interface IUserRepository
    {
        Task<List<UserWithRoleDto>> GetUsersWithRolesAsync();

    }
}
