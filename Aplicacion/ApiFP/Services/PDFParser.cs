using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace ApiFP.Services
{
    public class PDFParser {
        public Business.DatosFactura extraerCamposDePDF(MemoryStream aPdfStream) {
            PdfReader pdfReader = new PdfReader(aPdfStream);
            Business.DatosFactura datosExtraidos = new Business.DatosFactura();
            pdfReader.Close();
            return datosExtraidos;
        }
    }
}