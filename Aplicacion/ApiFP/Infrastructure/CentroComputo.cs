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
    public class CentroComputo
    {
        [Key,Required]
        public int Id { get; set; }
        [MaxLength(50), Column(TypeName = "varchar")]
        public string Nombre { get; set; }
        [MaxLength(32), Column(TypeName = "varchar")]
        public string ApiKey { get; set; }
        [MaxLength(150), Column(TypeName = "varchar")]
        public string Contacto { get; set; }
        [MaxLength(150), Column(TypeName = "varchar")]
        public string Email { get; set; }
    }
}