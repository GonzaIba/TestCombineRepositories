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
            Regex rx = new Regex(@"^[0-9]+$");
            MatchCollection matches;
            List<string> barCode = new List<string>();

            Business.DatosFactura datosExtraidos = new Business.DatosFactura();
            bool primerCuitEncontrado = false;

            for (int i = 0; i < lineas.Length; i++) {

                if (lineas[i].Contains(PALABRA_CLAVE_TIPO)) {
                    if (++i < lineas.Length) {
                        string siguienteLinea = lineas[i].Trim();
                        datosExtraidos.Tipo = siguienteLinea[siguienteLinea.Length - 1].ToString();
                    }
                    continue;
                }

                if (lineas[i].Contains(PALABRA_CLAVE_CUIT) && !primerCuitEncontrado) {
                    string[] palabras = lineas[i].Split();
                    //datosExtraidos.Cuit_Origen = encontrarSiguientePalabra(palabras, PALABRA_CLAVE_CUIT);
                    primerCuitEncontrado = true;
                    continue;
                }

                if (lineas[i].Contains(PALABRA_CLAVE_CUIT) && primerCuitEncontrado) {
                    string[] palabras = lineas[i].Split();
                    datosExtraidos.Cuit_Destino = encontrarSiguientePalabra(palabras, PALABRA_CLAVE_CUIT);
                    continue;
                }



                if (lineas[i].Contains(PALABRA_CLAVE_PUNTO_DE_VENTA)) {
                    string[] palabras = lineas[i].Split();
                    if (datosExtraidos.Tipo != null) {
                        int numeroConvertido; //No se utiliza, solo se declara para poder usar la implementacion de Int32.Tryparse()    
                        string sucursalVenta = encontrarSiguientePalabra(palabras, PALABRA_CLAVE_PUNTO_DE_VENTA.Split()[PALABRA_CLAVE_PUNTO_DE_VENTA.Split().Length - 1]);
                        string comprobante = encontrarSiguientePalabra(palabras, PALABRA_CLAVE_COMPROBANTE);
                        datosExtraidos.Numero = sucursalVenta + comprobante;
                        if (!Int32.TryParse(datosExtraidos.Numero, out numeroConvertido)) {
                            if (++i < lineas.Length) {
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

                if (lineas[i].Contains(PALABRA_CLAVE_IMPORTE_TOTAL)) {
                    string[] palabras = lineas[i].Split();
                    datosExtraidos.Importe = palabras[palabras.Length - 1];
                    continue;
                }

                if (lineas[i].Contains(PALABRA_CLAVE_IVA)) {
                    string[] palabras = lineas[i].Split();
                    datosExtraidos.IvaDescriminado = palabras[palabras.Length - 1];
                    continue;
                }

                if (lineas[i].Contains(PALABRA_CLAVE_GRAVADO)) {
                    string[] palabras = lineas[i].Split();
                    datosExtraidos.ImpuestosNoGravados = palabras[palabras.Length - 1];
                    continue;
                }

                if (lineas[i].Contains(PALABRA_CLAVE_PERCEPCION)) {
                    string[] palabras = lineas[i].Split();
                    datosExtraidos.Percepciones = palabras[palabras.Length - 1];
                    continue;
                }

                if (lineas[i].Contains(PALABRA_CLAVE_FECHA)) {
                    string[] palabras = lineas[i].Split();
                    datosExtraidos.Fecha = palabras[palabras.Length - 1];
                    continue;
                }

                if (empiezaDetalle(lineas[i])) { //TODO: POner palabra qe inciia
                    string separador = ConfigurationManager.AppSettings["SEPARADOR_DETALLE"];
                    for (i++; !lineas[i].Contains(PALABRA_FIN_DETALLE); i++) {
                        if (tieneInformacionValida(lineas[i])) {  //TODO: poner que filtre lineas con cosas raras y palabras especificas. 
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

                matches = rx.Matches(lineas[i]);
                if (matches.Count > 0)
                {
                    foreach (Match item in matches)
                    {
                        if (item.Length == 42)
                        {
                            barCode.Add(item.ToString().Substring(0,11));
                            barCode.Add(item.ToString().Substring(11, 3));
                            barCode.Add(item.ToString().Substring(14, 5));
                            barCode.Add(item.ToString().Substring(19, 14));
                            barCode.Add(item.ToString().Substring(33, 8));
                            barCode.Add(item.ToString().Substring(41, 1));

                            datosExtraidos.Cuit_Origen = barCode[0];
                        }
                    }
                }
            }

            return datosExtraidos;
        }


    }
}