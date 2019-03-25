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

            DirectoryInfo d = new DirectoryInfo(@"./PDFS");//Assuming Test is your Folder
            FileInfo[] Files = d.GetFiles("*.pdf"); //Getting Text files
            foreach (FileInfo file in Files) {
                MemoryStream pdfStream = ApiFP.Services.PDFParser.leerPdf(file.FullName);
                ApiFP.Services.PDFParser pdfParser = new ApiFP.Services.PDFParser();
                pdfParser.extraerCamposDePDF(pdfStream);
            }
        }
    }
}
