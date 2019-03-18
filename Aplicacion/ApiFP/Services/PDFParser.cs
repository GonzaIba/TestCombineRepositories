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
            string pattern = ConfigurationManager.AppSettings["DETALLE_REGEX_PATTERN"];
            Regex rgx = new Regex(pattern);
            return rgx.IsMatch(linea);
        }

        private bool empiezaDetalle (string linea)
        {
            foreach(String palabra in comienzoDetalle_Palabras) {
                if (linea.Contains(palabra))
                    return true;
            }
            return false;
        }

        //Se especifican las líneas donde esta la información que debe guardarse 
        //y además como parsear esa línea para obtenerla. 
        private Business.DatosFactura extraerDatos (String[] lineas)
        {
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
                    datosExtraidos.Cuit_Origen = encontrarSiguientePalabra(palabras, PALABRA_CLAVE_CUIT);
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
                        datosExtraidos.Numero = datosExtraidos.Numero.Replace(" ", "").Insert(4, "-");
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
                    for (; !lineas[i].Contains(PALABRA_FIN_DETALLE); i++) {
                        if (tieneInformacionValida(lineas[i])) {  //TODO: poner que filtre lineas con cosas raras y palabras especificas. 
                            datosExtraidos.Detalle += lineas[i] + separador;
                        }
                    }
                    continue; 
                }
            }

            return datosExtraidos;
        }


    }
}