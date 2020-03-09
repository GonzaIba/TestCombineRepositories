using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace ApiFP.Services.Parser
{
    public class ParserItemDomicilioComercial : ParserItem
    {
        private string PALABRA_DOMICILIO_COMERCIAL;

        private List<Regex> patronesDomicilio;

        public ParserItemDomicilioComercial()
        {
            PALABRA_DOMICILIO_COMERCIAL = ConfigurationManager.AppSettings["PALABRA_DOMICILIO_COMERCIAL"];

            patronesDomicilio = new List<Regex>
            {
                new Regex(@"\([A-Z]\d{4}[A-Z]{3}\)"),
                new Regex(@"[A-Z]\d{4}[A-Z]{3}"),
                new Regex(@"CP \d{4}")
            };
        }

        override public void Parse(Business.DatosFactura datosExtraidos, String[] lineas)
        {
            if (String.IsNullOrEmpty(datosExtraidos.DomicilioComercial))
            {
                for (int i = 0; i < lineas.Length; i++)
                {
                    if (lineas[i].Contains(PALABRA_DOMICILIO_COMERCIAL))
                    {
                        datosExtraidos.DomicilioComercial = lineas[i].Substring(PALABRA_DOMICILIO_COMERCIAL.Length);
                        return;
                    }

                    foreach(var rgx in patronesDomicilio)
                    {
                        if (rgx.IsMatch(lineas[i]))
                        {
                            datosExtraidos.DomicilioComercial = lineas[i];
                            return;
                        }
                    }
                }
            }
        }
    }
}