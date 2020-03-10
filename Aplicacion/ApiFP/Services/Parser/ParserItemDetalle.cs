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

        enum FacturasServicios
        {
            aysa,
            metrogas,
            personal,
            edenor,
            edesur,
            fibertel
        }

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
                new Regex(@"Detalle", opciones),
                new Regex(@"Cargos del mes", opciones)
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
                @"\d+\.\d{3},\d{2}",
                @"(( [0-9]+ ?\%)?( [0-9]+,[0-9]+)){2,}$",
                @"\$\s*",
                @"[\d,]+[\.][\d]{2,}",
                @"(\()?[0-9][0-9]([\.][\d]{2})?%(\))?",
                @"[\d]+,[\d]{2,}",
                @"%",
                @"\(\)",
                @"\.{2,}"
            };
        }

        override public void Parse(Business.DatosFactura datosExtraidos, String[] lineas)
        {
            if (String.IsNullOrEmpty(datosExtraidos.Detalle))
            {
                int indiceInicioDetalle = 0;
                int indiceFinDetalle = 0;

                var servicio = determinarServicio(lineas);

                if (servicio == -1)
                {
                    indiceInicioDetalle = lineas.Select((valor, indice) => new { valor, indice }).Where(par => expresionesInicioDetalle.Any(e => e.IsMatch(par.valor))).Select(par => par.indice).FirstOrDefault() + 1;
                    indiceFinDetalle = lineas.Select((valor, indice) => new { valor, indice }).Where(par => expresionesFinDetalle.Any(e => e.IsMatch(par.valor)) && par.indice > indiceInicioDetalle).Select(par => par.indice).FirstOrDefault();
                }
                else
                {
                    switch(servicio)
                    {
                        case (int)FacturasServicios.aysa:
                            indiceInicioDetalle = lineas.Select((valor, indice) => new { valor, indice }).Where(par => par.valor.ToLower().Contains("cargos por servicios")).Select(par => par.indice).FirstOrDefault() + 1;
                            indiceFinDetalle = lineas.Select((valor, indice) => new { valor, indice }).Where(par => par.valor.ToLower().Contains("total a pagar") && par.indice > indiceInicioDetalle).Select(par => par.indice).FirstOrDefault();
                            break;
                        case (int)FacturasServicios.metrogas:
                            indiceInicioDetalle = lineas.Select((valor, indice) => new { valor, indice }).Where(par => par.valor.ToLower().Contains("cargo fijo")).Select(par => par.indice).FirstOrDefault();
                            indiceFinDetalle = lineas.Select((valor, indice) => new { valor, indice }).Where(par => par.valor.ToLower().Contains("total liquidacion") && par.indice > indiceInicioDetalle).Select(par => par.indice).FirstOrDefault() - 1;
                            break;
                        case (int)FacturasServicios.personal:
                            indiceInicioDetalle = lineas.Select((valor, indice) => new { valor, indice }).Where(par => par.valor.ToLower().Contains("unitario total")).Select(par => par.indice).FirstOrDefault() + 1;
                            indiceFinDetalle = lineas.Select((valor, indice) => new { valor, indice }).Where(par => par.valor.ToLower().Contains("total cargos del mes") && par.indice > indiceInicioDetalle).Select(par => par.indice).FirstOrDefault();
                            break;
                        case (int)FacturasServicios.edenor:
                            indiceInicioDetalle = lineas.Select((valor, indice) => new { valor, indice }).Where(par => par.valor.ToLower().Contains("conceptos el")).Select(par => par.indice).FirstOrDefault();
                            indiceFinDetalle = lineas.Select((valor, indice) => new { valor, indice }).Where(par => par.valor.ToLower().Contains("total a pagar") && par.indice > indiceInicioDetalle).Select(par => par.indice).FirstOrDefault();
                            break; ;
                        case (int)FacturasServicios.edesur:
                            indiceInicioDetalle = lineas.Select((valor, indice) => new { valor, indice }).Where(par => par.valor.ToLower().Contains("cargo fijo")).Select(par => par.indice).FirstOrDefault();
                            indiceFinDetalle = lineas.Select((valor, indice) => new { valor, indice }).Where(par => par.valor.ToLower().Contains("total a pagar") && par.indice > indiceInicioDetalle).Select(par => par.indice).FirstOrDefault() - 1;
                            break;
                        case (int)FacturasServicios.fibertel:
                            indiceInicioDetalle = lineas.Select((valor, indice) => new { valor, indice }).Where(par => par.valor.ToLower().Contains("número de ref")).Select(par => par.indice).FirstOrDefault() + 1;
                            indiceFinDetalle = lineas.Select((valor, indice) => new { valor, indice }).Where(par => par.valor.ToLower().Contains("tel. gratuito") && par.indice > indiceInicioDetalle).Select(par => par.indice).FirstOrDefault();
                            break;
                    }
                }

                if (indiceInicioDetalle > 0 && indiceFinDetalle > indiceInicioDetalle)
                {
                    for (int i = indiceInicioDetalle; i < indiceFinDetalle; i++)
                        datosExtraidos.Detalle += TieneInformacionValida(lineas[i]) ? FiltrarPatrones(lineas[i], this.patrones) + " " : String.Empty;
                }
            }
        }

        private int determinarServicio(String[] lineas)
        {
            var servicio = -1;

            foreach(var s in lineas)
            {
                if (s.ToLower().Contains("agua y saneamientos"))
                {
                    servicio = (int)FacturasServicios.aysa;
                    break;
                }
                if (s.ToLower().Contains("metrogas s.a."))
                {
                    servicio = (int)FacturasServicios.metrogas;
                    break;
                }
                if (s.Contains("TELECOM ARGENTINA S.A."))
                {
                    servicio = (int)FacturasServicios.personal;
                    break;
                }
                if (s.ToLower().Contains("edenordigital.com"))
                {
                    servicio = (int)FacturasServicios.edenor;
                    break;
                }
                if (s.Contains("Evolución de su consumo de Energía"))
                {
                    servicio = (int)FacturasServicios.edesur;
                    break;
                }
                if (s.ToLower().Contains("telecom argentina s.a."))
                {
                    servicio = (int)FacturasServicios.fibertel;
                    break;
                }
            }
            return servicio;
        }
    }
}