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
        private List<Regex> expresionesInicioDetalle;
        private List<Regex> expresionesFinDetalle;
        private List<string> patrones;
        private RegexOptions opciones;

        private List<string> lineasDetalle;

        public ParserItemDetalle()
        {
            opciones = RegexOptions.IgnoreCase;

            expresionesInicioDetalle = new List<Regex>
            {
                new Regex(@"U. medida$", opciones),      //Facturas 1, 2, 3, 4 y 13
                new Regex(@"^IVA$", opciones),           //Facturas 1, 2, 3, 4 y 13
                new Regex(@"Cantidad Descripcion P. Unitario", opciones),     //Facturas 5 y 6
                new Regex(@"Cantidad Descripción", opciones),
                new Regex(@"Cód. Artículo Observaciones", opciones),   //Facturas 7, 8, 11
                new Regex(@"Descripción Cantidad Importe", opciones),     //Factura 9
                new Regex(@"Cantidad Código", opciones),      //Factura 10
                new Regex(@"Codigo Producto", opciones),      //Factura 12
                new Regex(@"CANT. DESCRIPCION", opciones),     //Facturas 14, 15, 16 y 17
                new Regex(@"tidad        Descripción", opciones),     //Facturas 18 y 19
                new Regex(@"Detalle", opciones)
            };

            expresionesFinDetalle = new List<Regex>
            {
                new Regex(@"Otros Tributos", opciones),
                new Regex(@"Importe", opciones),
                new Regex(@"IVA 2.5", opciones),
                new Regex(@"Subtotal", opciones),
                new Regex(@"son pesos", opciones),
                new Regex(@"Cuota", opciones),
                new Regex(@"Total", opciones),
                new Regex(@"3071171928406000400000000000000", opciones),
                new Regex(@"Periodo facturado", opciones),
                new Regex(@"Pagada el", opciones)
            };

            patrones = new List<string>
            {
                @"(( [0-9]+ ?\%)?( [0-9]+,[0-9]+)){2,}$",
                @"\$\s*",
                @"[\d,]+[\.][\d]{2}",
                @"(\()?[0-9][0-9]([\.][\d]{2})?%(\))?",
                @"[\d]+,[\d]{2}",
                @"%",
                @"\(\)"
            };

            lineasDetalle = new List<string>();
        }

        override public void Parse(Business.DatosFactura datosExtraidos, String[] lineas)
        {
            if (String.IsNullOrEmpty(datosExtraidos.Detalle))
            {
                int indiceInicioDetalle = 0;
                int indiceFinDetalle = 0;

                bool esMetrogas = false;
                for(int i=0; i<10; ++i)
                {
                    if (lineas[i].ToLower().Contains("metrogas s.a."))
                    {
                        esMetrogas = true;
                        break;
                    }
                }

                if (esMetrogas)
                {
                    indiceInicioDetalle = lineas.Select((valor, indice) => new { valor, indice }).Where(par => par.valor.ToLower().Contains("cargo fijo")).Select(par => par.indice).FirstOrDefault();
                    indiceFinDetalle = lineas.Select((valor, indice) => new { valor, indice }).Where(par => (par.valor.Contains("LIQUIDACION") && par.indice > indiceInicioDetalle)).Select(par => par.indice).FirstOrDefault();
                }
                else
                {
                    indiceInicioDetalle = lineas.Select((valor, indice) => new { valor, indice }).Where(par => expresionesInicioDetalle.Any(e => e.IsMatch(par.valor))).Select(par => par.indice).FirstOrDefault() + 1;
                    indiceFinDetalle = lineas.Select((valor, indice) => new { valor, indice }).Where(par => expresionesFinDetalle.Any(e => e.IsMatch(par.valor)) && par.indice > indiceInicioDetalle).Select(par => par.indice).FirstOrDefault();
                }

                if (indiceInicioDetalle > 0 && indiceFinDetalle > indiceInicioDetalle)
                {
                    for (int i = indiceInicioDetalle; i < indiceFinDetalle; i++)
                        datosExtraidos.Detalle += TieneInformacionValida(lineas[i]) ? FiltrarPatrones(lineas[i], this.patrones) + " " : String.Empty;
                }
            }
        }
    }
}