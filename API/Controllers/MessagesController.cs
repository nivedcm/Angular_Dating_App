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
    public class MessagesController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;


        public MessagesController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
        {
            var username = User.GetUserName();
            if (username == createMessageDto.RecipiantUserName) return BadRequest("You cant send yourself messages");
            var sender = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUserName());
            var recipient = await _unitOfWork.UserRepository.GetUserByUsernameAsync(createMessageDto.RecipiantUserName);
            if (recipient == null) return NotFound("Couldnt find this user");
            var message = new Message
            {
                Sender = sender,
                Recipiant = recipient,
                SenderUsername = sender.UserName,
                RecipiantUsername = recipient.UserName,
                Content = createMessageDto.Content
            };
            _unitOfWork.MessageRepository.AddMessage(message);
            if (await _unitOfWork.Complete()) return Ok(_mapper.Map<MessageDto>(message));
            return BadRequest("Failed to send message");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesFromUser([FromQuery]MessageParams messageParams)
        {
            messageParams.Username = User.GetUserName();
            var messages = await _unitOfWork.MessageRepository.GetMessagesForUser(messageParams);
            Response.AddPaginationHeaders(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages);
            return Ok(messages);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            var currentUsername = User.GetUserName();
            var message = await _unitOfWork.MessageRepository.GetMessage(id);
            if (message.Sender.UserName != currentUsername && message.Recipiant.UserName != currentUsername) return Unauthorized();
            if (message.Sender.UserName == currentUsername ) message.SenderDeleted = true;
            if (message.Recipiant.UserName == currentUsername) message.RecipiantDeleted = true;
            if (message.SenderDeleted && message.RecipiantDeleted) _unitOfWork.MessageRepository.DeleteMessage(message);
            if (await _unitOfWork.Complete()) return Ok(_mapper.Map<MessageDto>(message));
            return BadRequest("Failed to delete message");
        }
    }
}
