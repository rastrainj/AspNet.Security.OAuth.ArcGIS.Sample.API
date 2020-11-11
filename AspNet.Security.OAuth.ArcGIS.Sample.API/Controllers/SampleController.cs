using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AspNet.Security.OAuth.ArcGIS.Sample.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SampleController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route("withauth")]
        public ActionResult<bool> GetAuthorized()
        {
            return Ok("Authorized");
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route("anonymous")]
        [AllowAnonymous]
        public ActionResult<bool> GetAnonymous()
        {
            return Ok("Anonymous");
        }

    }
}
