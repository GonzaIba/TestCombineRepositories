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

        public ParserItemDomicilioComercial()
        {
            PALABRA_DOMICILIO_COMERCIAL = ConfigurationManager.AppSettings["PALABRA_DOMICILIO_COMERCIAL"];
        }

        override public void Parse(Business.DatosFactura datosExtraidos, String[] lineas)
        {
            for (int i = 0; i < lineas.Length; i++)
            {
                if (lineas[i].Contains(PALABRA_DOMICILIO_COMERCIAL))
                {
                    datosExtraidos.DomicilioComercial = (String.IsNullOrEmpty(datosExtraidos.DomicilioComercial)) ? lineas[i].Substring(PALABRA_DOMICILIO_COMERCIAL.Length) : datosExtraidos.DomicilioComercial;
                    continue;
                }
            }
        }
    }
}