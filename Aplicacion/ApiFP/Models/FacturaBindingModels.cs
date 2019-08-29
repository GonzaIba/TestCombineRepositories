using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ApiFP.Infrastructure;

namespace ApiFP.Models
{
    public class MinimalFacturaBindingModel
    {
        public string Tipo { get; set; }
        public string Numero { get; set; }
        public string CuitOrigen { get; set; }
        public string CuitDestino { get; set; }
        public string Fecha { get; set; }
    }
    public class BaseFacturaBindingModel : MinimalFacturaBindingModel
    {
        public string Importe { get; set; }
        public string Detalle { get; set; }
        public string Servicio { get; set; }
        public string IvaDiscriminado { get; set; }
        public string Retenciones { get; set; }
        public string Percepciones { get; set; }
        public string ImpuestosNoGravados { get; set; }
        [Required]
        public bool? SinArchivo { get; set; }
        public string DomicilioComercial { get; set; }
    }
    public class CreateFacturaBindingModel : BaseFacturaBindingModel
    {
        public Archivo Archivo { get; set; }

        public bool ValidateNoFile()
        {
            bool result = (this.SinArchivo == false) && (this.Archivo != null);
            result = result || (this.SinArchivo == true) && (this.Archivo == null);

            return result;
        }

        public bool ValidateMandatory()
        {
            bool result = Factura.ValidaCuit(this.CuitOrigen)
                        && Factura.ValidaCuit(this.CuitDestino);
            return result;
        }

        public bool validateDuplicate()
        {

            return true;
        }
    }

    public class GetFacturaBindingModel : BaseFacturaBindingModel
    {
        public int Id { get; set; }
        public Nullable<int> ArchivoId { get; set; }
        public bool Confirmada { get; set; }
        public string EstadoFactura { get; set; }
        public string FechaSubida { get; set; }

    }

    public class UpdateFacturaBindingModel : BaseFacturaBindingModel
    {
        public int Id { get; set; }
    }

    public class GetFacturaCCBindingModel : MinimalFacturaBindingModel
    {
        public int Id { get; set; }
    }

    public class GetFacturaCCDBindingModel : BaseFacturaBindingModel
    {
        public int Id { get; set; }
        public Archivo Archivo { get; set; }
        public string UrlArchivo { get; set; }
        public bool Confirmada { get; set; }
    }

    public class GetCuitsBindingModel{
        public List<string> CuitOrigen;
        public List<string> CuitDestino;
    }
}
