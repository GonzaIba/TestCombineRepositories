using ApiFP.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Net.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using System.Security.Claims;
using ApiFP.Models;

namespace ApiFP.Controllers
{
    [RoutePrefix("api/diagnostics")]
    public class DiagnosticController : BaseApiController
    {
        [AllowAnonymous]
        [Route("status")]
        public IHttpActionResult GetStatus()
        {
            return Ok(new { status = "OK" });
        }

    }
}