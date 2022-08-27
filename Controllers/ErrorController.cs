using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoldCap.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/{statusCode}")]
        public ViewResult Index(int statusCode)
        {
            ViewBag.ErrorStatusCode = statusCode;
            return View("NotFound");
        }
    }
}
