using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace ApiFP.Services.Parser
{
    public class ParserItemImporteTotal : ParserItem
    {
        string PALABRA_CLAVE_IMPORTE_TOTAL;
        private Regex rgxNumeroDecimal;

        public ParserItemImporteTotal()
        {
            PALABRA_CLAVE_IMPORTE_TOTAL = ConfigurationManager.AppSettings["PALABRA_CLAVE_IMPORTE_TOTAL"];
            rgxNumeroDecimal = new Regex(@"\d+\.\d{2}");
        }

        override public void Parse(Business.DatosFactura datosExtraidos, String[] lineas)
        {
            for (int i = 0; i < lineas.Length; i++)
            {
                if (lineas[i].Contains(PALABRA_CLAVE_IMPORTE_TOTAL) || lineas[i].ToLower().Contains("total") &&
                    !(lineas[i].ToLower().Contains("recargo") || lineas[i].ToLower().Contains("sub") ||
                    lineas[i].ToLower().Contains("iva")))
                {
                    string[] palabras = lineas[i].Split();
                    datosExtraidos.Importe = palabras[palabras.Length - 1];

                    if (String.IsNullOrEmpty(datosExtraidos.Importe) && (lineas[i].Trim() == "Importe Total:"))
                    {
                        datosExtraidos.Importe = lineas[i - 1];
                    }

                    var ds = (datosExtraidos.Importe.Length > 3) ? datosExtraidos.Importe.Substring(datosExtraidos.Importe.Length - 3, 1) : null;

                    if (!String.IsNullOrEmpty(ds) && ((ds == ".") || (ds == ",")))
                    {
                        var importe = datosExtraidos.Importe.Replace(".", "").Replace(",", "");
                        importe = importe.Insert(importe.Length - 2, ".");
                        datosExtraidos.Importe = importe;
                    }

                    datosExtraidos.Importe = datosExtraidos.Importe.Replace("$", "").Trim();
                    //continue;
                    //break;
                    if (rgxNumeroDecimal.IsMatch(datosExtraidos.Importe))
                        return;
                }

            }
        }
    }
}