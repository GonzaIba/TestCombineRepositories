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
using System.Configuration;

namespace ApiFP.Controllers
{
    [RoutePrefix("diagnostics")]
    public class DiagnosticController : BaseApiController
    {
        [AllowAnonymous]
        [Route("status")]
        public IHttpActionResult GetStatus()
        {
            return Ok(new { status = "OK" });
        }

        [AllowAnonymous]
        [Route("storage/path/relative")]
        public IHttpActionResult GetStoragePath()
        {
            string volume = ConfigurationManager.AppSettings["FSS_CURRENT_VOLUME"];
            string pathType = ConfigurationManager.AppSettings["FSS_CURRENT_PATH_TYPE"];            
            string path = System.Web.Hosting.HostingEnvironment.MapPath("~");
            path = path + ConfigurationManager.AppSettings["FSS_VOLUME_" + volume + "_" + pathType];
            return Ok(new { path = path });
        }

    }
}