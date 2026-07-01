using DatingApp.Core.Domain.Entities;
using DatingApp.Core.Domain.RepositoryContracts;
using DatingApp.Core.Helpers;
using DatingApp.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using static DatingApp.Core.Domain.RepositoryContracts.ILikesRepository;

namespace DatingApp.Infrastructure.Repositories
{
    public class LikesRepository : ILikesRepository
    {
        private readonly ApplicationDbContext _context;

        public LikesRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void AddLike(MemberLike like)
        {
            _context.Likes.Add(like);
        }

        public void DeleteLike(MemberLike like)
        {
            _context.Likes.Remove(like);
        }

        public async Task<IReadOnlyList<Guid>> GetCurrentMemberLikeIds(Guid memberId)
        {
           return await  _context.Likes
                .Where(x => x.SourceMemberID == memberId)
                .Select(id => id.TargetMemberID).ToListAsync();
        }

        public async Task<MemberLike?> GetMemberLike(Guid sourceMemberId, Guid targetMemberId)
        {
            return await _context.Likes.FindAsync(sourceMemberId, targetMemberId);
        }

        public async Task<PaginatedResult<Member>> GetMemberLikes(LikesParams likesParams)
        {
            var query = _context.Likes.AsQueryable();
            IQueryable<Member> result;

            switch (likesParams.Predicate)
            {
                case Predicate.liked:
                    result = query
                        .Where(like => like.SourceMemberID == likesParams.MemberId)
                        .Select(like => like.TargetMember);
                    break;
                case Predicate.likedBy:
                    result = query
                        .Where(like => like.TargetMemberID == likesParams.MemberId)
                        .Select(like => like.SourceMember);
                    break;
                default: // mutual
                    var likeIds = await GetCurrentMemberLikeIds(likesParams.MemberId);

                    result = query
                        .Where(x => x.TargetMemberID == likesParams.MemberId
                            && likeIds.Contains(x.SourceMemberID))
                        .Select(x => x.SourceMember);
                    break;
            }

            return await PaginationHelper.CreateAsync(result,
                likesParams.PageNumber, likesParams.PageSize);
        }

        public async Task<bool> SaveAllChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
