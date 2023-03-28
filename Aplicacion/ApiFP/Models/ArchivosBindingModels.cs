using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ApiFP.Models
{
    public class Archivo
    {
        public string Nombre { get; set; }
        public string Extension { get; set; }
        public string ContenidoBase64 { get; set; }        
    }

}