﻿using ApiFP.Infrastructure;
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
                Fecha = DateTime.Parse(createFacturaModel.Fecha, new CultureInfo("es-ES", false))               
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
        [Route("")]
        [HttpGet]        
        public async Task<List<GetFacturaBindingModel>> GetFacturasByUser()
        {
            DataAccessService service = new DataAccessService();
            return service.GetFacturasByUser(User.Identity.GetUserId());
        }

        [Authorize]
        [Route("")]
        [HttpPatch]
        public async Task<IHttpActionResult> UpdateFactura(UpdateFacturaBindingModel createFacturaModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                string user = User.Identity.GetUserId();
                var factura = db.Facturas.Find(createFacturaModel.Id);

                if (factura.UserIdFK == user)
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
                };
            }                          

            return Ok();
        }
    }
}