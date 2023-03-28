using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Diagnostics;

namespace ApiFP.Tests
{
    [TestClass]
    public partial class PDFParserTests
    {
        CultureInfo culture = new CultureInfo("es-ES");

        [TestMethod]
        public void Parse01()
        {
            var factura = new Infrastructure.Factura();
            factura.Parse(_facturas["01"]);
            Assert.IsTrue(factura.Numero == "00002-00000086");
            Assert.IsTrue(factura.Tipo == "C");
            Assert.IsTrue(factura.CuitOrigen == "20082082531");
            Assert.IsTrue(factura.Importe == "2000.00");
            Assert.IsTrue(factura.CuitDestino == "20329565980");
            Assert.IsTrue(factura.Fecha == DateTime.Parse("26/12/2018", culture));
            Assert.IsTrue(factura.Detalle == "SERVICIO DE VOLQUETE ");
            Console.WriteLine($"DETALLE FACTURA: {factura.Detalle}");
        }

        [TestMethod]
        public void Parse02()
        {
            var factura = new Infrastructure.Factura();
            factura.Parse(_facturas["02"]);
            Assert.IsTrue(factura.Numero == "00003-00000097");
            Assert.IsTrue(factura.Tipo == "A");
            Assert.IsTrue(factura.CuitOrigen == "20260497627");
            Assert.IsTrue(factura.Importe == "1800.00");
            Assert.IsTrue(factura.CuitDestino == "20329565980");
            Assert.IsTrue(factura.Fecha == DateTime.Parse("31/03/2019", culture));
            //Assert.IsTrue(factura.Detalle == "otras 001 Set planchetta + tapa + espatula");
            Console.WriteLine($"DETALLE FACTURA: {factura.Detalle}");
        }

        [TestMethod]
        public void Parse03()
        {
            var factura = new Infrastructure.Factura();
            factura.Parse(_facturas["03"]);
            Assert.IsTrue(factura.Numero == "00001-00000071");
            Assert.IsTrue(factura.Tipo == "C");
            Assert.IsTrue(factura.CuitOrigen == "20288620718");
            Assert.IsTrue(factura.Importe == "850.00");
            Assert.IsTrue(factura.CuitDestino == "33557848709");
            Assert.IsTrue(factura.Fecha == DateTime.Parse("07/03/2019", culture));
            //Assert.IsTrue(factura.Detalle == "");
            Console.WriteLine($"DETALLE FACTURA: {factura.Detalle}");
        }

        [TestMethod]
        public void Parse04()
        {
            var factura = new Infrastructure.Factura();
            factura.Parse(_facturas["04"]);
            Assert.IsTrue(factura.Numero == "00002-00000256");
            Assert.IsTrue(factura.Tipo == "C");
            Assert.IsTrue(factura.CuitOrigen == "23084320269");
            Assert.IsTrue(factura.Importe == "4109.00");
            Assert.IsTrue(factura.CuitDestino == "30708221542");
            Assert.IsTrue(factura.Fecha == DateTime.Parse("08/01/2019", culture));
            //Assert.IsTrue(factura.Detalle == "Publicidad Pequeñas Noticias - Aviso simple entre notas 400x300 (ENERO/2019)");
            Console.WriteLine($"DETALLE FACTURA: {factura.Detalle}");
        }

        [TestMethod]
        public void Parse05()
        {
            var factura = new Infrastructure.Factura();
            factura.Parse(_facturas["05"]);
            Assert.IsTrue(factura.Numero == "1001-00000096");
            Assert.IsTrue(factura.Tipo == "A");
            Assert.IsTrue(factura.CuitOrigen == "30711203660");
            Assert.IsTrue(factura.Importe == "3464.00");
            Assert.IsTrue(factura.CuitDestino == "20329565980");
            Assert.IsTrue(factura.Fecha == DateTime.Parse("14/06/2019", culture));
            //Assert.IsTrue(factura.Detalle == "1 Pelikano Hotel Combo 2 Almohadas De Duvet    50x90 1600 Gr");
            Console.WriteLine($"DETALLE FACTURA: {factura.Detalle}");
        }

        [TestMethod]
        public void Parse06()
        {
            var factura = new Infrastructure.Factura();
            factura.Parse(_facturas["06"]);
            Assert.IsTrue(factura.Numero == "3-00000089");
            Assert.IsTrue(factura.Tipo == "A");
            Assert.IsTrue(factura.CuitOrigen == "30715576909");
            Assert.IsTrue(factura.Importe == "1250.00");
            Assert.IsTrue(factura.CuitDestino == "20329565980");
            Assert.IsTrue(factura.Fecha == DateTime.Parse("05/11/2018", culture));
            //Assert.IsTrue(factura.Detalle == "1 5b2e85ef-86f4-4cf0-8c4b-7aabb38f37ff - Cable    2,5mm 3 Rollos X 50 Mts Cobre Electricidad  Normalizado");
            Console.WriteLine($"DETALLE FACTURA: {factura.Detalle}");
        }

        [TestMethod]
        public void Parse07()
        {
            var factura = new Infrastructure.Factura();
            factura.Parse(_facturas["07"]);
            Assert.IsTrue(factura.Numero == "0003-00000121");
            Assert.IsTrue(factura.Tipo == "A");
            Assert.IsTrue(factura.CuitOrigen == "20329565980");
            Assert.IsTrue(factura.Importe == "377.00");
            Assert.IsTrue(factura.CuitDestino == "30715237306");
            Assert.IsTrue(factura.Fecha == DateTime.Parse("04/05/2019", culture));
            //Assert.IsTrue(factura.Detalle == "");
            Console.WriteLine($"DETALLE FACTURA: {factura.Detalle}");
        }

        [TestMethod]
        public void Parse08()
        {
            var factura = new Infrastructure.Factura();
            factura.Parse(_facturas["08"]);
            Assert.IsTrue(factura.Numero == "0002-00028001");
            Assert.IsTrue(factura.Tipo == "A");
            Assert.IsTrue(factura.CuitOrigen == "30714687650");
            Assert.IsTrue(factura.Importe == "2781.79");
            Assert.IsTrue(factura.CuitDestino == "20329565980");
            Assert.IsTrue(factura.Fecha == DateTime.Parse("01/02/2019", culture));
            //Assert.IsTrue(factura.Detalle == "Xubio - Solución       Premium 3000 Observaciones: Abono Febrero 2019");
            Console.WriteLine($"DETALLE FACTURA: {factura.Detalle}");
        }

        [TestMethod]
        public void Parse09()
        {
            var factura = new Infrastructure.Factura();
            factura.Parse(_facturas["09"]);
            Assert.IsTrue(factura.Numero == "0003-00000104");
            Assert.IsTrue(factura.Tipo == "A");
            Assert.IsTrue(factura.CuitOrigen == "20300376879");
            Assert.IsTrue(factura.Importe == "2231.24");
            Assert.IsTrue(factura.CuitDestino == "30716248794");
            Assert.IsTrue(factura.Fecha == DateTime.Parse("05/06/2019", culture));
            //Assert.IsTrue(factura.Detalle == "SERVICIOS DE SOFTWARE - JUNIO 2019 1");
            Console.WriteLine($"DETALLE FACTURA: {factura.Detalle}");
        }

        [TestMethod]
        public void Parse10()
        {
            var factura = new Infrastructure.Factura();
            factura.Parse(_facturas["10"]);
            Assert.IsTrue(factura.Numero == "0004-00001343");
            Assert.IsTrue(factura.Tipo == "A");
            Assert.IsTrue(factura.CuitOrigen == "30715079107");
            Assert.IsTrue(factura.Importe == "9075.00");
            Assert.IsTrue(factura.CuitDestino == "20329565980");
            Assert.IsTrue(factura.Fecha == DateTime.Parse("13/06/2019", culture));
            //Assert.IsTrue(factura.Detalle == "1 Logotipo + manual de marca      1 Landing page publicitaria");
            Console.WriteLine($"DETALLE FACTURA: {factura.Detalle}");
        }

        [TestMethod]
        public void Parse11()
        {
            var factura = new Infrastructure.Factura();
            factura.Parse(_facturas["11"]);
            Assert.IsTrue(factura.Numero == "0004-00000095");
            Assert.IsTrue(factura.Tipo == "A");
            Assert.IsTrue(factura.CuitOrigen == "30715259601");
            Assert.IsTrue(factura.Importe == "3427.00");
            Assert.IsTrue(factura.CuitDestino == "20329565980");
            Assert.IsTrue(factura.Fecha == DateTime.Parse("25/06/2019", culture));
            //Assert.IsTrue(factura.Detalle == "CPS-DP- DICRO MACROLED       GU10-20W ECO 7W 220V CALIDO 2700K 124-01 Embutido Movil       Dicro diam 110mm - Blanco");
            Console.WriteLine($"DETALLE FACTURA: {factura.Detalle}");
        }        

        [TestMethod]
        public void Parse12()
        {
            var factura = new Infrastructure.Factura();
            factura.Parse(_facturas["12"]);
            Assert.IsTrue(factura.Numero == "00003-00000112");
            Assert.IsTrue(factura.Tipo == "A");
            Assert.IsTrue(factura.CuitOrigen == "20225898082");
            Assert.IsTrue(factura.Importe == "18293.51");
            Assert.IsTrue(factura.CuitDestino == "20329565980");
            Assert.IsTrue(factura.Fecha == DateTime.Parse("24/01/2019", culture));
            //Assert.IsTrue(factura.Detalle == "83 Bco Master Stone      1,5 M2 De Bco Tiza Master Stone y bacha Luxor Si71 Johnson aceros");
            Console.WriteLine($"DETALLE FACTURA: {factura.Detalle}");
        }

        [TestMethod]
        public void Parse13()
        {
            var factura = new Infrastructure.Factura();
            factura.Parse(_facturas["13"]);
            Assert.IsTrue(factura.Numero == "00002-00000233");
            Assert.IsTrue(factura.Tipo == "C");
            Assert.IsTrue(factura.CuitOrigen == "27264211358");
            Assert.IsTrue(factura.Importe == "1700.00");
            Assert.IsTrue(factura.CuitDestino == "20251069116");
            Assert.IsTrue(factura.Fecha == DateTime.Parse("04/02/2019", culture));
            //Assert.IsTrue(factura.Detalle == "SERVICIOS CONTABLES DEL MES DE ENERO 2019");
            Console.WriteLine($"DETALLE FACTURA: {factura.Detalle}");
        }

        [TestMethod]
        public void Parse14()
        {
            var factura = new Infrastructure.Factura();
            factura.Parse(_facturas["14"]);
            Assert.IsTrue(factura.Numero == "0004-00019560");
            Assert.IsTrue(factura.Tipo == "B");
            Assert.IsTrue(factura.CuitOrigen == "30711719284");
            Assert.IsTrue(factura.Importe == "950.00");
            Assert.IsTrue(factura.CuitDestino == "11111111113");
            Assert.IsTrue(factura.Fecha == DateTime.Parse("21/01/2020", culture));
            //Assert.IsTrue(factura.Detalle == "LIBRO DE DATOS 2018");
            Console.WriteLine($"DETALLE FACTURA: {factura.Detalle}");
        }

        [TestMethod]
        public void Parse15()
        {
            var factura = new Infrastructure.Factura();
            factura.Parse(_facturas["15"]);
            Assert.IsTrue(factura.Numero == "0008-00121881");
            Assert.IsTrue(factura.Tipo == "B");
            Assert.IsTrue(factura.CuitOrigen == "30711738572");
            Assert.IsTrue(factura.Importe == "858.00");
            Assert.IsTrue(factura.CuitDestino == "11111111113");
            Assert.IsTrue(factura.Fecha == DateTime.Parse("03/09/2019", culture));
            //Assert.IsTrue(factura.Detalle == "+ Reg. Deudores");
            Console.WriteLine($"DETALLE FACTURA: {factura.Detalle}");
        }

        [TestMethod]
        public void Parse16()
        {
            var factura = new Infrastructure.Factura();
            factura.Parse(_facturas["16"]);
            Assert.IsTrue(factura.Numero == "0009-00048830");
            Assert.IsTrue(factura.Tipo == "B");
            Assert.IsTrue(factura.CuitOrigen == "33714975299");
            Assert.IsTrue(factura.Importe == "850.00");
            Assert.IsTrue(factura.CuitDestino == "30534418856");
            Assert.IsTrue(factura.Fecha == DateTime.Parse("07/01/2020", culture));
            //Assert.IsTrue(factura.Detalle == "");
            Console.WriteLine($"DETALLE FACTURA: {factura.Detalle}");
        }

        [TestMethod]
        public void Parse17()
        {
            var factura = new Infrastructure.Factura();
            factura.Parse(_facturas["17"]);
            Assert.IsTrue(factura.Numero == "0013-00111091"); 
            Assert.IsTrue(factura.Tipo == "B");
            Assert.IsTrue(factura.CuitOrigen == "30714990973");
            Assert.IsTrue(factura.Importe == "2540.57");
            Assert.IsTrue(factura.CuitDestino == "11111111113");
            Assert.IsTrue(factura.Fecha == DateTime.Parse("21/01/2020", culture));
            //Assert.IsTrue(factura.Detalle == "Procesamineto de Expensas");
            Console.WriteLine($"DETALLE FACTURA: {factura.Detalle}");
        }

        [TestMethod]
        public void Parse18()
        {
            var factura = new Infrastructure.Factura();
            factura.Parse(_facturas["18"]);
            Assert.IsTrue(factura.Numero == "0006-00455282");
            Assert.IsTrue(factura.Tipo == "B");
            Assert.IsTrue(factura.CuitOrigen == "30583701040");
            //Assert.IsTrue(factura.Importe == "2204.00");
            Assert.IsTrue(factura.CuitDestino == "30715149555");
            Assert.IsTrue(factura.Fecha == DateTime.Parse("21/11/2019", culture));
            //Assert.IsTrue(factura.Detalle == "1    POR SERVICIOS DE CONFECCION LIQUIDACION                                 1    POR SERVICIOS DE PROCESAMIENTO DE DATOS");
            Console.WriteLine($"DETALLE FACTURA: {factura.Detalle}");
        }

        [TestMethod]
        public void Parse19()
        {
            var factura = new Infrastructure.Factura();
            factura.Parse(_facturas["19"]);
            Assert.IsTrue(factura.Numero == "0006-00453229");
            Assert.IsTrue(factura.Tipo == "B");
            Assert.IsTrue(factura.CuitOrigen == "30583701040");
            //Assert.IsTrue(factura.Importe == "675.00");
            Assert.IsTrue(factura.CuitDestino == "30663850446");
            Assert.IsTrue(factura.Fecha == DateTime.Parse("13/11/2019", culture));
            //Assert.IsTrue(factura.Detalle == "1    POR SERVICIOS DE CONFECCION LIQUIDACION                                 1    POR SERVICIOS DE PROCESAMIENTO DE DATOS");
            Console.WriteLine($"DETALLE FACTURA: {factura.Detalle}");
        }

        [TestMethod]
        public void ParseNoFactura()
        {
            var factura = new Infrastructure.Factura();
            factura.Parse(_facturas["NO_FACTURA"]);
            Assert.IsTrue(factura.Numero == "");
            Assert.IsTrue(factura.Tipo == "");
            Assert.IsTrue(factura.CuitOrigen == "");
        }

        [TestMethod]
        public void Parse20()
        {
            var factura = new Infrastructure.Factura();

            var serverPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            var fileFullPath = serverPath + "\\FacturasEjemplo\\FAC20.pdf";
            byte[] bytes = File.ReadAllBytes(fileFullPath);
            string file = Convert.ToBase64String(bytes);

            factura.Parse(file);
            Assert.IsTrue(factura.Numero == "0003-00001698");
            Assert.IsTrue(factura.Tipo == "C");
            Assert.IsTrue(factura.CuitOrigen == "23243371244");
            //Assert.IsTrue(factura.Importe == "798.00");
            Assert.IsTrue(factura.CuitDestino == "30539109215");
            Assert.IsTrue(factura.Fecha == DateTime.Parse("13/05/2020", culture));
            //Assert.IsTrue(factura.Detalle == "Sistema de gestión online para  1 consorcios Administración Global - 2020-");
            Console.WriteLine($"DETALLE FACTURA: {factura.Detalle}");
        }
        [TestMethod]
        public void Parse21()
        {
            var factura = new Infrastructure.Factura();

            var serverPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            var fileFullPath = serverPath + "\\FacturasEjemplo\\Sipac\\786-0018.pdf";
            byte[] bytes = File.ReadAllBytes(fileFullPath);
            string file = Convert.ToBase64String(bytes);

            factura.Parse(file);
            Assert.IsTrue(factura.Numero == "0005-00119420");
            Assert.IsTrue(factura.Tipo == "B");
            Assert.IsTrue(factura.CuitOrigen == "30707831142");
            Assert.IsTrue(factura.Importe == "3623.00");
            Assert.IsTrue(factura.CuitDestino == "30604783352");
            Assert.IsTrue(factura.Fecha == DateTime.Parse("11/09/2019", culture));
//            Assert.IsTrue(factura.Detalle == "");
            Console.WriteLine($"DETALLE FACTURA: {factura.Detalle}");
        }
    }
}
