using DatingApp.Core.Domain.Entities;
using DatingApp.Core.Domain.RepositoryContracts;
using DatingApp.Core.DTO;
using DatingApp.Core.Helpers;
using DatingApp.Core.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatingApp.Core.Services
{
    public class MembersService : IMembersService
    {
        private readonly IMemberRepository _memberRepository;

        public MembersService(IMemberRepository memberRepository)
        {
            _memberRepository = memberRepository;
        }

        public async Task<PaginatedResult<Member>> GetMembers(MemberParams memberParams)
        {

           return await _memberRepository.GetMembersAsync(memberParams);
        }

        public async Task<Member?> GetMemberByID(Guid? ID)
        {
            if (ID == null)
            { 
                return null;
            }
            Member? member = await _memberRepository.GetMemberByIdAsync(ID.Value);
            return member;
        }

        public async Task<Member?> GetMemberWithNAvigationByID(Guid? ID)
        {
            if (ID == null)
            {
                return null;
            }
            Member? member = await _memberRepository.GetMemberByIdForUpdateAsync(ID.Value);
            return member;
        }

        public async Task<(bool Success, string Message)> UpdateMember(Guid ID, UpdateMemberDto updateMemberDto)
        {
            Member? member = await _memberRepository.GetMemberByIdForUpdateAsync(ID);
            if (member == null)
            {
                return (false, "Member not found");
            }

            member.DisplayName = updateMemberDto.DisplayName ?? member.DisplayName;
            member.User.DisplayName = updateMemberDto.DisplayName ?? member.User.DisplayName;
            member.Description = updateMemberDto.Description ?? member.Description;        
            member.City = updateMemberDto.City ?? member.City;
            member.Country = updateMemberDto.Country ?? member.Country;
            
            

            _memberRepository.Update(member);
            if (await _memberRepository.SaveAllChangesAsync())
            {
                return (true, "success Update member");
            }
            return (false, "failed Update member");
        }

        public Task<IReadOnlyList<Photo>> GetPhotosForMember(Guid memberId)
        {
            if (memberId == Guid.Empty)
                return Task.FromResult<IReadOnlyList<Photo>>(new List<Photo>());

            return _memberRepository.GetPhotosForMemberAsync(memberId);
        }

        public async Task<bool> AddPhotoForMember(Member member, Photo photo)
        {
            if (member == null || photo == null) return false;

            member.Photos.Add(photo);
            return await _memberRepository.SaveAllChangesAsync();
        }

        public async Task<bool> SetMainPhotoForMember(Member member, Photo photo)
        {
            if (member == null || photo == null) return false;

            member.ImageUrl = photo.Url;
            member.User.ImageUrl = photo.Url;

            return await _memberRepository.SaveAllChangesAsync();

        }

        public async Task<bool> RemovePhotoForMember(Member member, Photo photo)
        {
            if (member == null || photo == null) return false;

            member.Photos.Remove(photo);


            return await _memberRepository.SaveAllChangesAsync();

        }
    }
}
