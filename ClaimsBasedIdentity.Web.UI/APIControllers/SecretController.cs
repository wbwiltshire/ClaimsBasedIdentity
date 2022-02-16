using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using ClaimsBasedIdentity.Data.POCO;
using Microsoft.AspNetCore.Http;

namespace ClaimsBasedIdentity.Web.UI.APIControllers
{
    [Authorize]
    [Produces("application/json")]
    [ApiController]
    public class SecretController : ControllerBase
    {
        private readonly ILogger<SecretController> logger;
        private readonly AppSettingsConfiguration settings;

        public SecretController(ILogger<SecretController> l, IOptions<AppSettingsConfiguration> s)
        {
            logger = l;
            settings = s.Value;
        }

        #region GET: /api/Secret/NoRole
        [Authorize]
        [HttpGet]
        [Route("/api/[controller]/NoRole")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SecretResponse))]                      // Needed by Swagger for example value / Schema
        public async Task<IActionResult> NoRole()
        {
            return Ok(await Task.FromResult(new SecretResponse { SecretType = "Classified", Message ="Secret information, but no role" }));
        }
        #endregion

        #region GET: /api/Secret/NoSecret
        [HttpGet]
        [AllowAnonymous]
        [Route("/api/[controller]/NoSecret")]
        public IActionResult NoSecret()
        {
            return Ok(new SecretResponse { SecretType = "Unclassified", Message = "No secret information." });
        }
        #endregion
    }
}
