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

        [Required, DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime FechaCreacion { get; set; }

        public EstadoFactura EstadoFactura { get; set; }
        [ForeignKey("EstadoFactura")]
        public int EstadoFacturaFK { get; set; }

        public int QtyDescargasCC { get; set; }

        public string DomicilioFiscal { get; set; }

        public void Insert()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            db.Facturas.Add(this);
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

        public bool ConfirmacionValida()
        {
            bool validacion = true;
            validacion = validacion && (!string.IsNullOrEmpty(this.Tipo));
            validacion = validacion && (!string.IsNullOrEmpty(this.Numero));
            validacion = validacion && (!string.IsNullOrEmpty(this.Importe));
            validacion = validacion && (!string.IsNullOrEmpty(this.CuitDestino));
            validacion = validacion && (!string.IsNullOrEmpty(this.CuitOrigen));
            validacion = validacion && (this.Fecha.HasValue) && (this.Fecha.Value != null);

            return validacion;
        }

        public void ReadDate(string Date)
        {
            if (!String.IsNullOrEmpty(Date)) { this.Fecha = DateTime.Parse(Date, new CultureInfo("es-ES", false)); };
        }

        public void Parse(string fileContent)
        {
            PDFParser pdfParser = new PDFParser();
            Business.DatosFactura datosFactura;
            try
            {
                datosFactura = pdfParser.extraerCamposDePDF(new MemoryStream(Convert.FromBase64String(fileContent)));

                this.CuitOrigen = datosFactura.Cuit_Origen;
                this.CuitDestino = datosFactura.Cuit_Destino;
                this.Detalle = datosFactura.Detalle;
                this.Tipo = datosFactura.Tipo;
                this.Numero = datosFactura.Numero;
                this.Importe = datosFactura.Importe;
                this.IvaDiscriminado = datosFactura.IvaDescriminado;
                this.Retenciones = datosFactura.Retenciones;
                this.Percepciones = datosFactura.Percepciones;
                this.ImpuestosNoGravados = datosFactura.ImpuestosNoGravados;
                this.ReadDate(datosFactura.Fecha);

                this.Update();
            }
            catch (Exception ex)
            {

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