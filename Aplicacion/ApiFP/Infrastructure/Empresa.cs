using ApiFP.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace ApiFP.Infrastructure
{
    public class Empresa
    {
        [Key,Required]
        public int Id { get; set; }

        [MaxLength(50), Column(TypeName = "varchar"), Index("IX_Cuit", 1, IsUnique = true), Required]
        public string Cuit { get; set; }

        [MaxLength(200), Column(TypeName = "varchar")]
        public string Nombre { get; set; }

        [MaxLength(500), Column(TypeName = "varchar")]
        public string DomicilioComercial { get; set; }

        [Column(TypeName = "Date"), DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public Nullable<DateTime> FechaInsercions { get; set; }

        public void Insert()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            //db.Entry(this).State = this.Id == 0 ? EntityState.Added : EntityState.Modified;
            db.Empresas.Add(this);
            db.SaveChanges();
        }

        public void Update()
        {
            using (var db = new ApplicationDbContext())
            {
                db.Entry(this).State = this.Id == 0 ? EntityState.Added : EntityState.Modified;
                db.SaveChanges();
            }
        }

        private static int CalcularDigitoCuit(string cuit)
        {
            int[] mult = new[] { 5, 4, 3, 2, 7, 6, 5, 4, 3, 2 };
            char[] nums = cuit.ToCharArray();
            int total = 0;
            for (int i = 0; i<mult.Length; i++)
            {
                total += int.Parse(nums[i].ToString()) * mult[i];
            }
            int resto = total % 11;
            return resto == 0 ? 0 : resto == 1 ? 9 : 11 - resto;
        }

        public static bool ValidaCuit(string cuit)
        {        
            if (cuit == null)        
            {        
                return false;        
            }        
            //Quito los guiones, el cuit resultante debe tener 11 caracteres.        
            cuit = cuit.Replace("-", string.Empty);        
            if (cuit.Length != 11)        
            {        
                return false;        
            }        
            else        
            {        
                int calculado = CalcularDigitoCuit(cuit);        
                int digito = int.Parse(cuit.Substring(10));        
                return calculado == digito;        
            }        
        }
    }

}