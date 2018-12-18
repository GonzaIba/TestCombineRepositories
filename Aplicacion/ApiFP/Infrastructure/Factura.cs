using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace ApiFP.Infrastructure
{
    public class Factura
    {
        [Key,Required]
        public int Id { get; set; }
        public string Tipo { get; set; }
        public string Numero { get; set; }
        public string Importe { get; set; }
        public string CuitOrigen { get; set; }
        public string CuitDestino { get; set; }
        public string Detalle { get; set; }
        public string Servicio { get; set; }
        public string IvaDiscriminado { get; set; }
        public string Retenciones { get; set; }
        public string Percepciones { get; set; }
        public string ImpuestosNoGravados { get; set; }

    }
}