using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace ApiFP.Services.Parser
{
    public class ParserItemNumeroFactura : ParserItem
    {
        string PALABRA_CLAVE_PUNTO_DE_VENTA;
        string PALABRA_CLAVE_COMPROBANTE;
        private Match _matchesNumeroFactura;
        private List<Regex> _rxNumeroFactura;

        public ParserItemNumeroFactura()
        {
            PALABRA_CLAVE_PUNTO_DE_VENTA = ConfigurationManager.AppSettings["PALABRA_CLAVE_PUNTO_DE_VENTA"];
            PALABRA_CLAVE_COMPROBANTE = ConfigurationManager.AppSettings["PALABRA_CLAVE_COMPROBANTE"];            

            _rxNumeroFactura = new List<Regex>()
            {
                new Regex(@"^\d{4,5}-\d{8}"),
                new Regex(@"^\d-\d{8}"),
                new Regex(@"^\d{4,5} \d{8}"),
                new Regex(@"[A,B,C]{1}-\d{4,5}-\d{8}"),
                new Regex(@"\d{4,5}-\d{8}"),
                new Regex(@"[A,B,C]{1}\d{4,5}-\d{8}")
            };            
        }

        override public void Parse(Business.DatosFactura datosExtraidos, String[] lineas)
        {
            for (int i = 0; i < lineas.Length; i++)
            {
                if (String.IsNullOrEmpty(datosExtraidos.Numero))
                {
                    foreach (var exp in _rxNumeroFactura.Select((value, idx) => new { value, idx }))
                    {
                        _matchesNumeroFactura = exp.value.Match(lineas[i].Trim());
                        if (_matchesNumeroFactura.Success)
                        {
                            switch (exp.idx)
                            {
                                case 0:
                                case 1:
                                case 4:
                                    datosExtraidos.Numero = _matchesNumeroFactura.Value;
                                    break;
                                case 2:
                                    datosExtraidos.Numero = _matchesNumeroFactura.Value.Replace(" ", "-");
                                    break;
                                case 3:
                                    var palabras = _matchesNumeroFactura.Value.Split('-');
                                    datosExtraidos.Tipo = palabras[0];
                                    datosExtraidos.Numero = palabras[1] + "-" + palabras[2];
                                    break;
                                case 5:
                                    var caracteres = _matchesNumeroFactura.Value;
                                    datosExtraidos.Tipo = caracteres[0].ToString();
                                    datosExtraidos.Numero = caracteres.Substring(1);
                                    //datosExtraidos.Tipo = palabras[0];
                                    //datosExtraidos.Numero = palabras[1] + "-" + palabras[2];
                                    break;
                            }
                        }
                    }
                    if (String.IsNullOrEmpty(datosExtraidos.Numero) == false)
                        return;
                }

                //if (lineas[i].Contains(PALABRA_CLAVE_PUNTO_DE_VENTA))
                //{
                //    string[] palabras = lineas[i].Split();
                //    if (datosExtraidos.Tipo != null)
                //    {
                //        int numeroConvertido; //No se utiliza, solo se declara para poder usar la implementacion de Int32.Tryparse()    
                //        string sucursalVenta = encontrarSiguientePalabra(palabras, PALABRA_CLAVE_PUNTO_DE_VENTA.Split()[PALABRA_CLAVE_PUNTO_DE_VENTA.Split().Length - 1]);
                //        string comprobante = encontrarSiguientePalabra(palabras, PALABRA_CLAVE_COMPROBANTE);
                //        datosExtraidos.Numero = sucursalVenta + comprobante;
                //        if (!Int32.TryParse(datosExtraidos.Numero, out numeroConvertido))
                //        {
                //            if (++i < lineas.Length)
                //            {
                //                datosExtraidos.Numero = lineas[i].Trim();
                //            }
                //        }

                //        int cantidadDigitosPuntoDeVenta = 4;
                //        datosExtraidos.Numero = datosExtraidos.Numero.Replace(" ", ""); //.Insert(cantidadDigitosPuntoDeVenta, "-");
                //        if (datosExtraidos.Numero.Length >= 13)
                //        {
                //            cantidadDigitosPuntoDeVenta = 5;
                //        }

                //        datosExtraidos.Numero = datosExtraidos.Numero.Insert(cantidadDigitosPuntoDeVenta, "-");
                //    }
                //    continue;
                //}


                //if (String.IsNullOrEmpty(datosExtraidos.Numero))
                //{
                //    if (lineas[i].ToLower().Contains("número"))
                //    {
                //        string[] palabras = lineas[i].Split();
                //        string[] datos = palabras[palabras.Length - 1].Split('-');

                //        datosExtraidos.Tipo = datos[0];
                //        datosExtraidos.Numero = datos[1] + "-" + datos[2];
                //    }
                //}

                //if (String.IsNullOrEmpty(datosExtraidos.Numero))
                //{
                //    if (lineas[i].ToLower().Contains("nº:") || lineas[i].ToLower().Contains("nº"))
                //    {
                //        string[] palabras = lineas[i].Split();

                //        datosExtraidos.Numero = palabras[palabras.Length - 1];
                //    }
                //}
            }
        }
    }
}