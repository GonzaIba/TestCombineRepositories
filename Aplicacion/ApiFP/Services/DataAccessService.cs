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

        public List<GetFacturaCCBindingModel> GetFacturasCC(string cuitDestino, string centroComputoId)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            var facturasList = db.Database.SqlQuery<GetFacturaCCBindingModel>(
                            "SELECT " +
                                "tf.Id," +
                                "tf.Tipo," +
                                "tf.Numero," +
                                "tf.CuitOrigen," +
                                "tf.CuitDestino," +
                                "tf.SinArchivo," +
                                "convert(varchar, tf.Fecha, 103) as Fecha " +                                
                            "From [Facturas] as tf " +
                            "where " +
                                "(tf.CuitDestino = @cuitDestino) " +
                                "and " +
                                "(" +
                                    "(tf.EstadoFacturaFK = 2) " + //confirmada
                                    "or " +
                                    "(" +
                                        "(tf.EstadoFacturaFK = 3) " + //descargada
                                        "and " +
                                        "(tf.QtyDescargasCC < 2) " +
                                        "and " +
                                        "not exists(select * from[DescargasFactura] as td where td.FacturaIdFK = tf.Id and td.CentroComputoIdFK = @centroComputo) " +
                                    ") " +
                                ") "
                            , new SqlParameter("@cuitDestino", cuitDestino), new SqlParameter("@centroComputo", centroComputoId));

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


        public List<string> GetCuitDestino(string userId)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            var facturasList = db.Database.SqlQuery<string>(
                    "select distinct top 5 CuitDestino " +
                    "From Facturas as fac " +
                    "Where(fac.CuitDestino is not null) and fac.UserIdFK = @user", new SqlParameter("@user", userId)); //.ToList();

            return facturasList.ToList();
        }

        public List<string> GetCuitOrigen(string userId)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            var facturasList = db.Database.SqlQuery<string>(
                    "select distinct top 5 CuitOrigen " +
                    "From Facturas as fac " +
                    "Where(fac.CuitOrigen is not null) and fac.UserIdFK = @user", new SqlParameter("@user", userId)); //.ToList();

            return facturasList.ToList();
        }

        public List<string> GetDetalleOrigen(string userId, string cuitOrigen)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            var detalleList = db.Database.SqlQuery<string>(
                    "select distinct top 5 Detalle " +
                    "From Facturas as fac " +
                    "Where(fac.Detalle is not null) " +
                        "and fac.UserIdFK = @user " +
                        "and fac.CuitOrigen = @cuitOrigen", new SqlParameter("@user", userId), new SqlParameter("@cuitOrigen", cuitOrigen)); //.ToList();

            return detalleList.ToList();
        }
    }
}