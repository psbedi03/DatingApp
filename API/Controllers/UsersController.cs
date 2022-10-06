using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
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
        private readonly IPhotoService _photoService;

        // }
        public UsersController(IUserRepository userRepository, IMapper mapper, IPhotoService photoService){
            _photoService = photoService;
            _mapper = mapper;
            _userRepository = userRepository;
            
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers([FromQuery]UserParams userParams){
            //now to we have extension methods, we will use our method
            // return await _context.MyProperty.ToListAsync();
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());
            userParams.CurrentUsername = user.UserName;
            if(string.IsNullOrEmpty(userParams.Gender)){
                userParams.Gender = user.Gender == "male" ? "female" : "male";
            }

            var users = await _userRepository.GetMembersAsync(userParams);//get users from repository

            Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);

            // var usersToReturn = _mapper.Map<IEnumerable<MemberDto>>(users);

            return Ok(users);
        }

        [HttpGet("{username}", Name="GetUser")]
        public async Task<ActionResult<MemberDto>> GetUser(string username){
            
            return await _userRepository.GetMemberAsync(username);;
        }

        [HttpPut()]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto){
            
            //this will give us user's username from the token that the api uses to authenticate the user
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());
            
            /**
            this mapper will will add properties from memberUpdateDto to our user object
            we don't have to manually type the following
            
            user.City = memberUpdateDto.City;
            user.Country = memberUpdateDto.Country;
            user.LookingFor = memberUpdateDto.LookingFor;
            */
            _mapper.Map(memberUpdateDto, user);

            _userRepository.Update(user);

            if(await _userRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Failed to update request");

        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file){
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

            var result = await _photoService.AddPhotoAsync(file);

            if(result.Error != null) return BadRequest(result.Error.Message);

            var photo = new Photo{
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            if(user.Photos.Count == 0){
                photo.IsMain = true;
            }

            user.Photos.Add(photo);

            if(await _userRepository.SaveAllAsync()){                
                //this get user is the name of endpoint httpget{username}
                return CreatedAtRoute("GetUser", new {username = user.UserName} ,_mapper.Map<PhotoDto>(photo));
            }
            
            return BadRequest("Problem Adding Photo");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId){
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

            var photo = user.Photos.FirstOrDefault(x => x.Id ==  photoId);

            if(photo.IsMain) return BadRequest("This is already main photo");

            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
            if(currentMain != null) currentMain.IsMain = false;
            photo.IsMain = true;

            if(await _userRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Failed to set main photo");
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId){
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if(photo == null) return NotFound();

            if(photo.IsMain) return BadRequest("You cannot delete your main photo");

            if(photo.PublicId != null){
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if(result.Error != null) return BadRequest(result.Error.Message);
            }

            user.Photos.Remove(photo);

            if(await _userRepository.SaveAllAsync()) return Ok();

            return BadRequest("Failed to delete the photo");
        }
    }
}