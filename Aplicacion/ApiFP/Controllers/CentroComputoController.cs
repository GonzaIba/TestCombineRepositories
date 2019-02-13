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
using System.Data.SqlClient;
using System.Globalization;
using System.Data.Entity;
using ApiFP.Helpers;
using Newtonsoft.Json;

namespace ApiFP.Controllers
{
    [RoutePrefix("cc")]
    public class CentroComputoController : BaseApiController
    {

        [AllowAnonymous]
        [Route("facturas/list/{cuitDestino}")]
        [HttpGet]
        public async Task<List<GetFacturaCCBindingModel>> GetFacturas(string cuitDestino)
        {
            List<GetFacturaCCBindingModel> facturaList = null;

            if (ValidateApiKey())
            {
                DataAccessService service = new DataAccessService();
                facturaList = service.GetFacturasCC(cuitDestino);
            }
            else
            {
                throw new Exception("ApiKey invalida");
            }

            return facturaList;
        }

        [AllowAnonymous]
        [Route("facturas/{facturaId}")]
        [HttpGet]
        public async Task<GetFacturaCCDBindingModel> GetFactura(Int64 facturaId)
        {            
            GetFacturaCCDBindingModel fac = null;

            if (ValidateApiKey())
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    Factura factura = db.Facturas.Find(facturaId);

                    if ((factura != null) && (factura.EstadoFacturaFK == 2))
                    {

                        fac = new GetFacturaCCDBindingModel()
                        {
                            Id = factura.Id,
                            Tipo = factura.Tipo,
                            Numero = factura.Numero,
                            Importe = factura.Importe,
                            CuitOrigen = factura.CuitOrigen,
                            CuitDestino = factura.CuitDestino,
                            Fecha = factura.Fecha.HasValue ? factura.Fecha.Value.ToString("d", CultureInfo.CreateSpecificCulture("es-ES")) : null,
                            Detalle = factura.Detalle,
                            Servicio = factura.Servicio,
                            IvaDiscriminado = factura.IvaDiscriminado,
                            Retenciones = factura.Retenciones,
                            Percepciones = factura.Percepciones,
                            ImpuestosNoGravados = factura.ImpuestosNoGravados,
                            SinArchivo = factura.SinArchivo
                        };

                        if (factura.SinArchivo.HasValue && !factura.SinArchivo.Value)
                        {
                            Infrastructure.Archivo archivoDb = db.Archivos.FirstOrDefault(x => x.FacturaIdFK == facturaId);

                            StorageService storageService = new StorageService();

                            Models.Archivo archivo = new Models.Archivo();
                            archivo.Nombre = archivoDb.Nombre;
                            archivo.Extension = archivoDb.Extension;
                            archivo.ContenidoBase64 = storageService.Restore(archivoDb.Ruta, archivoDb.Volumen, archivoDb.Ruta);

                            fac.Archivo = archivo;
                        }

                        factura.EstadoFacturaFK = 3;
                        db.SaveChanges();

                    }
                    else
                    {
                        ModelState.AddModelError("NotFound", "No se ha encontrado la factura especificada.");
                        //return BadRequest(ModelState);
                    };
                }
            }
            else
            {
                throw new Exception("ApiKey invalida");
            }

            return fac;
        }

        private bool ValidateApiKey()
        {
            IEnumerable<string> headerValues = Request.Headers.GetValues("ApiKey");
            var apiKey = headerValues.FirstOrDefault();

            ApplicationDbContext db = new ApplicationDbContext();
            CentroComputo centroComputo = db.CentrosDeComputo.FirstOrDefault(x => x.ApiKey == apiKey);

            return (centroComputo != null);
        }
    }
}