using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace ApiFP.Infrastructure
{
    public class EstadoFactura
    {
        [Key,Required]
        public int Id { get; set; }
        [MaxLength(50), Column(TypeName = "varchar")]
        public string Nombre { get; set; }        
    }

}