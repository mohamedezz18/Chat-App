using DatingApp.Core.Domain.Entities;
using DatingApp.Core.DTO;
using DatingApp.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatingApp.Core.ServiceContracts
{
    public interface IMembersService
    {
        public Task<PaginatedResult<Member>> GetMembers(MemberParams memberParams);
        public Task<Member?> GetMemberByID( Guid? ID);
        public Task<Member?> GetMemberWithNAvigationByID(Guid? ID);
        public Task<(bool Success, string Message)> UpdateMember(Guid ID, UpdateMemberDto updateMemberDto);

        public Task<IReadOnlyList<Photo>> GetPhotosForMember(Guid memberId);

        public Task<bool> AddPhotoForMember (Member member,Photo photo);

        public Task<bool> SetMainPhotoForMember(Member member, Photo photo);

        public  Task<bool> RemovePhotoForMember(Member member, Photo photo);

    }
}
