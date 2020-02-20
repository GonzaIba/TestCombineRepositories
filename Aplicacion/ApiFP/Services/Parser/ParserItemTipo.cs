using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace ApiFP.Services.Parser
{
    public class ParserItemTipo : ParserItem
    {
        string PALABRA_CLAVE_TIPO;
        private List<string> _tipoLista;
        private List<string> _tipoListaCodigo;

        public ParserItemTipo()
        {
            PALABRA_CLAVE_TIPO = ConfigurationManager.AppSettings["PALABRA_CLAVE_TIPO"];

            _tipoLista = new List<string>()
            {
                "A","B","C"
            };

            _tipoListaCodigo = new List<string>()
            {
                "código n° 06", "Código N 06","Código N 6","Codigo N 06"
            };
        }

        override public void Parse(Business.DatosFactura datosExtraidos, String[] lineas)
        {
            for (int i = 0; i < lineas.Length; i++)
            {
                if (String.IsNullOrEmpty(datosExtraidos.Tipo))
                {
                    //foreach (var s in _tipoListaCodigo)
                    //{
                    //    if (lineas[i].ToLower().Contains(s.ToLower()))
                    //    {
                    //        datosExtraidos.Tipo = "B";
                    //        return;
                    //    }
                    //}
                    
                    if (lineas[i].ToLower().Contains(PALABRA_CLAVE_TIPO))
                    {
                        if (++i < lineas.Length)
                        {
                            string siguienteLinea = lineas[i].Trim();
                            datosExtraidos.Tipo = siguienteLinea[siguienteLinea.Length - 1].ToString();
                        }
                        continue;
                    }

                    if (i <= 5)
                    {
                        if (_tipoLista.Contains(lineas[i].Trim()))
                        {
                            datosExtraidos.Tipo = lineas[i].Trim();
                        }

                        if (lineas[i].ToLower().StartsWith("factura "))
                        {
                            var palabras = lineas[i].Split();
                            datosExtraidos.Tipo = palabras[palabras.Length - 1].Trim();
                        }
                    }
                }
            }

            if (!String.IsNullOrEmpty(datosExtraidos.Tipo))
            {
                switch (datosExtraidos.Tipo)
                {

                    case "A":
                    case "B":
                    case "C":
                        break;
                    case "1":
                    case "2":
                    case "3":
                        datosExtraidos.Tipo = datosExtraidos.Tipo.Replace("1", "A");
                        datosExtraidos.Tipo = datosExtraidos.Tipo.Replace("2", "B");
                        datosExtraidos.Tipo = datosExtraidos.Tipo.Replace("3", "C");
                        break;
                    default:
                        datosExtraidos.Tipo = "";
                        break;
                }
            }
        }
    }
}