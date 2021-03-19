using API.Data;
using API.Data.Entities;
using API.DTOs;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public MessageRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        #region  SignalR Connection/ Group Methods
        public void AddGroup(Group group)
        {
            _context.Groups.Add(group);
        }

        public async Task<Connection> GetConnection(string connectionId)
        {
            return await _context.Connections.FindAsync(connectionId);
        }

        public async Task<Group> GetMessageGroup(string groupName)
        {
            return await _context.Groups.Include(x => x.Connections).FirstOrDefaultAsync(x => x.Name == groupName);
        }
        public void RemoveConnection(Connection connection)
        {
            _context.Connections.Remove(connection);
        }
        
        public async Task<Group> GetMessageGroupForConnection(string connectionId)
        {
            return await _context.Groups.Include(c => c.Connections)
                .Where(c => c.Connections.Any(x => x.ConnectionId == connectionId))
                .FirstOrDefaultAsync();
        }
        #endregion

        public void AddMessage(Message message)
        {
            _context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _context.Messages.Remove(message);
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages.Include(x=>x.Sender).Include(x => x.Recipiant).SingleOrDefaultAsync(x=> x.Id == id);
        }

        public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
        {
            var query = _context.Messages
                           .OrderByDescending(m => m.MessageSent)
                           .ProjectTo<MessageDto>(_mapper.ConfigurationProvider)
                           .AsQueryable();
            query = messageParams.Container switch { 
                "Inbox" => query.Where(u=>u.RecipiantUsername == messageParams.Username && !u.RecipiantDeleted),
                "Outbox" => query.Where(u => u.SenderUsername == messageParams.Username && !u.SenderDeleted),
                _ => query.Where(u => u.RecipiantUsername == messageParams.Username && !u.RecipiantDeleted && u.DateRead == null),
            };

            return await PagedList<MessageDto>.CreateAsync(query, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipientUsername)
        {
            var messages = await _context.Messages
                                       .Where(m => m.Recipiant.UserName == currentUsername 
                                                                && !m.RecipiantDeleted
                                                                && m.Sender.UserName == recipientUsername
                                                                || m.Recipiant.UserName == recipientUsername 
                                                                && m.Sender.UserName == currentUsername 
                                                                && !m.SenderDeleted)
                                      .OrderBy(m => m.MessageSent)
                                      .ProjectTo<MessageDto>(_mapper.ConfigurationProvider)
                                      .ToListAsync();

            var unreadMessages = messages.Where(m => m.DateRead == null &&
                                    m.RecipiantUsername == currentUsername).ToList();

            if(unreadMessages.Any())
            {
                foreach(var message in unreadMessages)
                {
                    message.DateRead = DateTime.UtcNow;
                }
            }
            return messages;
        }
    }
}
