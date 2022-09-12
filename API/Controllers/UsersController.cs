using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    //we no longer these two now, as we are inheriting from BaseApiController
    //[ApiController]
    //[Route("api/[controller]")]
    
    //by adding authorize here, all the methods within this controller will have to authorize
    [Authorize]
    public class UsersController : BaseApiController
    {
        // public UsersController(DataContext context){
        //     _context = context;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        // }
        public UsersController(IUserRepository userRepository, IMapper mapper){
            _mapper = mapper;
            _userRepository = userRepository;
            
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers(){
            //now to we have extension methods, we will use our method
            // return await _context.MyProperty.ToListAsync();

            var users = await _userRepository.GetMembersAsync();//get users from repository

            // var usersToReturn = _mapper.Map<IEnumerable<MemberDto>>(users);

            return Ok(users);
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDto>> GetUser(string username){
            
            return await _userRepository.GetMemberAsync(username);;
        }
    }
}