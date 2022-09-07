using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Errors
{
    public class ApiException
    {
        //we always want status code
        //if we don't give msg and details then it will null by default
        public ApiException(int statusCode, string message = null, string details=null)
        {
            StatusCode = statusCode;
            Message = message;
            Details = details;
        }

        public int StatusCode { get; set; }
        public string Message { get; set; }

        public string Details { get; set; }
    }
}