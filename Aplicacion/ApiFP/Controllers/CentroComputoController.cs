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
using System.Configuration;

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
                var apiKey = GetApiKey();
                ApplicationDbContext db = new ApplicationDbContext();
                CentroComputo centroComputo = db.CentrosDeComputo.FirstOrDefault(x => x.ApiKey == apiKey);

                DataAccessService service = new DataAccessService();
                facturaList = service.GetFacturasCC(cuitDestino, centroComputo.Id.ToString());
            }
            else
            {
                throw new Exception("ApiKey invalida");
            }

            return facturaList;
        }

        [AllowAnonymous]
        [Route("facturas/list")]
        [HttpPost]
        public async Task<List<GetFacturaCCBindingModel>> GetFacturasLote(List<string> cuitsDestino)
        {
            List<GetFacturaCCBindingModel> facturaList = null;

            try
            {
                if (ValidateApiKey())
                {
                    var apiKey = GetApiKey();
                    ApplicationDbContext db = new ApplicationDbContext();
                    CentroComputo centroComputo = db.CentrosDeComputo.FirstOrDefault(x => x.ApiKey == apiKey);

                    DataAccessService service = new DataAccessService();
                    facturaList = service.GetFacturasCC(cuitsDestino, centroComputo.Id.ToString());
                }
                else
                {
                    throw new Exception("ApiKey invalida");
                }
            }
            catch (Exception ex)
            {

            }

            return facturaList;
        }

        [AllowAnonymous]
        [Route("facturas/{facturaId}")]
        [HttpGet]
        public async Task<GetFacturaCCDBindingModel> GetFactura(Int64 facturaId)
        {            
            GetFacturaCCDBindingModel fac = null;

            try
            {
                if (ValidateApiKey())
                {
                    var apiKey = GetApiKey();
                    using (ApplicationDbContext db = new ApplicationDbContext())
                    {
                        Factura factura = db.Facturas.Find(facturaId);
                        CentroComputo centroComputo = db.CentrosDeComputo.FirstOrDefault(x => x.ApiKey == apiKey);
                        var descargaFactura = db.DescargasFactura.Where(x => x.FacturaIdFK == factura.Id && x.CentroComputoIdFK == centroComputo.Id);

                        int qtyDownload = int.Parse(ConfigurationManager.AppSettings["QTY_MULTIPLE_DOWNLOADS_CC"]);
                        bool descargaValida =
                            (factura != null)
                            &&
                            (
                                (factura.EstadoFacturaFK == 2)
                                ||
                                (
                                    (factura.EstadoFacturaFK == 3)
                                    &&
                                    (
                                        (factura.QtyDescargasCC < qtyDownload)
                                        ||
                                        (
                                            (factura.QtyDescargasCC >= qtyDownload)
                                            &&
                                            (descargaFactura.Count() > 0)
                                        )
                                    )
                                )
                            );


                        if (descargaValida)
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
                                try
                                {
                                    fac.UrlArchivo = ConfigurationManager.AppSettings["BASE_URL_FILE"] + archivoDb.EncryptId();
                                }
                                catch (Exception exa)
                                {
                                    LogHelper.GenerateLog(exa);
                                    throw exa;
                                }

                            }

                            factura.EstadoFacturaFK = 3;

                            var descarga = new Infrastructure.DescargaFactura()
                            {
                                CentroComputoIdFK = centroComputo.Id,
                                FacturaIdFK = factura.Id
                            };
                            db.DescargasFactura.Add(descarga);
                            db.SaveChanges();

                            //validar incremento de descarga
                            var descargasCC = db.DescargasFactura.Where(x => x.FacturaIdFK == factura.Id).Select(m => m.CentroComputoIdFK).Distinct();
                            factura.QtyDescargasCC = descargasCC.Count();
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
            }catch(Exception exf)
            {
                LogHelper.GenerateLog(exf);
                throw exf;
            }
            return fac;
        }

        private bool ValidateApiKey()
        {
            var apiKey = GetApiKey();
            ApplicationDbContext db = new ApplicationDbContext();
            CentroComputo centroComputo = db.CentrosDeComputo.FirstOrDefault(x => x.ApiKey == apiKey);
                
            return (centroComputo != null);
        }

        private string GetApiKey()
        {
            IEnumerable<string> headerValues = Request.Headers.GetValues("ApiKey");
            var apiKey = headerValues.FirstOrDefault();
            return apiKey;
        }
    }
}