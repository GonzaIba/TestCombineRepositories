using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ApiFP.Models
{
    public class CreateFacturaBindingModel
    {

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
        [Required]
        public Archivo Archivo { get; set; }

    }

}