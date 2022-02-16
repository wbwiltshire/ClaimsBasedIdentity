using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using ClaimsBasedIdentity.Web.UI.Models;
using ClaimsBasedIdentity.Data.POCO;

namespace ClaimsBasedIdentity.Web.UI.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> logger;
        private readonly AppSettingsConfiguration settings;
        private readonly IConfiguration config;

        public HomeController(ILogger<HomeController> l, IOptions<AppSettingsConfiguration> s, IConfiguration c)
        {
            logger = l;
            settings = s.Value;
            config = c;
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
            ViewBag.Version = config.GetValue<string>("Version");
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
        [AllowAnonymous]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
