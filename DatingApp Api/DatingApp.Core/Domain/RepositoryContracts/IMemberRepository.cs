using DatingApp.Core.Domain.Entities;
using DatingApp.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatingApp.Core.Domain.RepositoryContracts
{
    public interface IMemberRepository
    {
        void Update(Member member);
        Task<PaginatedResult<Member>> GetMembersAsync(MemberParams memberParams);
        Task<Member?> GetMemberByIdAsync(Guid id);
        public Task<Member?> GetMemberByIdForUpdateAsync(Guid id);
        Task<IReadOnlyList<Photo>> GetPhotosForMemberAsync(Guid memberId);

        Task<bool> SaveAllChangesAsync();
    }
}
