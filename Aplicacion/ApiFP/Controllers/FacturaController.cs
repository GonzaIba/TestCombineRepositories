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
using System.IO;
using System.Configuration;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using OfficeOpenXml;
using EncryptionLibrary;

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
            try
            {
                string userName = User.Identity.GetUserName();

                var batchParam = ControllerContext.Request.GetQueryNameValuePairs().LastOrDefault(x => x.Key == "batch").Value;



                LogHelper.GenerateInfo(userName + " request:" + JsonConvert.SerializeObject(createFacturaModel));

                if (!createFacturaModel.ValidateNoFile())
                {
                    ModelState.AddModelError(string.Empty, "Error en la especificacion del parametro SinArchivo.");
                }

                if ((batchParam != "Y") && (!createFacturaModel.ValidateMandatory()))
                {
                    ModelState.AddModelError(string.Empty, "Error, no han sido provistos todos los datos obligaotios");
                }

                if (
                    (!String.IsNullOrEmpty(createFacturaModel.Numero) && !String.IsNullOrEmpty(createFacturaModel.CuitOrigen) && !String.IsNullOrEmpty(createFacturaModel.Tipo)) 
                    && 
                    (DataAccessService.GetDuplicates(createFacturaModel.Numero, createFacturaModel.CuitOrigen, createFacturaModel.Tipo) > 0)
                    )
                {
                    ModelState.AddModelError(string.Empty, "Error, numero de factura duplicada.");
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
                    //EstadoFacturaFK = 1,
                    DomicilioComercial = createFacturaModel.DomicilioComercial
                };
                factura.ReadDate(createFacturaModel.Fecha);

                factura.EstadoFacturaFK = factura.ConfirmacionValida() ? 1 : 4;

                //if (!String.IsNullOrEmpty(createFacturaModel.Fecha)) { factura.Fecha = DateTime.Parse(createFacturaModel.Fecha, new CultureInfo("es-ES", false)); };

                factura.Insert();

                if (createFacturaModel.Archivo != null)
                {

                    //factura.Parse(createFacturaModel.Archivo.ContenidoBase64);

                    var archivo = new Infrastructure.Archivo()
                    {
                        Nombre = createFacturaModel.Archivo.Nombre,
                        Extension = createFacturaModel.Archivo.Extension,
                        FacturaIdFK = factura.Id,
                    };
                    archivo.Store(createFacturaModel.Archivo);

                    archivo.Insert();
                }
            }
            catch (Exception ex)
            {
                LogHelper.GenerateLog(ex);
                throw ex;
            }

            return Ok();
        }

        [Authorize]
        [Route("")]
        [HttpGet]
        public async Task<List<GetFacturaBindingModel>> GetFacturasByUser()
        {
            return DataAccessService.GetFacturas(User.Identity.GetUserId());
        }

        [Authorize]
        [Route("lastmonth")]
        [HttpGet]
        public async Task<List<GetFacturaBindingModel>> GetFacturasByUserLastMonth()
        {
            return DataAccessService.GetFacturasLastMonth(User.Identity.GetUserId());
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
                        factura.DomicilioComercial = createFacturaModel.DomicilioComercial;
                        factura.EstadoFacturaFK = factura.ConfirmacionValida() ? 1 : 4;

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
            var facturaList = DataAccessService.GetFacturas(User.Identity.GetUserId(), facturaId);

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

                if (facturas != null)
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
        [Route("cuit/{partialCuit}")]
        [HttpGet]
        public async Task<List<string>> GetCuits([FromUri] string partialCuit)
        {
             return DataAccessService.GetCuitList(partialCuit);            
        }

        [Authorize]
        [Route("cuit")]
        [HttpGet]
        public async Task<GetCuitsBindingModel> GetCuitsByUser()
        {
            GetCuitsBindingModel cuitlist = new GetCuitsBindingModel();
            cuitlist.CuitDestino = DataAccessService.GetCuitDestino(User.Identity.GetUserId());
            cuitlist.CuitOrigen = DataAccessService.GetCuitOrigen(User.Identity.GetUserId());

            return cuitlist;
        }

        [Authorize]
        [Route("detalle/{cuitOrigen}")]
        [HttpGet]
        public async Task<List<string>> GetDetalleByUser(string cuitOrigen)
        {

            var listaDetalle = DataAccessService.GetDetalleOrigen(User.Identity.GetUserId(), cuitOrigen);

            return listaDetalle;
        }

        [Authorize]
        [Route("parse")]
        [HttpPost]
        public async Task<GetFacturaBindingModel> ParseFactura(Models.Archivo archivo)
        {
            //LogHelper.GenerateInfo(userName + " request:" + JsonConvert.SerializeObject(createFacturaModel));
            var factura = new Infrastructure.Factura();
            GetFacturaBindingModel dto = null;
            try
            {
                if (archivo.Extension.ToLower() == ".pdf")
                {
                    factura.Parse(archivo.ContenidoBase64);

                    dto = new GetFacturaBindingModel()
                    {
                        CuitOrigen = factura.CuitOrigen,
                        CuitDestino = factura.CuitDestino,
                        Detalle = factura.Detalle,
                        Tipo = factura.Tipo,
                        Numero = factura.Numero,
                        Importe = factura.Importe,
                        IvaDiscriminado = factura.IvaDiscriminado,
                        Retenciones = factura.Retenciones,
                        Percepciones = factura.Percepciones,
                        ImpuestosNoGravados = factura.ImpuestosNoGravados,
                        Fecha = factura.Fecha.HasValue ? factura.Fecha.Value.ToString("d", new CultureInfo("es-ES", false)) : null,
                        DomicilioComercial = factura.DomicilioComercial
                    };
                }
            }
            catch (Exception ex)
            {

            }

            return dto;
        }

        [Authorize]
        [Route("export-invoice")]
        [HttpPost]

        public async Task ExportFacturas()
        {
            HttpContent requestContent = Request.Content;
            string invoicesId = requestContent.ReadAsStringAsync().Result;

            var ids = invoicesId.Split(',').Select(n => int.Parse(n)).ToArray();
            List<GetFacturaBindingModel> invoices = new List<GetFacturaBindingModel>();

            foreach (int item in ids)
            {
                invoices.Add(DataAccessService.GetFacturas(User.Identity.GetUserId(), item)[0]);
            }

            ExcelPackage ExcelPkg = new ExcelPackage();
            ExcelWorksheet workSheet = ExcelPkg.Workbook.Worksheets.Add("Sheet1");
            
            // Header of the Excel sheet 
            workSheet.Cells[1, 1].Value = "Tipo";
            workSheet.Cells[1, 2].Value = "Numero";
            workSheet.Cells[1, 3].Value = "Importe";
            workSheet.Cells[1, 4].Value = "C. Origen";
            workSheet.Cells[1, 5].Value = "C. Destino";
            workSheet.Cells[1, 6].Value = "Fecha";
            workSheet.Cells[1, 7].Value = "Detalle";
            workSheet.Cells[1, 8].Value = "Domicilio";
            workSheet.Cells[1, 9].Value = "Iva Discriminado";
            workSheet.Cells[1, 10].Value = "Retenciones";
            workSheet.Cells[1, 11].Value = "Servicio";
            workSheet.Cells[1, 12].Value = "Percepciones";
            workSheet.Cells[1, 13].Value = "Impuestos no gravados";
            workSheet.Cells[1, 14].Value = "ID";

            int idx = 2;

            foreach (var item in invoices)
            {
                workSheet.Cells[idx, 1].Value = item.Tipo;
                workSheet.Cells[idx, 2].Value = item.Numero;
                workSheet.Cells[idx, 3].Value = item.Importe;
                workSheet.Cells[idx, 4].Value = item.CuitOrigen;
                workSheet.Cells[idx, 5].Value = item.CuitDestino;
                workSheet.Cells[idx, 6].Value = item.Fecha;
                workSheet.Cells[idx, 7].Value = item.Detalle.Replace("\n", " ");
                workSheet.Cells[idx, 8].Value = item.DomicilioComercial;
                workSheet.Cells[idx, 9].Value = item.IvaDiscriminado;
                workSheet.Cells[idx, 10].Value = item.Retenciones;
                workSheet.Cells[idx, 11].Value = item.Servicio;
                workSheet.Cells[idx, 12].Value = item.Percepciones;
                workSheet.Cells[idx, 13].Value = item.ImpuestosNoGravados;
                workSheet.Cells[idx, 14].Value = clsEncriptar.Encriptar(DateTime.Now.ToString() + "|" +  item.Id.ToString(), ConfigurationManager.AppSettings["PASS_ENCRYPTER"]) ;

                idx += 1;
            }

            MemoryStream ms = new MemoryStream();
            ExcelPkg.SaveAs(ms);
            byte[] ba = ms.GetBuffer();
            HttpContext.Current.Response.ClearContent();
            HttpContext.Current.Response.AddHeader("Content-Disposition", "attachement; filename=RecordExport.xlsx");
            HttpContext.Current.Response.AddHeader("Content-Length", ba.Length.ToString());
            HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            HttpContext.Current.Response.BinaryWrite(ba);
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();

        }

        public async Task<HttpResponseMessage> ExportFactura()
        {
            HttpResponseMessage result = new HttpResponseMessage();

            try
            {
                // 5061,5062

                HttpContent requestContent = Request.Content;
               string invoicesId = requestContent.ReadAsStringAsync().Result;

                var ids = invoicesId.Split(',').Select(n => int.Parse(n)).ToArray();
                List<GetFacturaBindingModel> invoices = new List<GetFacturaBindingModel>();

                foreach(int item in ids)
                {
                    invoices.Add(DataAccessService.GetFacturas(User.Identity.GetUserId(), item)[0]);
                }


                var sb = new StringBuilder();

                var separador = ConfigurationManager.AppSettings["SEPARADOR_CSV"].ToString();

                sb.AppendLine($"\"Tipo\"{separador}\"Numero\"{separador}\"Importe\"{separador}\"C. Origen\"{separador}\"C. Destino\"{separador}\"Fecha\"{separador}\"Detalle\"{separador}\"Domicilio\"{separador}\"Servicio\"{separador}\"Iva Discriminado\"{separador}\"Retenciones\"{separador}\"Percepciones\"{separador}\"Impuestos no gravados\"{separador}\"ID\"");                
                foreach (var item in invoices)
                {
                    sb.AppendLine($"{item.Tipo}{separador}{item.Numero}{separador}{item.Importe}{separador}{item.CuitOrigen}{separador}{item.CuitDestino}{separador}{item.Fecha}{separador}{item.Detalle.Replace("\n", " ")}{separador}{item.DomicilioComercial}{separador}{item.Servicio}{separador}{item.IvaDiscriminado}{separador}{item.Retenciones}{separador}{item.Percepciones}{separador}{item.ImpuestosNoGravados}{separador}{item.Id}");
                }

                result.Content = new StringContent(sb.ToString());
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("text/csv");
                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment"); //attachment will force download
                result.Content.Headers.ContentDisposition.FileName = "RecordExport.csv";
                return result;
            }
            catch (Exception ex)
            {
                result.StatusCode = HttpStatusCode.InternalServerError;
                return result;
            }
        }
    }
}