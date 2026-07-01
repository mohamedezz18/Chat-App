using DatingApp.Core.Domain.Entities;
using DatingApp.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatingApp.Core.Domain.RepositoryContracts
{
    public interface ILikesRepository
    {
        enum Predicate { liked , likedBy, mutual}
        Task<MemberLike?> GetMemberLike(Guid sourceMemberId, Guid targetMemberId);
        Task<PaginatedResult<Member>> GetMemberLikes(LikesParams likesParams);
        Task<IReadOnlyList<Guid>> GetCurrentMemberLikeIds(Guid memberId);
        void DeleteLike(MemberLike like);
        void AddLike(MemberLike like);
        Task<bool> SaveAllChangesAsync();

    }
}
