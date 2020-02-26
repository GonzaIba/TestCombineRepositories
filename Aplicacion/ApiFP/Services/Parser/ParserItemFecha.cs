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

        public ParserItemFecha()
        {
            this.patronFecha = new Regex(@"\d{2}[/|-]\d{2}[/|-]\d{2,4}");
        }

        override public void Parse(Business.DatosFactura datosExtraidos, String[] lineas)
        {
            if (String.IsNullOrEmpty(datosExtraidos.Fecha))
            {
                int indiceFecha = lineas.Select((valor, indice) => new { valor, indice }).Where(par => EsFecha(par.valor) && !par.valor.ToLower().Contains("inicio") && !par.valor.ToLower().Contains("venc") && !par.valor.ToLower().Contains("per")).Select(par => par.indice).FirstOrDefault();
                var matchFecha = patronFecha.Match(lineas[indiceFecha]);
                datosExtraidos.Fecha = matchFecha.Value;
            }            
        }

        private bool EsFecha(string texto) => patronFecha.IsMatch(texto);
    }
}     
//{
//            for (int i = 0; i < lineas.Length; i++)
//            {
//                if (String.IsNullOrEmpty(datosExtraidos.Fecha))
//                {
//                    if (lineas[i].Contains(PALABRA_CLAVE_FECHA) || _palabrasClave.Any(p => lineas[i].ToLower().Contains(p)))
//                    {
//                        if (lineas[i].Any(Char.IsWhiteSpace))
//                        {
//                            string[] palabras = lineas[i].Split();
//                            int indiceFecha = ObtenerIndiceFecha(palabras);

//                            datosExtraidos.Fecha = indiceFecha.Equals(-1)
//                                ? EsFecha(lineas[i - 1]) ? lineas[i - 1] : String.Empty
//                                : palabras[indiceFecha];

//                            break;
//                        }
//                        else
//                        {
//                            int indiceFecha = lineas.First().IndexOf(':') + 1;
//                            datosExtraidos.Fecha = lineas.First().Substring(indiceFecha);
//                        }
//                    }

//                    if (EsFecha(lineas[i]))
//                    {
//                        datosExtraidos.Fecha = _matchFecha.Value;
//                        break;
//                    }
//                }
//            }
//        }
        /// <summary>
        /// Obtiene el índice del string que concuerda con el formato de fecha.
        /// Si no hay ninguna coincidencia, retorna -1.
        /// </summary>
        /// <returns>Índice de fecha en el arreglo de strings.</returns>
//        private int ObtenerIndiceFecha(string[] textos)
//        {
//            int indiceFecha = -1;

//            for(int i = 0; i < textos.Length; i++)
//            {
//                if (EsFecha(textos[i]))
//                {
//                    indiceFecha = i;
//                    break;
//                }
//            }

//            return indiceFecha;
//        }

//        /// <summary>
//        /// Verifica que la cadena de texto corresponda a un formato de fecha válido.
//        /// </summary>
//        /// <param name="texto"></param>
//        /// <returns></returns>
//        private bool EsFecha(string texto)
//        {
//            bool resultado = false;

//            for (int i = 0; i < _rxFecha.Count; i++)
//            {
//                _matchFecha = _rxFecha[i].Match(texto);
//                if (_matchFecha.Success)
//                {
//                    resultado = true;
//                    break;
//                }
//            }

//            return resultado;
//        }
//    }
//}