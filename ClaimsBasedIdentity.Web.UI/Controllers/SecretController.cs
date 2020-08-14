using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using ClaimsBasedIdentity.Web.UI.Models;
using ClaimsBasedIdentity.Data;
using ClaimsBasedIdentity.Data.POCO;
using ClaimsBasedIdentity.Data.Repository;
using ClaimsBasedIdentity.Data.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;


namespace ClaimsBasedIdentity.Web.UI.Controllers
{
    [Authorize]
    public class SecretController : Controller
    {
        private readonly ILogger<SecretController> logger;
        private readonly AppSettingsConfiguration settings;

        public SecretController(ILogger<SecretController> l, IOptions<AppSettingsConfiguration> s)
        {
            logger = l;
            settings = s.Value;
        }

        [HttpGet]
        [Authorize(Policy = "IsAuthorized")]                         // An alternative way to Authorize
        [Route("/[controller]/AdminSecret")]
        public IActionResult AdminSecret()
        {

            ViewData["Message"] = "Admin secret information";
            return View();
        }

        [HttpGet]
        [Authorize(Policy = "IsAuthorized")]
        [Route("/[controller]/ManagerSecret")]
        public IActionResult ManagerSecret()
        {

            ViewData["Message"] = "Manager secret information.";
            return View();
        }

        [HttpGet]
        [Authorize(Policy = "IsAuthorized")]
        [Route("/[controller]/BasicSecret")]
        public IActionResult BasicSecret()
        {
            ViewData["Message"] = "Basic secret information.";
            return View();
        }

        [HttpGet]
        [Authorize(Policy = "IsAuthorized")]
        [Route("/[controller]/ComboSecret")]
        public IActionResult ComboSecret()
        {
            ViewData["Message"] = "Combo secret information for Manager and Basic.";
            return View();
        }

        [HttpGet]
        [Authorize]
        [Route("/[controller]/NoRole")]
        public IActionResult NoRole()
        {
            ViewData["Message"] = "Secret information, but no role";
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("/[controller]/NoSecret")]
        public IActionResult NoSecret()
        {
            ViewData["Message"] = "No secret information.";
            return View();
        }

        [HttpGet]
        [Authorize(Policy = "IsAdult")]
        [Route("/[controller]/GrownUpSecret")]
        public IActionResult GrownUpSecret()
        {
            ViewData["Message"] = "This information is for grown ups only!";
            return View();
        }

        [HttpGet]
        [Authorize(Policy = "IsAuthorized")]
        [Route("/[controller]/ClaimsSecret")]
        public IActionResult ClaimsSecret()
        {
            ViewData["Message"] = "This information is for the IT Department only!";
            return View();
        }

    }
}
