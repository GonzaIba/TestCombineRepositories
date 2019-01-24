using ApiFP.Infrastructure;
using ApiFP.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace ApiFP.Services
{
    public class DataAccessService
    {

        public List<GetFacturaBindingModel> GetFacturasByUser(string userId)
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
                "Where fac.UserIdFK = @user", new SqlParameter("@user", userId)).ToList();
            return facturasList;
        }

    }
}