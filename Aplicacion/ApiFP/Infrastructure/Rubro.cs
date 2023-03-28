using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace ApiFP.Infrastructure
{
    public class Rubro
    {
        [Key,Required]
        public int Id { get; set; }
        [MaxLength(50), Column(TypeName="varchar"), Index("IX_Rubro", 1, IsUnique = true)]
        public string Tipo { get; set; }
        [MaxLength(50), Column(TypeName = "varchar"), Index("IX_Rubro", 2, IsUnique = true)]
        public string Nombre { get; set; }
    }
}