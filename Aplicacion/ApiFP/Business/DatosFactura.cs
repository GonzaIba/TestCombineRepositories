using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiFP.Business
{
    public class DatosFactura {
        public string Cuit_Origen { get; set; }
        public string Cuit_Destino { get; set; }
        public string Detalle { get; set; }
        public string Tipo { get; set; }
        public string Numero { get; set; }
        public string Importe { get; set; }
        public string IvaDescriminado { get; set; }
        public string Retenciones { get; set; }
        public string Percepciones { get; set; }
        public string ImpuestosNoGravados { get; set; }
        public string Fecha { get; set; }
        public string DomicilioComercial { get; set; }
    }
}
