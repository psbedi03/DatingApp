using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class FallBackController : Controller
    {
        //we will return physical file index.html
        //the use of this is that, in case API doesn't what to do then API will return this file
        public ActionResult Index(){
            return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "index.html"), "text/HTML");
        }
        
    }
}