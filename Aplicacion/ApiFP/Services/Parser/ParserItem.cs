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
                    } while (caracteresOmitidos.IndexOf(palabras[j]) >= 0);
                    return palabras[j];
                }
            }
            return "";
        }
    }
}