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
    [RoutePrefix("facturas")]
    public class FacturasController : BaseApiController
    {

        [Authorize]
        [Route("create")]
        public async Task<IHttpActionResult> CreateFactura(CreateFacturaBindingModel createFacturaModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var factura = new Infrastructure.Factura()
            {
                Tipo = createFacturaModel.Tipo,
                Numero = createFacturaModel.Numero,
                Importe = createFacturaModel.Importe,
                CuitOrigen = createFacturaModel.CuitOrigen,
                CuitDestino = createFacturaModel.CuitDestino,
                Detalle = createFacturaModel.Detalle,
                Servicio = createFacturaModel.Servicio,
                IvaDiscriminado = createFacturaModel.IvaDiscriminado,
                Retenciones = createFacturaModel.Retenciones,
                Percepciones = createFacturaModel.Percepciones,
                ImpuestosNoGravados = createFacturaModel.ImpuestosNoGravados,
                UserIdFK = User.Identity.GetUserId(),                
            };

            factura.Insert();

            var archivo = new Infrastructure.Archivo()
            {
                Nombre = createFacturaModel.Archivo.Nombre,
                Extension = createFacturaModel.Archivo.Extension,
                FacturaIdFK = factura.Id
            };

            StorageService storageService = new StorageService();
            StorageService.StoreResult storeResult = new StorageService.StoreResult();
            storeResult = storageService.Store(archivo.CreateStorageName(), createFacturaModel.Archivo.ContenidoBase64);

            if (storeResult.Result == 0)
            {
                archivo.Ruta = storeResult.FullPath; ;
                archivo.TipoAlmacenamiento = storeResult.StorageType;
                archivo.Volumen = storeResult.Volume;
                archivo.Insert();
            }
            
            return Ok();
        }

        [Authorize]
        [HttpGet]
        [Route("user")]
        public async Task<IHttpActionResult> GetFacturasByUser()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            string user = User.Identity.GetUserId();
            var facturas = db.Facturas.Where(x => x.UserIdFK == user).ToList();
            return Ok(facturas);

        }

    }
}