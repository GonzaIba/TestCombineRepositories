using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiFP.Helpers
{
    /// <summary>
    /// Log de errores y acciones
    /// </summary>
    public static class LogHelper
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Agrega una linea al archivo de log
        /// </summary>
        /// <param name="mensaje">
        /// Mensaje a guardar en el archivo
        /// </param>
        public static void GenerateLog(Exception ex)
        {
            try
            {
                log.Error(ex);
            }
            catch
            {
            }
        }

        /// <summary>
        /// Agrega una linea al archivo de log
        /// </summary>
        /// <param name="mensaje">
        /// Mensaje a guardar en el archivo
        /// </param>
        public static void GenerateInfo(string mensaje)
        {
            try
            {
                log.Info(mensaje);
            }
            catch
            {
            }
        }
    }
}