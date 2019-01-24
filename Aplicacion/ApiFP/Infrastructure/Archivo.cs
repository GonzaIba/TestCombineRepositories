using ApiFP.Services;
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
    public class Archivo
    {
        [Key,Required]
        public int Id { get; set; }
        [MaxLength(100), Column(TypeName = "varchar")]
        public string Nombre { get; set; }
        [MaxLength(10), Column(TypeName = "varchar")]
        public string Extension { get; set; }
        [MaxLength(50), Column(TypeName = "varchar")]
        public string TipoAlmacenamiento { get; set; }
        [MaxLength(50), Column(TypeName = "varchar")]
        public string Volumen { get; set; }
        [MaxLength(200), Column(TypeName = "varchar")]
        public string Ruta { get; set; }
       
        public Factura Factura { get; set; }
        [ForeignKey("Factura")]
        public int FacturaIdFK { get; set; }

        public void Insert()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            db.Archivos.Add(this);
            db.SaveChanges();
        }

        public string CreateStorageName()
        {
            return FacturaIdFK.ToString() + "_" + Nombre + Extension;
        }

        public void Delete()
        {
            StorageService storageService = new StorageService();
            storageService.Delete(this.TipoAlmacenamiento, this.Volumen, this.Ruta);
        }
    }

}