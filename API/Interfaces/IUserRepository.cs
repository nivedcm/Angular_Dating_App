using API.Data.Entities;
using API.DTOs;
using API.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface IUserRepository
    {
        Task<AppUser> GetUserByIdAsync(int id);
        Task<AppUser> GetUserByUsernameAsync(string username);
        Task<string> GetUserGender(string username);
        Task<IEnumerable<AppUser>> GetUsersAsync();
        void Update(AppUser user);
       
        Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams);
        Task<MemberDto> GetMemberAsync(string username);
        Task<string> GetUserGenderAsync(string username);
    }
}
