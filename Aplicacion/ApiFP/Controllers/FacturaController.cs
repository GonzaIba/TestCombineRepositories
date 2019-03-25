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
    [RoutePrefix("facturas")]
    public class FacturasController : BaseApiController
    {

        [Authorize]
        [Route("")]
        [HttpPost]
        public async Task<IHttpActionResult> CreateFactura(CreateFacturaBindingModel createFacturaModel)
        {
            string userName = User.Identity.GetUserName();

            LogHelper.GenerateInfo(userName + " request:" + JsonConvert.SerializeObject(createFacturaModel));

            if (!createFacturaModel.ValidarSinArchivo())
            {
                ModelState.AddModelError(string.Empty, "Error en la especificacion del parametro SinArchivo.");
            }

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
                SinArchivo = createFacturaModel.SinArchivo,
                EstadoFacturaFK = 1
            };

            factura.ReadDate(createFacturaModel.Fecha);
            //if (!String.IsNullOrEmpty(createFacturaModel.Fecha)) { factura.Fecha = DateTime.Parse(createFacturaModel.Fecha, new CultureInfo("es-ES", false)); };

            factura.Insert();

            if (createFacturaModel.Archivo != null) {

                var archivo = new Infrastructure.Archivo()
                {
                    Nombre = createFacturaModel.Archivo.Nombre,
                    Extension = createFacturaModel.Archivo.Extension,
                    FacturaIdFK = factura.Id
                };

                StorageService storageService = new StorageService();
                StorageService.StoreResult storeResult = new StorageService.StoreResult();
                storeResult = storageService.Store(archivo.CreateStorageName(), createFacturaModel.Archivo.ContenidoBase64);

                factura.Parse(createFacturaModel.Archivo.ContenidoBase64);

                if (storeResult.Result == 0)
                {
                    archivo.Ruta = storeResult.FullPath; ;
                    archivo.TipoAlmacenamiento = storeResult.StorageType;
                    archivo.Volumen = storeResult.Volume;
                    archivo.Insert();
                }
            }

            return Ok();
        }

        [Authorize]
        [Route("")]
        [HttpGet]
        public async Task<List<GetFacturaBindingModel>> GetFacturasByUser()
        {
            DataAccessService service = new DataAccessService();
            return service.GetFacturas(User.Identity.GetUserId());
        }

        [Authorize]
        [Route("")]
        [HttpPatch]
        public async Task<IHttpActionResult> UpdateFactura(UpdateFacturaBindingModel createFacturaModel)
        {
            string userName = User.Identity.GetUserName();

            LogHelper.GenerateInfo(userName + " request:" + JsonConvert.SerializeObject(createFacturaModel));

            try
            {

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    string user = User.Identity.GetUserId();
                    var factura = db.Facturas.Find(createFacturaModel.Id);

                    //una factura puede ser editada si no se encuentra descargada
                    if ((factura.UserIdFK == user) && (factura.EstadoFacturaFK != 3))
                    {
                        factura.Tipo = createFacturaModel.Tipo;
                        factura.Numero = createFacturaModel.Numero;
                        factura.Importe = createFacturaModel.Importe;
                        factura.CuitOrigen = createFacturaModel.CuitOrigen;
                        factura.CuitDestino = createFacturaModel.CuitDestino;
                        factura.Fecha = DateTime.Parse(createFacturaModel.Fecha, new CultureInfo("es-ES", false));
                        factura.Detalle = createFacturaModel.Detalle;
                        factura.Servicio = createFacturaModel.Servicio;
                        factura.IvaDiscriminado = createFacturaModel.IvaDiscriminado;
                        factura.Retenciones = createFacturaModel.Retenciones;
                        factura.Percepciones = createFacturaModel.Percepciones;
                        factura.ImpuestosNoGravados = createFacturaModel.ImpuestosNoGravados;

                        db.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "La factura ha sido confirmada, no puede ser actualizada.");
                        return BadRequest(ModelState);
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.GenerateInfo(userName + " Exception:" + ex.Message);
                LogHelper.GenerateLog(ex);
                throw ex;
            }
            return Ok();
        }


        [Authorize]
        [Route("{facturaId}")]
        [HttpDelete]
        [HttpPost]
        public async Task<IHttpActionResult> DeleteFactura(int facturaId)
        {
            string user = User.Identity.GetUserId();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            using (ApplicationDbContext db = new ApplicationDbContext())
            {                
                Factura factura = db.Facturas.Find(facturaId);                              

                if ((factura != null) && (factura.UserIdFK == user))
                {
                    if (factura.SinArchivo.HasValue && !factura.SinArchivo.Value)
                    {
                        Infrastructure.Archivo archivo = db.Archivos.FirstOrDefault(x => x.FacturaIdFK == facturaId);

                        if (archivo != null)
                        {
                            archivo.Delete();
                        }
                    }

                    db.Facturas.Remove(factura);
                    db.SaveChanges();
                }
                else
                {
                    ModelState.AddModelError("NotFound", "No se ha encontrado la factura especificada.");
                    return BadRequest(ModelState);
                };
            }

            return Ok();
        }

        [Authorize]
        [Route("{facturaId}")]
        [HttpGet]
        public async Task<GetFacturaBindingModel> GetFacturasById(int facturaId)
        {
            DataAccessService service = new DataAccessService();
            var facturaList = service.GetFacturas(User.Identity.GetUserId(), facturaId);

            return facturaList[0];
        }

        [Authorize]
        [Route("confirm")]
        [HttpPost]
        public async Task<IHttpActionResult> ConfirmFactura(List<int> confirmFacturaModel)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string user = User.Identity.GetUserId();

            using (ApplicationDbContext db = new ApplicationDbContext())
            {                
                var facturas = db.Facturas.Where(x => confirmFacturaModel.Contains(x.Id) && x.UserIdFK == user);

                if (facturas  != null)
                {
                    foreach (Factura factura in facturas)
                    {
                        if (factura.ConfirmacionValida())
                        {
                            factura.Confirmada = true;
                            factura.EstadoFacturaFK = 2;
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Algunas facturas no han sido confirmadas.");
                        }

                    }

                    db.SaveChanges();
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "No se han encontrado las facturas indicadas.");
                    return BadRequest(ModelState);
                };
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok();
        }


        [Authorize]
        [Route("cuit")]
        [HttpGet]
        public async Task<GetCuitsBindingModel> GetCuitsByUser()
        {
            GetCuitsBindingModel cuitlist = new GetCuitsBindingModel();
            DataAccessService service = new DataAccessService();
            cuitlist.CuitDestino = service.GetCuitDestino(User.Identity.GetUserId());
            cuitlist.CuitOrigen = service.GetCuitOrigen(User.Identity.GetUserId());

            return cuitlist;
        }

        [Authorize]
        [Route("detalle/{cuitOrigen}")]
        [HttpGet]
        public async Task<List<string>> GetDetalleByUser(string cuitOrigen)
        {
            
            DataAccessService service = new DataAccessService();
            var listaDetalle = service.GetDetalleOrigen(User.Identity.GetUserId(), cuitOrigen);            

            return listaDetalle;
        }

    }
}