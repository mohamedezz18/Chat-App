using DatingApp.Core.Domain.Entities;
using DatingApp.Core.Domain.RepositoryContracts;
using DatingApp.Core.Helpers;
using DatingApp.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatingApp.Infrastructure.Repositories
{
    public class MemberRepository : IMemberRepository
    {
        private readonly ApplicationDbContext _db;
        public MemberRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Member?> GetMemberByIdAsync(Guid id)
        {
           return await _db.Members.FindAsync(id);
        }

        public async Task<Member?> GetMemberByIdForUpdateAsync(Guid id)
        {

            return await _db.Members
                .Include(x => x.User)
                .Include(x => x.Photos)
                .FirstOrDefaultAsync(x => x.Id == id);
        }


        public async Task<PaginatedResult<Member>> GetMembersAsync(MemberParams memberParams)
        {
            var query = _db.Members.AsQueryable().AsNoTracking();

            query = query.Where(x => x.Id != memberParams.CurrentMemberId);

            if (memberParams.Gender != null)
            {
                query = query.Where(x => x.Gender == memberParams.Gender);
            }

            var oldestDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-memberParams.MaxAge - 1));
            var youngestDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-memberParams.MinAge));

            query = query.Where(x => x.DateOfBirth >= oldestDob && x.DateOfBirth <= youngestDob);

            query = memberParams.OrderBy switch
            {
                MemberParams.OrderType.created => query.OrderByDescending(x => x.Created),
                _ => query.OrderByDescending(x => x.LastActive)
            };
            return await PaginationHelper.CreateAsync(query, memberParams.PageNumber, memberParams.PageSize);
        }

        public async Task<IReadOnlyList<Photo>> GetPhotosForMemberAsync(Guid memberId)
        {
            return await _db.Members.Where(mb => mb.Id == memberId)
                .SelectMany(x => x.Photos)
                .ToListAsync();
        }

        public async Task<bool> SaveAllChangesAsync()
        {
            return await _db.SaveChangesAsync() > 0;
        }

        public void Update(Member member)
        {
            _db.Entry(member).State = EntityState.Modified;
        }
    }
}
