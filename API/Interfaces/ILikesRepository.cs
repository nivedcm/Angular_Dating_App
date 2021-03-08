using API.Data.Entities;
using API.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface ILikesRepository
    {
        Task<UserLike> GetUserLike(int sourceUserId, int likedUserId);
        Task<AppUser> GetUserWithLikes(int UserId);
        Task<IEnumerable<LikeDto>> GetUserLikes(string predicate, int UserId);
    }
}
