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
using ApiFP.Services;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Configuration;

namespace ApiFP.Controllers
{
    [RoutePrefix("archivos")]
    public class ArchivosController : BaseApiController
    {

        [Authorize]
        [HttpGet]
        [Route("id/{fileid}")]        
        public async Task<Models.Archivo> GetFileById(string fileid)
        
        {
            ApplicationDbContext db = new ApplicationDbContext();
            Infrastructure.Archivo archivoInfo = db.Archivos.Find(int.Parse(fileid));

            StorageService storageService = new StorageService();            

            Models.Archivo archivo = new Models.Archivo();
            archivo.Nombre = archivoInfo.Nombre;
            archivo.Extension = archivoInfo.Extension;
            archivo.ContenidoBase64 = storageService.Restore(archivoInfo.Ruta, archivoInfo.Volumen, archivoInfo.Ruta);

            return archivo;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("id")]
        public async Task<HttpResponseMessage> GetFileByUrl()

        {
            Models.Archivo archivo;

            try
            {
                Infrastructure.Archivo archivoInfo = new Infrastructure.Archivo();
                //desencriptar parametro            
                string encid = HttpUtility.ParseQueryString(Request.RequestUri.Query).GetValues("encid").First();
                var fileid = archivoInfo.DecryptId(encid);

                //buscar archivo
                ApplicationDbContext db = new ApplicationDbContext();
                archivoInfo = db.Archivos.Find(int.Parse(fileid));

                StorageService storageService = new StorageService();

                archivo = new Models.Archivo();
                archivo.Nombre = archivoInfo.Nombre;
                archivo.Extension = archivoInfo.Extension;
                archivo.ContenidoBase64 = storageService.Restore(archivoInfo.Ruta, archivoInfo.Volumen, archivoInfo.Ruta);

                //var fsResult = new FileStreamResult(fileStream, "application/pdf");
                var bytes = Convert.FromBase64String(archivo.ContenidoBase64);                

                using (MemoryStream ms = new MemoryStream())
                {                    
                    HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                    result.Content = new ByteArrayContent(bytes);

                    string mediaType;
                    if (archivo.Extension.Contains("pdf"))
                    {
                        mediaType = "application/pdf";
                    }
                    else
                    {
                        mediaType = archivo.Extension.Replace(".", "image/");
                    }
                    result.Content.Headers.ContentType = new MediaTypeHeaderValue(mediaType);

                    return result;
                }


            }
            catch(Exception Ex)
            {
                var response = Request.CreateResponse(HttpStatusCode.Moved);
                response.Headers.Location = new Uri(ConfigurationManager.AppSettings["URL_ERROR"]);
                return response;
            }            
        }

    }
}