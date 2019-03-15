using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Text;

namespace ApiFP.Services
{
    public class PDFParser {

        const string PALABRA_CLAVE_CUIT = "CUIT";
        const string PALABRA_CLAVE_TIPO = "ORIGINAL";
        const string PALABRA_CLAVE_PUNTO_DE_VENTA = "Punto de Venta";
        const string PALABRA_CLAVE_COMPROBANTE = "Comp.Nro";
        const string PALABRA_CLAVE_IMPORTE_TOTAL = "Importe Total:";
        const string PALABRA_CLAVE_IVA = "IVA 21.00";
        const string PALABRA_CLAVE_GRAVADO = "Gravado";
        const string PALABRA_CLAVE_PERCEPCION = "Percepci";
        const string PALABRA_CLAVE_FECHA = "Fecha de Emisi";
        static string[] caracteresOmitidos = { " ", ":" };
        public static MemoryStream leerPdf(string nombreArchivo) {
            return new MemoryStream(File.ReadAllBytes(nombreArchivo));
        }

        //Extrae los datos de la factura del pdf que se envía como un MemoryStream
        //Luego lo convierte en texto, y lo separa por líneas. 
        //Por último llama a la función extraerDatos para rellenar el objeto DatosFactura      
        //Se suponen que las facturas ocupan una sola página
        public static Business.DatosFactura extraerCamposDePDF(MemoryStream aPdfStream) {
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


    private static string encontrarSiguientePalabra(string[] palabras, string palabra) {
            for (int j = 0; j < palabras.Length; j++) {
                if (palabras[j].Contains(palabra)) {
                    do {
                        if (palabras.Length <= ++j)
                            return "";
                    } while (caracteresOmitidos.Contains(palabras[j]));
                    return palabras[j];
                }
            }
            return "";
        }
        //Se especifican las líneas donde esta la información que debe guardarse 
        //y además como parsear esa línea para obtenerla. 
        private static Business.DatosFactura extraerDatos (String[] lineas) {
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
                        if (datosExtraidos.Tipo == "A") {
                            string sucursalVenta = encontrarSiguientePalabra(palabras, PALABRA_CLAVE_PUNTO_DE_VENTA.Split()[PALABRA_CLAVE_PUNTO_DE_VENTA.Split().Length - 1]);
                            string comprobante = encontrarSiguientePalabra(palabras, PALABRA_CLAVE_COMPROBANTE);
                            datosExtraidos.Numero = sucursalVenta + comprobante;
                        } else {
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
            }

            return datosExtraidos;
        }


    }
}