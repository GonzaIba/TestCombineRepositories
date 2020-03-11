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
        private List<Regex> _rgxImporte;
        private Regex rgxDetalleImpuestos;

        public ParserItemImporteTotal()
        {
            PALABRA_CLAVE_IMPORTE_TOTAL = ConfigurationManager.AppSettings["PALABRA_CLAVE_IMPORTE_TOTAL"];
            _rgxImporte = new List<Regex>
            {
                new Regex(@"\A\d+[,|\.]\d{2}\Z"),
                new Regex(@"\A\d+\.\d{3},\d{2}\Z"),
                new Regex(@"\A\d+,\d{3}\.\d{2}\Z")
            };

            rgxDetalleImpuestos = new Regex(@"subtotal.*iva.*total\Z");
        }

        override public void Parse(Business.DatosFactura datosExtraidos, String[] lineas)
        {
            for (int i = 0; i < lineas.Length; i++)
            {
                if (String.IsNullOrEmpty(datosExtraidos.Importe))
                {

                    //if (lineas[i].ToLower().Contains("subtotal iva total"))
                    //{
                        
                    //}

                    if (rgxDetalleImpuestos.IsMatch(lineas[i].ToLower()))
                    {
                        String[] palabras = lineas[++i].Split();
                        datosExtraidos.Importe = DarFormato(palabras.Last().Replace("$", ""));
                        continue;
                    }

                    if (lineas[i].Contains(PALABRA_CLAVE_IMPORTE_TOTAL) || lineas[i].ToLower().Contains("total") &&
                    !(lineas[i].ToLower().Contains("recargo") || lineas[i].ToLower().Contains("sub") ||
                    lineas[i].ToLower().Contains("iva")))
                    {
                        if (lineas[i].ToLower().Contains("impuestos"))
                        {
                            String[] aux = lineas[i].Split();
                            datosExtraidos.Importe = DarFormato(aux[aux.Length - 1]);
                            return;
                        }

                        String[] palabras;

                        if (lineas[i].Trim() == "Importe Total:")
                            palabras = lineas[i - 1].Split();
                        else
                            palabras = lineas[i].Split();
                        
                        foreach(var s in palabras)
                        {
                            String input = (s.Contains("$")) ? s.Replace("$", "") : s;
                            foreach(var rgx in _rgxImporte)
                            {
                                Match match = rgx.Match(input);
                                if (match.Success)
                                {
                                    datosExtraidos.Importe = DarFormato(match.Value);

                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }

        public String DarFormato(String importe)
        {
            importe = importe.Replace(",", "").Replace(".", "");
            String decimales = importe.Substring(importe.Length - 2);
            importe = importe.Remove(importe.Length - 2);
            return importe + "." + decimales;

        }
    }
}