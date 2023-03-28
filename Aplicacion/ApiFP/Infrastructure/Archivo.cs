using ApiFP.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
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

        public string EncryptId()
        {
            string param = DateTime.Now.ToString() + "|" + this.Id.ToString();
            var encParam = EncryptionLibrary.clsEncriptar.Encriptar(param, ConfigurationManager.AppSettings["ENCRYPTION_KEY_URL_FILE"]);
            encParam = EncryptionLibrary.clsBase64.CodeToBase64(encParam);
            return encParam;
        }

        public string DecryptId(string encParam)
        {
            var decrypParam = EncryptionLibrary.clsBase64.DecodeFromBase64(encParam);
            decrypParam = EncryptionLibrary.clsEncriptar.Desencriptar(decrypParam, ConfigurationManager.AppSettings["ENCRYPTION_KEY_URL_FILE"]);
            string[] param = decrypParam.Split('|');

            return param[1];
        }


        public string CreateStorageName()
        {
            return FacturaIdFK.ToString() + "_" + Nombre + Extension;
        }

        public void Delete()
        {
            StorageService storageService = StorageServiceFactory.Get(this.TipoAlmacenamiento);
            storageService.Delete(this.Volumen, this.Ruta);
        }

        public bool Store(Models.Archivo file)
        {
            StorageService storageService = StorageServiceFactory.GetDefault();
            StoreResult storeResult = storageService.Store(this.CreateStorageName(), file.ContenidoBase64);

            if (storeResult.Result == 0)
            {
                this.Ruta = storeResult.FullPath;
                this.TipoAlmacenamiento = storeResult.StorageType;
                this.Volumen = storeResult.Volume;
                return true;
            }
            return false;
        }

        public string Restore()
        {
            StorageService storageService = StorageServiceFactory.Get(this.TipoAlmacenamiento);
            return storageService.Restore(this.Volumen, this.Ruta);
        }
    }

}