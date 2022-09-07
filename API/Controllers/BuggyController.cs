using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    //purpose of this controller is simply to return errors, so that we can see what we get back from various different responses
    public class BuggyController : BaseApiController
    {
        
        private readonly DataContext _context;
        public BuggyController(DataContext context){
            _context = context;
        }

        [Authorize]
        [HttpGet("auth")]
        public ActionResult<string> GetSecret(){
            return "secret text";
        }

        [HttpGet("not-found")]
        public ActionResult<AppUser> GetNotFound(){
            var thing = _context.MyProperty.Find(-1);

            if(thing == null) return NotFound();

            return Ok(thing);
        }

        [HttpGet("server-error")]
        public ActionResult<string> GetServerError(){
            /**
            this is not good approach as we are silently swallowing the errors 
            and not showing what exactly is wrong
            */  

            // try{
            // var thing = _context.MyProperty.Find(-1);

            // var thingToReturn = thing.ToString();
            // return thingToReturn;

            // }catch(Exception ex){
            //     return StatusCode(500, "Computer says no!");
            // }

            var thing = _context.MyProperty.Find(-1);

            var thingToReturn = thing.ToString();
            return thingToReturn;
        }

        [HttpGet("bad-request")]
        public ActionResult<string> GetBadRequest(){
            return BadRequest("This was not a good request");
        }
        
    }
}