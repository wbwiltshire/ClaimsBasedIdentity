using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using ClaimsBasedIdentity.Web.UI.Models;
using ClaimsBasedIdentity.Data.POCO;

namespace ClaimsBasedIdentity.Web.UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> logger;
        private readonly AppSettingsConfiguration settings;

        public HomeController(ILogger<HomeController> l, IOptions<AppSettingsConfiguration> s)
        {
            logger = l;
            settings = s.Value;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        [Route("/[controller]/LoginSuccess")]
        public IActionResult LoginSuccess()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("/[controller]/Logout")]
        public IActionResult Logout()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult About()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("/[controller]/InvalidCredentials")]
        public IActionResult InvalidCredentials()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
