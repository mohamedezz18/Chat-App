using DatingApp.Core.Domain.Entities;
using DatingApp.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using static DatingApp.Core.Domain.RepositoryContracts.ILikesRepository;

namespace DatingApp.Core.ServiceContracts
{
    public interface ILikesService
    {
        Task<bool> AddToggleLikeAsync(Guid sourceMemberId, Guid targetMemberId);
        Task<bool> RemoveLikeAsync(MemberLike memberLike);
        Task<IReadOnlyList<Guid>> GetCurrentMemberLikeIdsAsync(Guid memberId);
        Task<MemberLike?> GetMemberLikeAsync(Guid sourceMemberId, Guid targetMemberId);
        Task<PaginatedResult<Member>> GetMemberLikesAsync(LikesParams likesParams);
    }
}
