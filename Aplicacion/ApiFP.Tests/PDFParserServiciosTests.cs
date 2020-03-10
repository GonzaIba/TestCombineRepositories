using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Diagnostics;

namespace ApiFP.Tests
{
    [TestClass]
    public partial class PDFParserServiciosTests
    {
        CultureInfo culture = new CultureInfo("es-ES");

        [TestMethod]
        public void Parse01()
        {
            var factura = new Infrastructure.Factura();

            var serverPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            var fileFullPath = serverPath + "\\FacturasEjemplo\\SE01_aysa.pdf";            
            byte[] bytes = File.ReadAllBytes(fileFullPath);
            string file = Convert.ToBase64String(bytes);

            factura.Parse(file);
            Assert.IsTrue(factura.Numero == "0108-30087064");
            Assert.IsTrue(factura.Tipo == "B");
            Assert.IsTrue(factura.CuitOrigen == "30709565075");
            Assert.IsTrue(factura.Importe == "749.18");
            //Assert.IsTrue(factura.CuitDestino == "20315762694"); //No sale en la factura
            Assert.IsTrue(factura.Fecha == DateTime.Parse("30/11/2019", culture));
            Console.WriteLine($"DETALLE FACTURA: {factura.Detalle}");
        }
        
        [TestMethod]
        public void Parse02()
        {
            var factura = new Infrastructure.Factura();

            var serverPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            var fileFullPath = serverPath + "\\FacturasEjemplo\\SE02_metrogas.pdf";
            byte[] bytes = File.ReadAllBytes(fileFullPath);
            string file = Convert.ToBase64String(bytes);

            factura.Parse(file);
            Assert.IsTrue(factura.Numero == "0008-24703565");
            Assert.IsTrue(factura.Tipo == "B");
            Assert.IsTrue(factura.CuitOrigen == "30657863676");
            Assert.IsTrue(factura.Importe == "759.17");
            Assert.IsTrue(factura.CuitDestino == "11142248600");
            Assert.IsTrue(factura.Fecha == DateTime.Parse("01/11/2019", culture));
            Console.WriteLine($"DETALLE FACTURA: {factura.Detalle}");
        }

        [TestMethod]
        public void Parse03()
        {
            var factura = new Infrastructure.Factura();

            var serverPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            var fileFullPath = serverPath + "\\FacturasEjemplo\\SE03_personal.pdf";
            byte[] bytes = File.ReadAllBytes(fileFullPath);
            string file = Convert.ToBase64String(bytes);

            factura.Parse(file);
            Assert.IsTrue(factura.Numero == "6517-21830718");
            Assert.IsTrue(factura.Tipo == "B");
            Assert.IsTrue(factura.CuitOrigen == "30639453738");
            Assert.IsTrue(factura.Importe == "527.78");
            //Assert.IsTrue(factura.CuitDestino == "20315762694"); //No sale en la factura
            Assert.IsTrue(factura.Fecha == DateTime.Parse("10/12/2019", culture));
            Console.WriteLine($"DETALLE FACTURA: {factura.Detalle}"); //Hecho
        }

        [TestMethod]
        public void Parse04()
        {
            var factura = new Infrastructure.Factura();

            var serverPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            var fileFullPath = serverPath + "\\FacturasEjemplo\\SE04_edenor.pdf";
            byte[] bytes = File.ReadAllBytes(fileFullPath);
            string file = Convert.ToBase64String(bytes);

            factura.Parse(file);
            Assert.IsTrue(factura.Numero == "0014-07891518");
            Assert.IsTrue(factura.Tipo == "A");
            Assert.IsTrue(factura.CuitOrigen == "63500392427"); //No sale en la factura, lo obtiene del codigo de barras
            Assert.IsTrue(factura.Importe == "13958.32");
            Assert.IsTrue(factura.CuitDestino == "30707831142");
            Assert.IsTrue(factura.Fecha == DateTime.Parse("20/11/2019", culture));
            Console.WriteLine($"DETALLE FACTURA: {factura.Detalle}"); //Hecho
        }

        [TestMethod]
        public void Parse05()
        {
            var factura = new Infrastructure.Factura();

            var serverPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            var fileFullPath = serverPath + "\\FacturasEjemplo\\SE05_edesur.pdf";
            byte[] bytes = File.ReadAllBytes(fileFullPath);
            string file = Convert.ToBase64String(bytes);

            factura.Parse(file);
            Assert.IsTrue(factura.Numero == "0201-05213990");
            Assert.IsTrue(factura.Tipo == "B");
            Assert.IsTrue(factura.CuitOrigen == "00903780407"); //No sale en la factura, lo obtiene del codigo de barras
            Assert.IsTrue(factura.Importe == "1377.78");
            Assert.IsTrue(factura.CuitDestino == "20164979025");
            Assert.IsTrue(factura.Fecha == DateTime.Parse("27/11/2019", culture));
            Console.WriteLine($"DETALLE FACTURA: {factura.Detalle}");
        }

        [TestMethod]
        public void Parse06()
        {
            var factura = new Infrastructure.Factura();

            var serverPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            var fileFullPath = serverPath + "\\FacturasEjemplo\\SE06_fiber187.pdf";
            byte[] bytes = File.ReadAllBytes(fileFullPath);
            string file = Convert.ToBase64String(bytes);

            factura.Parse(file);
            Assert.IsTrue(factura.Numero == "8340-03361897");
            Assert.IsTrue(factura.Tipo == "A");
            Assert.IsTrue(factura.CuitOrigen == "30639453738");
            Assert.IsTrue(factura.Importe == "10443.41");
            Assert.IsTrue(factura.CuitDestino == "30707831142");
            Assert.IsTrue(factura.Fecha == DateTime.Parse("14/12/2019", culture));
            Console.WriteLine($"DETALLE FACTURA: {factura.Detalle}"); //Hecho
        }

        [TestMethod]
        public void Parse07()
        {
            var factura = new Infrastructure.Factura();

            var serverPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            var fileFullPath = serverPath + "\\FacturasEjemplo\\SE07_fiber254.pdf";
            byte[] bytes = File.ReadAllBytes(fileFullPath);
            string file = Convert.ToBase64String(bytes);

            factura.Parse(file);
            Assert.IsTrue(factura.Numero == "8340-03378254");
            Assert.IsTrue(factura.Tipo == "A");
            Assert.IsTrue(factura.CuitOrigen == "30639453738");
            Assert.IsTrue(factura.Importe == "5543.02");
            Assert.IsTrue(factura.CuitDestino == "30707831142");
            Assert.IsTrue(factura.Fecha == DateTime.Parse("14/12/2019", culture));
            Console.WriteLine($"DETALLE FACTURA: {factura.Detalle}"); //Hecho
        }

        [TestMethod]
        public void Parse08()
        {
            var factura = new Infrastructure.Factura();

            var serverPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            var fileFullPath = serverPath + "\\FacturasEjemplo\\SE08_307146-rosmino.pdf";
            byte[] bytes = File.ReadAllBytes(fileFullPath);
            string file = Convert.ToBase64String(bytes);

            factura.Parse(file);
            Assert.IsTrue(factura.Numero == "0008-00307146");
            Assert.IsTrue(factura.Tipo == "A");
            Assert.IsTrue(factura.CuitOrigen == "30540873832");
            Assert.IsTrue(factura.Importe == "560.00");
            Assert.IsTrue(factura.CuitDestino == "30707831142");
            Assert.IsTrue(factura.Fecha == DateTime.Parse("27/11/2019", culture));
            Console.WriteLine($"DETALLE FACTURA: {factura.Detalle}"); //Hecho
        }
    }
}
