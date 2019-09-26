using System;
using System.Collections.Generic;
using System.Web;

/// <summary>
/// Summary description for Token
/// </summary>
public class TokenHandler
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

    private TokenData _tokenData;

    public TokenHandler(TokenData tokenData)
    {
        this._tokenData = tokenData;
    }

    public void GetToken()
    {
        //obtener desde base
        GetTokenFromDB();
        //validar token de db
        if (!this._tokenData.validToken())
        {
            //obtener desde servicio
            GetTokenFromService();
            // guardar en DB
            clsDB.InsertNewToken(this._tokenData);
        }        
    }

    private void GetTokenFromService()
    {
        LoginTicketService objLogticket = new LoginTicketService(this._tokenData);
        objLogticket.ObtenerTokenFromService();
    }

    private void GetTokenFromDB()
    {
        clsDB.GetToken(ref this._tokenData);
    }
}