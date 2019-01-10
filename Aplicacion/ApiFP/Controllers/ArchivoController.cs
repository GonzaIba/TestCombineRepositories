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
    }
}