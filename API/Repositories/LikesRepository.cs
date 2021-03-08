using API.Data;
using API.Data.Entities;
using API.DTOs;
using API.Extensions;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Repositories
{
    public class LikesRepository : ILikesRepository
    {
        private readonly DataContext _context;
        public LikesRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<UserLike> GetUserLike(int sourceUserId, int likedUserId)
        {
            return await _context.Likes.FindAsync(sourceUserId, likedUserId);
        }

        public async Task<IEnumerable<LikeDto>> GetUserLikes(string predicate, int userId)
        {
            var users = _context.Users.OrderBy(u => u.UserName).AsQueryable();
            var likes = _context.Likes.AsQueryable();

            if(predicate == "liked")
            {
                likes = likes.Where(likes => likes.SourceUserId == userId);
                users = likes.Select(like => like.LikedUser);
            }
            if (predicate == "likedBy")
            {
                likes = likes.Where(likes => likes.LikedUserId == userId);
                users = likes.Select(like => like.SourceUser);
            }
         
            return await users.Select(user => new LikeDto
            {
                UserName = user.UserName,
                Age = user.DateOfBirth.CalculateAge(),
                KnownAs = user.KnownAs,
                PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain).Url,
                City = user.City,
                Id = user.Id
            }).ToListAsync();
        }

        public async Task<AppUser> GetUserWithLikes(int UserId)
        {
            return await _context.Users.Include(x=>x.LikedByUsers).FirstOrDefaultAsync(x => x.Id == UserId);
        }
    }
}
