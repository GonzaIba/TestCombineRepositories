using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Text;
using System.Configuration;
using System.Text.RegularExpressions;

namespace ApiFP.Services
{
    public class PDFParser
    {

        string PALABRA_CLAVE_CUIT;
        string PALABRA_CLAVE_TIPO;
        string PALABRA_CLAVE_PUNTO_DE_VENTA;
        string PALABRA_CLAVE_COMPROBANTE;
        string PALABRA_CLAVE_IMPORTE_TOTAL;
        string PALABRA_CLAVE_IVA;
        string PALABRA_CLAVE_GRAVADO;
        string PALABRA_CLAVE_PERCEPCION;
        string PALABRA_CLAVE_FECHA;
        string caracteresOmitidos;
        List<string> comienzoDetalle_Palabras;
        string PALABRA_FIN_DETALLE;
        string PALABRA_DOMICILIO_COMERCIAL;

        public PDFParser ()
        {
            comienzoDetalle_Palabras = new List<string>();
            char separador = ConfigurationManager.AppSettings["SEPARADOR_INICIO_DETALLE"][0];
            string palabrasInicioDetalle = ConfigurationManager.AppSettings["INICIO_DETALLE"];
            foreach(String s in palabrasInicioDetalle.Split(separador)) {
                comienzoDetalle_Palabras.Add(s);
            }
            PALABRA_CLAVE_CUIT = ConfigurationManager.AppSettings["PALABRA_CLAVE_CUIT"];
            PALABRA_CLAVE_TIPO = ConfigurationManager.AppSettings["PALABRA_CLAVE_TIPO"];
            PALABRA_CLAVE_PUNTO_DE_VENTA = ConfigurationManager.AppSettings["PALABRA_CLAVE_PUNTO_DE_VENTA"];
            PALABRA_CLAVE_COMPROBANTE = ConfigurationManager.AppSettings["PALABRA_CLAVE_COMPROBANTE"];
            PALABRA_CLAVE_IMPORTE_TOTAL = ConfigurationManager.AppSettings["PALABRA_CLAVE_IMPORTE_TOTAL"];
            PALABRA_CLAVE_IVA = ConfigurationManager.AppSettings["PALABRA_CLAVE_IVA"];
            PALABRA_CLAVE_GRAVADO = ConfigurationManager.AppSettings["PALABRA_CLAVE_GRAVADO"];
            PALABRA_CLAVE_PERCEPCION = ConfigurationManager.AppSettings["PALABRA_CLAVE_PERCEPCION"];
            PALABRA_CLAVE_FECHA = ConfigurationManager.AppSettings["PALABRA_CLAVE_FECHA"];
            PALABRA_FIN_DETALLE = ConfigurationManager.AppSettings["PALABRA_FIN_DETALLE"];
            caracteresOmitidos = ConfigurationManager.AppSettings["CARACTERES_OMITIDOS"];
            PALABRA_DOMICILIO_COMERCIAL = ConfigurationManager.AppSettings["PALABRA_DOMICILIO_COMERCIAL"];

        }
        public static MemoryStream leerPdf (string nombreArchivo)
        {
            return new MemoryStream(File.ReadAllBytes(nombreArchivo));
        }

        //Extrae los datos de la factura del pdf que se envía como un MemoryStream
        //Luego lo convierte en texto, y lo separa por líneas. 
        //Por último llama a la función extraerDatos para rellenar el objeto DatosFactura      
        //Se suponen que las facturas ocupan una sola página
        public Business.DatosFactura extraerCamposDePDF (MemoryStream aPdfStream)
        {
            PdfReader pdfReader = new PdfReader(aPdfStream);
            Business.DatosFactura datosExtraidos = new Business.DatosFactura();

            ITextExtractionStrategy strategy = new LocationTextExtractionStrategy();
            string textoActual = PdfTextExtractor.GetTextFromPage(pdfReader, 1, strategy);
            textoActual = Encoding.UTF8.GetString(ASCIIEncoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(textoActual)));
            String[] lineas = textoActual.Split(Environment.NewLine.ToCharArray());
            datosExtraidos = extraerDatos(lineas);

            pdfReader.Close();
            return datosExtraidos;
        }


        private string encontrarSiguientePalabra (string[] palabras, string palabra)
        {
            for (int j = 0; j < palabras.Length; j++) {
                if (palabras[j].Contains(palabra)) {
                    do {
                        if (palabras.Length <= ++j)
                            return "";
                    } while (caracteresOmitidos.IndexOf(palabras[j]) >= 0);
                    return palabras[j];
                }
            }
            return "";
        }

        private bool tieneInformacionValida (string linea)
        {
            string patron_numeros = ConfigurationManager.AppSettings["DETALLE_REGEX_PATTERN"];
            string palabrasAOmitir = ConfigurationManager.AppSettings["DETALLE_OMITIR_PALABRAS"];
            Regex rgx1 = new Regex(patron_numeros);
            Regex rgx2 = new Regex(palabrasAOmitir);
            return !(rgx1.IsMatch(linea) || rgx2.IsMatch(linea));
        }

        private bool empiezaDetalle (string linea)
        {
            foreach(String palabra in comienzoDetalle_Palabras) {
                string pattern = palabra + "$";
                Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                if (rgx.IsMatch(linea))
                    return true;
            }
            return false;
        }

        private string filtrarNumerosAlFinal (string linea )
        {
            string patron = ConfigurationManager.AppSettings["NUMEROS_AL_FINAL_PATTERN"];
            return Regex.Replace(linea, patron, "");
        }

        //Se especifican las líneas donde esta la información que debe guardarse 
        //y además como parsear esa línea para obtenerla. 
        private Business.DatosFactura extraerDatos (String[] lineas)
        {
            Regex rxCodigoBarra = new Regex(@"^\d{40,42}$");
            MatchCollection matchesCodigoBarra = null;
            List<string> barCode = new List<string>();

            List<Regex> rxNumeroFactura = new List<Regex>()
            {
                new Regex(@"^\d{4,5}-\d{8}"),
                new Regex(@"^\d-\d{8}"),
                new Regex(@"^\d{4,5} \d{8}"),
                new Regex(@"[A,B,C]{1}-\d{4,5}-\d{8}")                
            };
            Match matchesNumeroFactura = null;

            List<Regex> rxCuit = new List<Regex>()
            {
                new Regex(@"^\d{2}-\d{8}-\d{1}"),
                new Regex(@"^\d{11}"),
                new Regex(@"\d{2}-\d{8}-\d{1}")
            };
            Match matchCuit = null;

            List<string> tipoLista = new List<string>()
            {
                "A","B","C"
            };

            Business.DatosFactura datosExtraidos = new Business.DatosFactura();
            bool primerCuitEncontrado = false;

            for (int i = 0; i < lineas.Length; i++)
            {
                #region "TIPO"
                if (String.IsNullOrEmpty(datosExtraidos.Tipo))
                {
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
                        if (tipoLista.Contains(lineas[i].Trim()))
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
                #endregion

                #region "CUIT_ORIGEN"                
                if (String.IsNullOrEmpty(datosExtraidos.Cuit_Origen))
                {
                    if (lineas[i].Contains(PALABRA_CLAVE_CUIT) && !primerCuitEncontrado)
                    {
                        string[] palabras = lineas[i].Split();
                        datosExtraidos.Cuit_Origen = encontrarSiguientePalabra(palabras, PALABRA_CLAVE_CUIT).Replace("-", "");
                        primerCuitEncontrado = true;
                        continue;
                    }

                    if (String.IsNullOrEmpty(datosExtraidos.Cuit_Origen))
                    {
                        foreach (var exp in rxCuit.Select((value, idx) => new { value, idx }))
                        {
                            matchCuit = exp.value.Match(lineas[i].Trim());
                            if (matchCuit.Success)
                            {
                                switch (exp.idx)
                                {                                    
                                    case 0:
                                        datosExtraidos.Cuit_Origen = matchCuit.Value.Replace("-", "");
                                        break;
                                    case 1:
                                    case 2:
                                        datosExtraidos.Cuit_Origen = matchCuit.Value;
                                        break;
                                        /*
                                    case 2:
                                        var palabras = lineas[i].Split();
                                        datosExtraidos.Cuit_Origen = palabras[palabras.Length - 1].Trim();
                                        break;
                                        */
                                }
                            }
                        }
                    }
                }
                #endregion

                if (lineas[i].Contains(PALABRA_CLAVE_CUIT) && primerCuitEncontrado)
                {
                    string[] palabras = lineas[i].Split();
                    datosExtraidos.Cuit_Destino = encontrarSiguientePalabra(palabras, PALABRA_CLAVE_CUIT);
                    continue;
                }

                #region "NUMERO_FACTURA"

                if (lineas[i].Contains(PALABRA_CLAVE_PUNTO_DE_VENTA))
                {
                    string[] palabras = lineas[i].Split();
                    if (datosExtraidos.Tipo != null)
                    {
                        int numeroConvertido; //No se utiliza, solo se declara para poder usar la implementacion de Int32.Tryparse()    
                        string sucursalVenta = encontrarSiguientePalabra(palabras, PALABRA_CLAVE_PUNTO_DE_VENTA.Split()[PALABRA_CLAVE_PUNTO_DE_VENTA.Split().Length - 1]);
                        string comprobante = encontrarSiguientePalabra(palabras, PALABRA_CLAVE_COMPROBANTE);
                        datosExtraidos.Numero = sucursalVenta + comprobante;
                        if (!Int32.TryParse(datosExtraidos.Numero, out numeroConvertido))
                        {
                            if (++i < lineas.Length)
                            {
                                datosExtraidos.Numero = lineas[i].Trim();
                            }
                        }

                        int cantidadDigitosPuntoDeVenta = 4;
                        datosExtraidos.Numero = datosExtraidos.Numero.Replace(" ", ""); //.Insert(cantidadDigitosPuntoDeVenta, "-");
                        if (datosExtraidos.Numero.Length >= 13)
                        {
                            cantidadDigitosPuntoDeVenta = 5;
                        }

                        datosExtraidos.Numero = datosExtraidos.Numero.Insert(cantidadDigitosPuntoDeVenta, "-");
                    }
                    continue;
                }

                if (String.IsNullOrEmpty(datosExtraidos.Numero))
                {
                    foreach(var exp in rxNumeroFactura.Select((value, idx) => new { value, idx }))
                    {
                        matchesNumeroFactura = exp.value.Match(lineas[i].Trim());
                        if(matchesNumeroFactura.Success)
                        {
                            switch (exp.idx)
                            {
                                case 0:
                                case 1:                                
                                    datosExtraidos.Numero = matchesNumeroFactura.Value;                                    
                                    break;
                                case 2:
                                    datosExtraidos.Numero = matchesNumeroFactura.Value.Replace(" ", "-");
                                    break;
                                case 3:
                                    var palabras = matchesNumeroFactura.Value.Split('-');
                                    datosExtraidos.Tipo = palabras[0];
                                    datosExtraidos.Numero = palabras[1] + "-" + palabras[2];
                                    break;
                            }
                        }
                    }
                }

                if (String.IsNullOrEmpty(datosExtraidos.Numero))
                {
                    if (lineas[i].ToLower().Contains("número"))
                    {
                        string[] palabras = lineas[i].Split();
                        string[] datos = palabras[palabras.Length - 1].Split('-');

                        datosExtraidos.Tipo = datos[0];
                        datosExtraidos.Numero = datos[1] + "-" + datos[2];
                    }
                }
                if (String.IsNullOrEmpty(datosExtraidos.Numero))
                {
                    if (lineas[i].ToLower().Contains("nº:") || lineas[i].ToLower().Contains("nº"))
                    {
                        string[] palabras = lineas[i].Split();

                        datosExtraidos.Numero = palabras[palabras.Length - 1];
                    }
                }
                #endregion

                if (lineas[i].Contains(PALABRA_CLAVE_IMPORTE_TOTAL) || lineas[i].ToLower().Contains("total"))
                {
                    string[] palabras = lineas[i].Split();
                    datosExtraidos.Importe = palabras[palabras.Length - 1];
                    continue;
                }

                if (lineas[i].Contains(PALABRA_CLAVE_IVA))
                {
                    string[] palabras = lineas[i].Split();
                    datosExtraidos.IvaDescriminado = palabras[palabras.Length - 1];
                    continue;
                }

                if (lineas[i].Contains(PALABRA_CLAVE_GRAVADO))
                {
                    string[] palabras = lineas[i].Split();
                    datosExtraidos.ImpuestosNoGravados = palabras[palabras.Length - 1];
                    continue;
                }

                if (lineas[i].Contains(PALABRA_CLAVE_PERCEPCION))
                {
                    string[] palabras = lineas[i].Split();
                    datosExtraidos.Percepciones = palabras[palabras.Length - 1];
                    continue;
                }

                if (lineas[i].Contains(PALABRA_CLAVE_FECHA) || lineas[i].ToLower().Contains("fecha:"))
                {
                    string[] palabras = lineas[i].Split();
                    datosExtraidos.Fecha = palabras[palabras.Length - 1];
                    continue;
                }

                if (empiezaDetalle(lineas[i]))
                { //TODO: POner palabra qe inciia
                    string separador = ConfigurationManager.AppSettings["SEPARADOR_DETALLE"];
                    for (i++; !lineas[i].Contains(PALABRA_FIN_DETALLE); i++)
                    {
                        if (tieneInformacionValida(lineas[i]))
                        {  //TODO: poner que filtre lineas con cosas raras y palabras especificas. 
                            datosExtraidos.Detalle += filtrarNumerosAlFinal(lineas[i]) + separador;
                        }
                    }
                    continue;
                }

                if (lineas[i].Contains(PALABRA_DOMICILIO_COMERCIAL))
                {
                    datosExtraidos.DomicilioComercial = (String.IsNullOrEmpty(datosExtraidos.DomicilioComercial)) ? lineas[i].Substring(PALABRA_DOMICILIO_COMERCIAL.Length) : datosExtraidos.DomicilioComercial;
                    continue;
                }

                if (matchesCodigoBarra == null)
                {
                    matchesCodigoBarra = rxCodigoBarra.Matches(lineas[i]);
                    if (matchesCodigoBarra.Count > 0)
                    {
                        foreach (Match item in matchesCodigoBarra)
                        {
                            if (item.Length == 42)
                            {
                                barCode.Add(item.ToString().Substring(0, 11));
                                barCode.Add(item.ToString().Substring(11, 3));
                                barCode.Add(item.ToString().Substring(14, 5));
                                barCode.Add(item.ToString().Substring(19, 14));
                                barCode.Add(item.ToString().Substring(33, 8));
                                barCode.Add(item.ToString().Substring(41, 1));

                                datosExtraidos.Cuit_Origen = barCode[0];
                            }
                            else if (item.Length == 40)
                            {
                                barCode.Add(item.ToString().Substring(0, 11));
                                barCode.Add(item.ToString().Substring(11, 2));
                                barCode.Add(item.ToString().Substring(13, 4));
                                barCode.Add(item.ToString().Substring(17, 14));
                                barCode.Add(item.ToString().Substring(31, 8));
                                barCode.Add(item.ToString().Substring(39, 1));

                                datosExtraidos.Cuit_Origen = barCode[0];
                            }
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
            return datosExtraidos;
        }


    }
}