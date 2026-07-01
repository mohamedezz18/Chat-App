using DatingApp.Core.Domain.Entities;
using DatingApp.Core.Domain.RepositoryContracts;
using DatingApp.Core.Helpers;
using DatingApp.Core.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatingApp.Core.Services
{
    public class LikesService : ILikesService
    {
        private readonly ILikesRepository _likesRepository;

        public LikesService(ILikesRepository likesRepository)
        {
            _likesRepository = likesRepository;
        }

        public async Task<bool> AddToggleLikeAsync(Guid sourceMemberId, Guid targetMemberId)
        {

            var existingLike = await _likesRepository.GetMemberLike(sourceMemberId, targetMemberId);
            if (existingLike == null)
            {
                var like = new MemberLike
                {
                    SourceMemberID = sourceMemberId,
                    TargetMemberID = targetMemberId
                };

                _likesRepository.AddLike(like);
            }

            else
            {
                _likesRepository.DeleteLike(existingLike);
            }
            return await _likesRepository.SaveAllChangesAsync();
        }

        public async Task<IReadOnlyList<Guid>> GetCurrentMemberLikeIdsAsync(Guid memberId)
        {
            return await _likesRepository.GetCurrentMemberLikeIds(memberId);
        }

        public async Task<PaginatedResult<Member>> GetMemberLikesAsync(LikesParams likesParams)
        {
            return await _likesRepository.GetMemberLikes(likesParams);
        }

        public async Task<MemberLike?> GetMemberLikeAsync(Guid sourceMemberId, Guid targetMemberId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> RemoveLikeAsync(MemberLike memberLike)
        {
            throw new NotImplementedException();
        }
    }
}
