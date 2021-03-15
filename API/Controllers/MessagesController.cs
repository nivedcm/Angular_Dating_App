using API.Data.Entities;
using API.DTOs;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Authorize]
    public class MessagesController : BaseApiController
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public MessagesController(IMessageRepository messageRepository, IUserRepository userRepository, IMapper mapper)
        {
            _messageRepository = messageRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
        {
            var username = User.GetUserName();

            if (username == createMessageDto.RecipiantUserName) return BadRequest("You cant send yourself messages");

            var sender = await _userRepository.GetUserByUsernameAsync(User.GetUserName());
            var recipient = await _userRepository.GetUserByUsernameAsync(createMessageDto.RecipiantUserName);

            if (recipient == null) return NotFound("Couldnt find this user");

            var message = new Message
            {
                Sender = sender,
                Recipiant = recipient,
                SenderUsername = sender.UserName,
                RecipiantUsername = recipient.UserName,
                Content = createMessageDto.Content
            };

            _messageRepository.AddMessage(message);

            if (await _userRepository.SaveAllAsync()) return Ok(_mapper.Map<MessageDto>(message));

            return BadRequest("Failed to send message");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesFromUser([FromQuery]MessageParams messageParams)
        {
            messageParams.Username = User.GetUserName();
            var messages = await _messageRepository.GetMessagesForUser(messageParams);
            Response.AddPaginationHeaders(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages);
            return Ok(messages);
        }


        [HttpGet("thread/{username}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesThread(string username)
        {
            var currentUsername = User.GetUserName();
            return Ok(await _messageRepository.GetMessageThread(currentUsername, username));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            var currentUsername = User.GetUserName();
            var message = await _messageRepository.GetMessage(id);
            if (message.Sender.UserName != currentUsername && message.Recipiant.UserName != currentUsername) return Unauthorized();
            if (message.Sender.UserName == currentUsername ) message.SenderDeleted = true;
            if (message.Recipiant.UserName == currentUsername) message.RecipiantDeleted = true;
            if (message.SenderDeleted && message.RecipiantDeleted) _messageRepository.DeleteMessage(message);
            if (await _userRepository.SaveAllAsync()) return Ok(_mapper.Map<MessageDto>(message));
            return BadRequest("Failed to delete message");
        }
    }
}
