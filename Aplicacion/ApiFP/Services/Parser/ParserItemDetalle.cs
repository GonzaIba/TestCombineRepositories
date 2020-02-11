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
                "importe",
                "iva 2.5",
                "subtotal",
                "son pesos",
                "cuota",
                "total",
                "3071171928406000400000000000000",
                "periodo facturado",
                "pagada el"
            };

            patrones = new List<string>
            {
                @"\$\s*",
                @"[\d,]+[\.][\d]{2}",
                @"(\()?[0-9][0-9]([\.][\d]{2})?%(\))?",
                @"[\d]+,[\d]{2}",
                @"%",
                @"\(\)"
            };
            
            expressions = new List<Regex>
            { 
                new Regex(@"U. medida$", options),      //Facturas 1, 2, 3, 4 y 13
                new Regex(@"^IVA$", options),           //Facturas 1, 2, 3, 4 y 13
                new Regex(@"Cantidad Descripcion P. Unitario", options),     //Facturas 5 y 6
                new Regex(@"Cód. Artículo Observaciones", options),   //Facturas 7, 8, 11
                new Regex(@"Descripción Cantidad Importe", options),     //Factura 9
                new Regex(@"Cantidad Código", options),      //Factura 10
                new Regex(@"Codigo Producto", options),      //Factura 12
                new Regex(@"CANT. DESCRIPCION", options),     //Facturas 14, 15, 16 y 17
                new Regex(@"tidad        Descripción", options)     //Facturas 18 y 19
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
                            ? tipoDetalle < 2
                                    ? FiltrarNumerosAlFinal(lineas[i]) + " " : FiltrarCaracteres(lineas[i]) + " "
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