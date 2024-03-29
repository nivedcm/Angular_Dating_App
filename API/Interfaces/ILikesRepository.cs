﻿using API.Data.Entities;
using API.DTOs;
using API.Helpers;
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
        Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams);
    }
}
