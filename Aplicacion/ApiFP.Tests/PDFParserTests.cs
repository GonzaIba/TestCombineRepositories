using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
namespace ApiFP.Tests
{
    [TestClass]
    public class PDFParserTests
    {
        [TestMethod]
        public void TestMethod() {
            MemoryStream pdfStream = ApiFP.Services.PDFParser.leerPdf("./Prueba.pdf");
            ApiFP.Services.PDFParser.extraerCamposDePDF(pdfStream);
        }
    }
}
