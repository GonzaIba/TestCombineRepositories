using System;
using System.Collections.Generic;
using System.Web;

/// <summary>
/// Summary description for TokenData
/// </summary>
public class TokenData
{
    // Entero de 32 bits sin signo que identifica el requerimiento 
    public UInt32 UniqueId;
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
    // Ambiente sobre el cual generar el ticket - PROD - HOMO 
    public string Enviroment;

    public TokenData()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public bool validToken()
    {
        //Menor que cero -> t1 es anterior a t2.
        //Cero -> t1 es igual que t2.
        //Mayor que cero -> t1 es posterior a t2.
        bool response;
        response = (this.ExpirationTime != null);
        response = response && (!String.IsNullOrEmpty(this.Token));
        response = response && (!String.IsNullOrEmpty(this.Sign));
        response = response && (DateTime.Compare(DateTime.Now, this.ExpirationTime) < 0);

        return response;
    }
}