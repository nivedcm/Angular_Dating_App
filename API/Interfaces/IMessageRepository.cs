﻿using API.Data.Entities;
using API.DTOs;
using API.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface IMessageRepository
    {
        void AddMessage(Message message);
        void DeleteMessage(Message message);
        Task<Message> GetMessage(int id);

        Task<PagedList<MessageDto>> GetMessagesForUser(LikesParams likesParams);
        Task<bool> SaveAllAsync();
    }
}
