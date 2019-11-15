using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace ApiFP.Services.Parser
{
    public class ParserItemDetalle : ParserItem
    {
        private string _separador;
        private string PALABRA_FIN_DETALLE;
        private List<string> comienzoDetalle_Palabras;

        public ParserItemDetalle()
        {
            string _separador = ConfigurationManager.AppSettings["SEPARADOR_DETALLE"];
            PALABRA_FIN_DETALLE = ConfigurationManager.AppSettings["PALABRA_FIN_DETALLE"];

            comienzoDetalle_Palabras = new List<string>();
            char separadorChar = ConfigurationManager.AppSettings["SEPARADOR_INICIO_DETALLE"][0];
            string palabrasInicioDetalle = ConfigurationManager.AppSettings["INICIO_DETALLE"];
            foreach (String s in palabrasInicioDetalle.Split(separadorChar))
            {
                comienzoDetalle_Palabras.Add(s);
            }
        }

        override public void Parse(Business.DatosFactura datosExtraidos, String[] lineas)
        {
            for (int i = 0; i < lineas.Length; i++)
            {
                if (empiezaDetalle(lineas[i]))
                { //TODO: POner palabra qe inciia                    
                    for (i++; !lineas[i].Contains(PALABRA_FIN_DETALLE); i++)
                    {
                        if (tieneInformacionValida(lineas[i]))
                        {  //TODO: poner que filtre lineas con cosas raras y palabras especificas. 
                            datosExtraidos.Detalle += filtrarNumerosAlFinal(lineas[i]) + _separador;
                        }
                    }
                    continue;
                }
            }
        }

        private bool empiezaDetalle(string linea)
        {
            foreach (String palabra in comienzoDetalle_Palabras)
            {
                string pattern = palabra + "$";
                Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                if (rgx.IsMatch(linea))
                    return true;
            }
            return false;
        }
    }
}