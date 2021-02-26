using API.Data;
using API.Data.Entities;
using API.DTOs;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UsersController(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
        {
            var users = await _userRepository.GetMembersAsync();
            return Ok(users);
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDto>> GetUserByUserNameAsync(string username)
        {

            var user = await _userRepository.GetMemberAsync(username);
            return Ok(user);
        }

        //[Authorize]
        //[HttpGet("{id}")]
        //public async Task<ActionResult<AppUser>> GetUserAsync(int id)
        //{
        //    return await _userRepository.GetUserByIdAsync(id);
        //}

        //[HttpPost]
        //public ActionResult AddUserAsync(AppUser user)
        //{
        //    return Ok(_context.Users.Add(user));
        //}
    }
}
