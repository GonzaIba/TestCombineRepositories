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
    public class DescargaFactura
    {
        [Key,Required]
        public int Id { get; set; }        

        public CentroComputo CentroComputo { get; set; }
        [ForeignKey("CentroComputo")]
        public int CentroComputoIdFK { get; set; }

        public Factura Factura { get; set; }
        [ForeignKey("Factura")]
        public int FacturaIdFK { get; set; }

        [Required, DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime Fecha { get; set; }
    }
}