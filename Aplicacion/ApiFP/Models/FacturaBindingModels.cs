﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ApiFP.Models
{
    public class BaseFacturaBindingModel
    {
        public string Tipo { get; set; }
        public string Numero { get; set; }
        public string Importe { get; set; }
        public string CuitOrigen { get; set; }
        public string CuitDestino { get; set; }
        public string Fecha { get; set; }
        public string Detalle { get; set; }
        public string Servicio { get; set; }
        public string IvaDiscriminado { get; set; }
        public string Retenciones { get; set; }
        public string Percepciones { get; set; }
        public string ImpuestosNoGravados { get; set; }
        [Required]
        public bool? SinArchivo { get; set; }
    }
    public class CreateFacturaBindingModel : BaseFacturaBindingModel
    {        
        public Archivo Archivo { get; set; }

        public bool ValidarSinArchivo()
        {
            bool result =  (this.SinArchivo == false) && (this.Archivo != null);
            result = result || (this.SinArchivo == true) && (this.Archivo == null);

            return result;
        }
    }

    public class GetFacturaBindingModel : BaseFacturaBindingModel
    {
        public int Id { get; set; }
        public Nullable<int> ArchivoId { get; set; }
        public bool Confirmada { get; set; }
    }

    public class UpdateFacturaBindingModel : BaseFacturaBindingModel
    {
        public int Id { get; set; }
    }
}