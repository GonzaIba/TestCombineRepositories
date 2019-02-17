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
using ApiFP.Services;
using Archivo = ApiFP.Models.Archivo;

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
        [Route("version")]
        public IHttpActionResult GetApiVersion()
        {
            return Ok(new { apiVersion = typeof(BaseApiController).Assembly.GetName().Version.ToString() });
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


        public class MailTest
        {
            public string destination;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("email/send")]
        public IHttpActionResult SendEmailTest(MailTest mail)
        {
            IdentityMessage mailToSend = new IdentityMessage();
            mailToSend.Destination = mail.destination;
            mailToSend.Subject = "Test";
            mailToSend.Body = "Test Email";

            EmailService service = new EmailService();
            try
            {
                service.Send(mailToSend);
                return Ok();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [AllowAnonymous]
        [HttpPost]
        [Route("storage/cloud/put")]
        public IHttpActionResult PutStorageCloud(Models.Archivo archivo)
        {

            CloudStorageService service = new CloudStorageService();
            var fileName = archivo.Nombre + archivo.Extension;
            service.Store(fileName, archivo.ContenidoBase64);


            return Ok();
        }


        [AllowAnonymous]
        [HttpGet]
        [Route("storage/cloud/get")]
        public Models.Archivo GetStorageCloud()
        {

            CloudStorageService service = new CloudStorageService();

            var storageType = "GCS";
            var volume = ConfigurationManager.AppSettings["GCS_BUCKET_NAME"];

            

            Models.Archivo archivo = new Models.Archivo();
            archivo.Nombre = "Test";
            archivo.Extension = ".pdf";
            archivo.ContenidoBase64 = service.Restore(storageType, volume, "", "Test.pdf");

            return archivo;            
        }
    }
}