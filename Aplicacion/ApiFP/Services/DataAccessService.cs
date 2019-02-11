using ApiFP.Infrastructure;
using ApiFP.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace ApiFP.Services
{
    public class DataAccessService
    {

        public List<GetFacturaBindingModel> GetFacturas(string userId, int? facturaId = null)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            //var facturas = db.Facturas.Where(x => x.UserIdFK == user).ToList();
            /*
            var facturasList = (from facturas in db.Facturas
                                join archivos in db.Archivos on facturas.Id equals archivos.FacturaIdFK                            
                                where facturas.UserIdFK == user
                                select new
                                  {
                                    facturas,
                                    ArchivoId = archivos.Id
                                }).ToList();
*/

            var facturasList = db.Database.SqlQuery<GetFacturaBindingModel>(
                "SELECT " +
                    "fac.Id," +
                    "fac.Tipo," +
                    "fac.Numero," +
                    "fac.Importe," +
                    "fac.CuitOrigen," +
                    "fac.CuitDestino," +
                    "fac.Detalle," +
                    "fac.Servicio," +
                    "fac.IvaDiscriminado," +
                    "fac.Retenciones," +
                    "fac.Percepciones," +
                    "fac.ImpuestosNoGravados," +
                    "fac.SinArchivo," +
                    "fac.Confirmada," +
                    "convert(varchar, fac.Fecha, 103) as Fecha," +
                    "arc.Id as ArchivoId " +
                "From Facturas as fac " +
                "Left join Archivos as arc on fac.Id = arc.FacturaIdFK " +
                "Where fac.UserIdFK = @user and fac.EstadoFacturaFK <> 3", new SqlParameter("@user", userId)); //.ToList();

            if (facturaId != null)
            {
                return facturasList.Where(x => x.Id == facturaId).ToList();
            }            

            return facturasList.ToList();
        }

        public List<GetFacturaCCBindingModel> GetFacturasCC(string cuitDestino)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            var facturasList = db.Database.SqlQuery<GetFacturaCCBindingModel>(
                            "SELECT " +
                                "fac.Id," +
                                "fac.Tipo," +
                                "fac.Numero," +                                
                                "fac.CuitOrigen," +
                                "fac.CuitDestino," +
                                "fac.SinArchivo," +                                
                                "convert(varchar, fac.Fecha, 103) as Fecha " +                                
                            "From Facturas as fac " +
                            "Where fac.EstadoFacturaFK = 2 and fac.CuitDestino = @cuitDestino", new SqlParameter("@cuitDestino", cuitDestino));

            return facturasList.ToList();
        }

        private string DBDateToString(Nullable<DateTime> fecha)
        {
            if (fecha.HasValue)
            {
                return fecha.Value.ToString("d", CultureInfo.CreateSpecificCulture("es-ES"));
            }

            return "";
        }
    }
}