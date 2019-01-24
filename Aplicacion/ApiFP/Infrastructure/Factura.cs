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
    public class Factura
    {
        [Key,Required]
        public int Id { get; set; }
        [MaxLength(50), Column(TypeName = "varchar")]
        public string Tipo { get; set; }
        [MaxLength(50), Column(TypeName = "varchar")]
        public string Numero { get; set; }
        [MaxLength(50), Column(TypeName = "varchar")]
        public string Importe { get; set; }
        [MaxLength(50), Column(TypeName = "varchar")]
        public string CuitOrigen { get; set; }
        [MaxLength(50), Column(TypeName = "varchar")]
        public string CuitDestino { get; set; }
        [Column(TypeName = "Date")]
        public Nullable<DateTime> Fecha { get; set; }
        [MaxLength(500), Column(TypeName = "varchar")]
        public string Detalle { get; set; }
        [MaxLength(50), Column(TypeName = "varchar")]
        public string Servicio { get; set; }
        [MaxLength(50), Column(TypeName = "varchar")]
        public string IvaDiscriminado { get; set; }
        [MaxLength(100), Column(TypeName = "varchar")]
        public string Retenciones { get; set; }
        [MaxLength(100), Column(TypeName = "varchar")]
        public string Percepciones { get; set; }
        [MaxLength(100), Column(TypeName = "varchar")]
        public string ImpuestosNoGravados { get; set; }

        public ApplicationUser ApplicationUser { get; set; }
        [ForeignKey("ApplicationUser")]
        public string UserIdFK { get; set; }

        [Column(TypeName = "bit"), Required]
        public bool? SinArchivo { get; set; }

        [Column(TypeName = "bit"), DefaultValue(false)]
        public bool Confirmada { get; set; }

        public void Insert()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            db.Facturas.Add(this);
            db.SaveChanges();
        }
    }

}