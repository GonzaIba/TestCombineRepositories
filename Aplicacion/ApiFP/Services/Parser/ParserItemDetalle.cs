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
        private List<Regex> expressions;
        private List<string> palabrasFinDetalle;
        private List<string> patrones;
        private Match _match;
        private RegexOptions options;

        public ParserItemDetalle()
        {
            options = RegexOptions.IgnoreCase;

            palabrasFinDetalle = new List<string>
            {
                "otros tributos",
                "importe"
            };

            patrones = new List<string>
            {
                @"\$\s*",
                @"[\d,]+[\.][\d]{2}",
                @"\([0-9][0-9]%\)"
            };
            
            expressions = new List<Regex>
            { 
                new Regex(@"U. medida$", options),      //Facturas 1,2,3,4,13
                new Regex(@"^IVA$", options),           //Facturas 1,2,3,4,13
                new Regex(@"Cantidad Descripcion P. Unitario", options)     //Facturas 5 y 6
            };
        }

        override public void Parse(Business.DatosFactura datosExtraidos, String[] lineas)
        {
            for (int i = 0; i < lineas.Length; i++)
            {
                int tipoDetalle = ObtenerTipoDetalle(lineas[i]);

                if (!tipoDetalle.Equals(-1))
                {
                    i++;
                    while (!EsFinDetalle(lineas[i]))
                    {
                        datosExtraidos.Detalle += TieneInformacionValida(lineas[i])
                            ? tipoDetalle == 0 || tipoDetalle == 1
                                    ? FiltrarNumerosAlFinal(lineas[i]) + " "
                                    : tipoDetalle.Equals(2)
                                        ? FiltrarCaracteres(lineas[i]) + " "
                                        : String.Empty
                            : String.Empty;

                        i++;
                    }
                }                
            }
        }

        protected override bool TieneInformacionValida(string linea)
        {
            bool resultado = false;
            
            if(base.TieneInformacionValida(linea))
            {
                resultado = true;
            }

            return resultado;
        }

        private int ObtenerTipoDetalle(string linea)
        {
            //bool resultado = palabrasInicioDetalle.Any(p => linea.ToLower().Contains(p));
            int indiceDetalle = -1;

            foreach (var expression in expressions.Select((value, idx) => new { value, idx }))
            {
                this._match = expression.value.Match(linea);

                if (_match.Success)
                {
                    indiceDetalle = expression.idx;
                    break;
                }
            }

            return indiceDetalle;
        }

        private bool EsFinDetalle(string linea)
        {
            return palabrasFinDetalle.Any(p => linea.ToLower().Contains(p));
        }

        private string FiltrarCaracteres(string linea)
        {
            foreach(string patron in patrones)
            {
                linea = Regex.Replace(linea, patron, String.Empty, options);
            }

            return linea;
        }
    }
}