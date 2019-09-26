using System;
using System.Collections.Generic;
using System.Web;

/// <summary>
/// Summary description for LoginTicketDB
/// </summary>
public class LoginTicketDB
{
    // Momento en que fue generado el requerimiento 
    public DateTime GenerationTime;
    // Momento en el que exoira la solicitud 
    public DateTime ExpirationTime;
    // Identificacion del WSN para el cual se solicita el TA 
    public string Service;
    // Firma de seguridad recibida en la respuesta 
    public string Sign;
    // Token de seguridad recibido en la respuesta 
    public string Token;
    public LoginTicketDB()
    {
        //
        // TODO: Add constructor logic here
        //
    }
}