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
using ApiFP.Services.Parser;

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

        public PDFParser()
        {
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
        public static MemoryStream leerPdf(string nombreArchivo)
        {
            return new MemoryStream(File.ReadAllBytes(nombreArchivo));
        }

        //Extrae los datos de la factura del pdf que se envía como un MemoryStream
        //Luego lo convierte en texto, y lo separa por líneas. 
        //Por último llama a la función extraerDatos para rellenar el objeto DatosFactura      
        //Se suponen que las facturas ocupan una sola página
        public Business.DatosFactura extraerCamposDePDF(MemoryStream aPdfStream)
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


        private string encontrarSiguientePalabra(string[] palabras, string palabra)
        {
            for (int j = 0; j < palabras.Length; j++)
            {
                if (palabras[j].Contains(palabra))
                {
                    do
                    {
                        if (palabras.Length <= ++j)
                            return "";
                    } while (caracteresOmitidos.IndexOf(palabras[j]) >= 0);
                    return palabras[j];
                }
            }
            return "";
        }

        //Se especifican las líneas donde esta la información que debe guardarse 
        //y además como parsear esa línea para obtenerla. 
        private Business.DatosFactura extraerDatos(String[] lineas)
        {
            Regex rxCodigoBarra = new Regex(@"^\d{40,42}$");
            MatchCollection matchesCodigoBarra = null;
            List<string> barCode = new List<string>();

            Business.DatosFactura datosExtraidos = new Business.DatosFactura();

            List<ParserItem> parserList = new List<ParserItem>()
                {
                    new ParserItemTipo(),
                    new ParserItemCuit(),
                    new ParserItemNumeroFactura(),
                    new ParserItemImporteTotal(),
                    new ParserItemDomicilioComercial(),
                    new ParserItemDetalle()
                };
            foreach (ParserItem parser in parserList)
            {
                parser.Parse(datosExtraidos, lineas);
            }

            for (int i = 0; i < lineas.Length; i++)
            {
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

            return datosExtraidos;
        }

        public String[] extraerConEstrategiaPDF(MemoryStream aPdfStream)
        {
            PdfReader pdfReader = new PdfReader(aPdfStream);
            
            ITextExtractionStrategy strategy = new LocationTextExtractionStrategy();            

            string textoActual = PdfTextExtractor.GetTextFromPage(pdfReader, 1, strategy);
            textoActual = Encoding.UTF8.GetString(ASCIIEncoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(textoActual)));
            String[] lineas = textoActual.Split(Environment.NewLine.ToCharArray());            

            pdfReader.Close();
            return lineas;
        }
    }
}