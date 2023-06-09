//<summary> 
//EJEMPLO - Consola cliente del webservice de autenticacion y autorizacion 
//</summary> 
//<description> 
// Consume el metodo LoginCms del WSAA 
// Muestra en stdout el login ticket response 
//</description> 
//<version>8.5.26.1200</version> 
//<usage> 
// clientelogincms (opciones) ... 
// opciones: 
// -s servicio ID del servicio de negocio 
// -w url URL del WSDL del WSAA 
// -c certificado Ruta del certificado (con clave privada) 
// -v on|off Salida detallada on/off 
// -? Muestra ayuda de uso 
//</usage> 
//<platform>.NET Framework 2.0</platform> 
//<disclaimer> 
// El Departamento de Arquitectura Informatica de la AFIP (DeArIn/AFIP), pone a disposicion 
// el siguiente codigo para su utilizacion con el WebService de Facturacion Electronica (WSFE) 
// de la AFIP. 
// 
// El mismo no puede ser re-distribuido, publicado o descargado en forma total o parcial, ya sea 
// en forma electronica, mecanica u optica, sin la autorizacion de DeArIn/AFIP. El uso no 
// autorizado del mismo esta prohibido. 
// 
// DeArIn/AFIP no asume ninguna responsabilidad de los errores que pueda contener el codigo ni la 
// obligacion de subsanar dichos errores o informar de la existencia de los mismos. 
// 
// DeArIn/AFIP no asume ninguna responsabilidad que surja de la utilizacion del codigo, ya sea por 
// utilizacion ilegal de patentes, perdida de beneficios, perdida de informacion o cualquier otro 
// inconveniente. 
// 
// Bajo ninguna circunstancia DeArIn/AFIP podra ser indicada como responsable por consecuencias y/o 
// incidentes ya sean directos o indirectos que puedan surgir de la utilizacion del codigo. 
// 
// DeSoTe/AFIP no da ninguna garantia, expresa o implicita, de la utilidad del codigo, si el mismo es 
// correcto, o si cumple con los requerimientos de algun proposito en particular. 
// 
// DeArIn/AFIP puede realizar cambios en cualquier momento en el codigo sin previo aviso. 
// 
// El codigo debera ser evaluado, verificado, corregido y/o adaptado por personal tecnico calificado 
// de las entidades que lo utilicen. 
// 
// EL SIGUIENTE CODIGO ES DISTRIBUIDO PARA EVALUACION, CON TODOS SUS ERRORES Y OMISIONES. LA 
// RESPONSABILIDAD DEL CORRECTO FUNCIONAMIENTO DEL MISMO YA SEA POR SI SOLO O COMO PARTE DE 
// OTRA APLICACION, QUEDA A CARGO DE LAS ENTIDADES QUE LO UTILICEN. LA UTILIZACION DEL CODIGO 
// SIGNIFICA LA ACEPTACION DE TODOS LOS TERMINOS Y CONDICIONES MENCIONADAS ANTERIORMENTE. 
// 
// DeArIn-AFIP 
//</disclaimer> 

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Net;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;

/// <summary>
/// Clase para crear objetos Login Tickets
/// </summary>
/// <remarks> 
/// Ver documentacion:
/// Especificacion Tecnica del Webservice de Autenticacion y Autorizacion
/// Version 1.0
/// Departamento de Seguridad Informatica - AFIP
/// </remarks>
public class LoginTicketService
{
    private TokenData _tokenData;

    public XmlDocument XmlLoginTicketRequest = null;
    public XmlDocument XmlLoginTicketResponse = null;
    public string RutaDelCertificadoFirmante;
    public string XmlStrLoginTicketRequestTemplate = "<loginTicketRequest><header><uniqueId></uniqueId><generationTime></generationTime><expirationTime></expirationTime></header><service></service></loginTicketRequest>";

    public LoginTicketService(TokenData tokenData)
    {
        this._tokenData = tokenData;
    }
    /// <summary> 
    /// Construye un Login Ticket obtenido del WSAA 
    /// </summary> 
    /// <param name="argServicio">Servicio al que se desea acceder</param> 
    /// <param name="argUrlWsaa">URL del WSAA</param> 
    /// <param name="argRutaCertX509Firmante">Ruta del certificado X509 (con clave privada) usado para firmar</param> 
    /// <param name="argVerbose">Nivel detallado de descripcion? true/false</param> 
    /// <remarks></remarks>
    public string ObtenerTokenFromService()
    {

        string cmsFirmadoBase64;
        string loginTicketResponse;

        XmlNode xmlNodoUniqueId;
        XmlNode xmlNodoGenerationTime;
        XmlNode xmlNodoExpirationTime;
        XmlNode xmlNodoService;

        // PASO 1: Genero el Login Ticket Request 
        try
        {
            XmlLoginTicketRequest = new XmlDocument();
            XmlLoginTicketRequest.LoadXml(XmlStrLoginTicketRequestTemplate);

            xmlNodoUniqueId = XmlLoginTicketRequest.SelectSingleNode("//uniqueId");
            xmlNodoGenerationTime = XmlLoginTicketRequest.SelectSingleNode("//generationTime");
            xmlNodoExpirationTime = XmlLoginTicketRequest.SelectSingleNode("//expirationTime");
            xmlNodoService = XmlLoginTicketRequest.SelectSingleNode("//service");

            xmlNodoGenerationTime.InnerText = DateTime.Now.AddMinutes(-10).ToString("s");
            xmlNodoExpirationTime.InnerText = DateTime.Now.AddHours(+12).ToString("s");
            xmlNodoUniqueId.InnerText = Convert.ToString(1);
            xmlNodoService.InnerText = this._tokenData.Service;            
        }

        catch (Exception excepcionAlGenerarLoginTicketRequest)
        {
            throw new Exception("***Error GENERANDO el LoginTicketRequest : " + excepcionAlGenerarLoginTicketRequest.Message);
        }

        // PASO 2: Firmo el Login Ticket Request 
        try
        {
            if (this._tokenData.Enviroment == "PROD")
            {
                RutaDelCertificadoFirmante = ConfigurationManager.AppSettings["CERTIFICATE_PROD_PATH"];
            }
            else
            {
                RutaDelCertificadoFirmante = ConfigurationManager.AppSettings["CERTIFICATE_HOMO_PATH"];
            }

            RutaDelCertificadoFirmante = System.Web.Hosting.HostingEnvironment.MapPath("~") + RutaDelCertificadoFirmante;

            X509Certificate2 certFirmante = CertificadosX509Lib.ObtieneCertificadoDesdeArchivo(RutaDelCertificadoFirmante);
            // Convierto el login ticket request a bytes, para firmar 
            Encoding EncodedMsg = Encoding.UTF8;
            byte[] msgBytes = EncodedMsg.GetBytes(XmlLoginTicketRequest.OuterXml);

            // Firmo el msg y paso a Base64 
            byte[] encodedSignedCms = CertificadosX509Lib.FirmaBytesMensaje(msgBytes, certFirmante);
            cmsFirmadoBase64 = Convert.ToBase64String(encodedSignedCms);
        }

        catch (Exception excepcionAlFirmar)
        {
            throw new Exception("***Error FIRMANDO el LoginTicketRequest : " + excepcionAlFirmar.Message);
        }

        // PASO 3: Invoco al WSAA para obtener el Login Ticket Response 
        try
        {
            ApiFP.Wsaa.LoginCMSService servicioWsaa = new ApiFP.Wsaa.LoginCMSService();
           
            if (this._tokenData.Enviroment == "PROD")
            {
                servicioWsaa.Url = ConfigurationManager.AppSettings["WSAA_PROD_URL"];
            }
            else
            {
                servicioWsaa.Url = ConfigurationManager.AppSettings["WSAA_HOMO_URL"];
            }
            loginTicketResponse = servicioWsaa.loginCms(cmsFirmadoBase64);
        }

        catch (Exception excepcionAlInvocarWsaa)
        {
            throw new Exception("***Error INVOCANDO al servicio WSAA : " + excepcionAlInvocarWsaa.Message);
        }


        // PASO 4: Analizo el Login Ticket Response recibido del WSAA 
        try
        {
            XmlLoginTicketResponse = new XmlDocument();
            XmlLoginTicketResponse.LoadXml(loginTicketResponse);

            this._tokenData.UniqueId = UInt32.Parse(XmlLoginTicketResponse.SelectSingleNode("//uniqueId").InnerText);
            this._tokenData.GenerationTime = DateTime.Parse(XmlLoginTicketResponse.SelectSingleNode("//generationTime").InnerText);
            this._tokenData.ExpirationTime = DateTime.Parse(XmlLoginTicketResponse.SelectSingleNode("//expirationTime").InnerText);
            this._tokenData.Sign = XmlLoginTicketResponse.SelectSingleNode("//sign").InnerText;
            this._tokenData.Token = XmlLoginTicketResponse.SelectSingleNode("//token").InnerText;
        }
        catch (Exception excepcionAlAnalizarLoginTicketResponse)
        {
            throw new Exception("***Error ANALIZANDO el LoginTicketResponse : " + excepcionAlAnalizarLoginTicketResponse.Message);
        }

        return loginTicketResponse;
    }
}
