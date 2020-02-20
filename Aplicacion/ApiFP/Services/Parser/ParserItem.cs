using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace ApiFP.Services.Parser
{
    public abstract class ParserItem
    {
        string caracteresOmitidos;

        public ParserItem()
        {
            caracteresOmitidos = ConfigurationManager.AppSettings["CARACTERES_OMITIDOS"];
        }

        public abstract void Parse(Business.DatosFactura datosExtraidos, String[] lineas);

        protected string encontrarSiguientePalabra(string[] palabras, string palabra)
        {
            for (int j = 0; j < palabras.Length; j++)
            {
                if (palabras[j].Contains(palabra))
                {
                    do
                    {
                        if (palabras.Length <= ++j)
                            return "";
                    } while (caracteresOmitidos.IndexOf(palabras[j]) >= 0 || palabras[j].Contains("Nº"));
                    return palabras[j];
                }
            }
            return "";
        }

        protected virtual string FiltrarPatrones(string linea, List<string> patrones = null)
        {
            foreach (string patron in patrones)
            {
                linea = Regex.Replace(linea, patron, String.Empty, RegexOptions.IgnoreCase);
            }

            return linea;
        }

        protected virtual bool TieneInformacionValida(string linea)
        {
            string patron_numeros = ConfigurationManager.AppSettings["DETALLE_REGEX_PATTERN"];
            string palabrasAOmitir = ConfigurationManager.AppSettings["DETALLE_OMITIR_PALABRAS"];
            Regex rgx1 = new Regex(patron_numeros);
            Regex rgx2 = new Regex(palabrasAOmitir);
            return !(rgx1.IsMatch(linea) || rgx2.IsMatch(linea));
        }
    }
}