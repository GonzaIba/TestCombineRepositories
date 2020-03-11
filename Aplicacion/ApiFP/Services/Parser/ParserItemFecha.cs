using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace ApiFP.Services.Parser
{
    public class ParserItemFecha : ParserItem
    {
        private Regex patronFecha;

        private List<String> palabrasNoAceptadas;

        public ParserItemFecha()
        {
            this.patronFecha = new Regex(@"\d{2}[/|-]\d{2}[/|-]\d{2,4}");

            palabrasNoAceptadas = new List<string>
            {
                "inicio",
                "venc",
                "vto",
                "per"
            };
        }

        override public void Parse(Business.DatosFactura datosExtraidos, String[] lineas)
        {
            if (String.IsNullOrEmpty(datosExtraidos.Fecha))
            {
                bool esFacFiber = false;

                for (int i = 0; i < 12; ++i)
                    if (lineas[i].ToLower().Contains("telecom argentina s.a."))
                        esFacFiber = true;

                int indiceFecha = 0;
                
                if (esFacFiber)
                {
                    indiceFecha = lineas.Select((valor, indice) => new { valor, indice }).Where(par => EsFecha(par.valor) && par.valor.ToLower().Contains("fecha")).Select(par => par.indice).FirstOrDefault();
                }
                else
                {
                    indiceFecha = lineas.Select((valor, indice) => new { valor, indice }).Where(par => EsFecha(par.valor) && checkearLinea(par.valor.ToLower())).Select(par => par.indice).FirstOrDefault();
                }

                var matchFecha = patronFecha.Match(lineas[indiceFecha]);
                datosExtraidos.Fecha = matchFecha.Value;
            }            
        }

        private bool EsFecha(string texto) => patronFecha.IsMatch(texto);

        private bool checkearLinea(String linea)
        {
            foreach(var s in palabrasNoAceptadas)
            {
                if (linea.Contains(s))
                    return false;
            }
            return true;
        }
    }
}