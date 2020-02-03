using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Collections.Generic;

namespace ApiFP.Tests
{
    [TestClass]
    public partial class PDFParserTests
    {
   
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
        public void Parse12()
        {
            var factura = new Infrastructure.Factura();
            factura.Parse(_facturas["12"]);
            Assert.IsTrue(factura.Numero == "00003-00000112");
            Assert.IsTrue(factura.Tipo == "A");
            Assert.IsTrue(factura.CuitOrigen == "20225898082");
            Assert.IsTrue(factura.Importe == "18293.51");
            Assert.IsTrue(factura.CuitDestino == "20329565980");
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
        }
    }
}
