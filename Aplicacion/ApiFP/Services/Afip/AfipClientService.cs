using System;
using System.Configuration;
using ApiFP.Wsci;

public class AfipClientService
{
    public AfipClientService()
    {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    public class GetLoginTokenRequest
    {
        public string ServiceToUse;
        public string Enviroment;
    }

    public class GetLoginTokenResponse
    {
        public string Result;
        public string Message;
        public TokenData Token;
    }

    public GetLoginTokenResponse GetToken(GetLoginTokenRequest getLoginTokenRequest)
    {
        GetLoginTokenResponse getTokenResponse = new GetLoginTokenResponse();
        try
        {
            TokenData tokenData = new TokenData();
            tokenData.Enviroment = getLoginTokenRequest.Enviroment;
            tokenData.Service = getLoginTokenRequest.ServiceToUse;

            TokenHandler tokenHandler = new TokenHandler(tokenData);
            tokenHandler.GetToken();
            getTokenResponse.Token = tokenData;
            getTokenResponse.Result = "0";
            getTokenResponse.Message = "";
        }
        catch (Exception e)
        {
            getTokenResponse.Result = "1";
            getTokenResponse.Message = e.Message;
        }
        return getTokenResponse;
    }

    public ApiFP.Wsci.personaReturn ConsultaInscripcion(string IdPersona, string Ambiente)
    {
        string result = "";
        //ejemplo de homologacion IdPersona=27255820422
        GetLoginTokenRequest getTokenRequest = new GetLoginTokenRequest();
        getTokenRequest.ServiceToUse = "ws_sr_constancia_inscripcion";
        getTokenRequest.Enviroment = Ambiente;
        GetLoginTokenResponse getTokenResponse = this.GetToken(getTokenRequest);
        ApiFP.Wsci.personaReturn response = new ApiFP.Wsci.personaReturn();

        ApiFP.Wsci.PersonaServiceA5 service = new ApiFP.Wsci.PersonaServiceA5();

        service.Url = ConfigurationManager.AppSettings["WSCI_" + Ambiente + "_URL"];
        service.Timeout = GetWsTimeout();
        response = service.getPersona(getTokenResponse.Token.Token, getTokenResponse.Token.Sign, Int64.Parse(ConfigurationManager.AppSettings["CLAMASA_CUIT"]), Int64.Parse(IdPersona));
        return response;
    }
    
    public ApiFP.Wsci.dummyReturn ConsultaInscripcionDummy(string Ambiente)
    {
        ApiFP.Wsci.PersonaServiceA5 service = new ApiFP.Wsci.PersonaServiceA5();
        service.Timeout = GetWsTimeout();
        service.Url = ConfigurationManager.AppSettings["WSCI_" + Ambiente + "_URL"];

        ApiFP.Wsci.dummyReturn response = new ApiFP.Wsci.dummyReturn();
        response = service.dummy();
        return response;
    }

    #region "FE"
    /*
    public class ConsultarUltimoNumeroComprobanteRequest
    {
        public string ComprobanteTipo;
        public string PuntoVenta;
        public string Ambiente;
    }
    public class ConsultarUltimoNumeroComprobanteResponse
    {
        public string NumeroUltimoComprobante;
        public string Error;
        public string Obs;
    }

    public ConsultarUltimoNumeroComprobanteResponse ConsultarUltimo(ConsultarUltimoNumeroComprobanteRequest consultarUltimoNumeroComprobanteRequest)
    {
        //ejemplo de homologacion IdPersona=27255820422
        GetLoginTokenRequest getTokenRequest = new GetLoginTokenRequest();
        getTokenRequest.ServiceToUse = "wsfe";
        getTokenRequest.Enviroment = consultarUltimoNumeroComprobanteRequest.Ambiente;
        GetLoginTokenResponse getTokenResponse = this.GetToken(getTokenRequest);

        Wsfe.Service service = new Wsfe.Service();
        service.Timeout = GetWsTimeout();
        service.Url = ConfigurationManager.AppSettings["WSFE_" + consultarUltimoNumeroComprobanteRequest.Ambiente + "_URL"];
        Wsfe.FEAuthRequest requestAuth = new Wsfe.FEAuthRequest();
        requestAuth.Token = getTokenResponse.Token.Token;
        requestAuth.Sign = getTokenResponse.Token.Sign;
        requestAuth.Cuit = Int64.Parse(ConfigurationManager.AppSettings["CLAMASA_CUIT"]);
        int ptoVenta = (int)Int64.Parse(consultarUltimoNumeroComprobanteRequest.PuntoVenta);
        int cbteTipo = (int)Int32.Parse(consultarUltimoNumeroComprobanteRequest.ComprobanteTipo);
        Wsfe.FERecuperaLastCbteResponse response = new Wsfe.FERecuperaLastCbteResponse();
        response = service.FECompUltimoAutorizado(requestAuth, ptoVenta, cbteTipo);

        ConsultarUltimoNumeroComprobanteResponse consultarUltimoResponse = new ConsultarUltimoNumeroComprobanteResponse();

        consultarUltimoResponse.NumeroUltimoComprobante = response.CbteNro.ToString();

        return consultarUltimoResponse;
    }

    public class ConsultarComprobanteRequest
    {
        public string ComprobanteTipo;
        public string ComprobanteNumero;
        public string PuntoVenta;
        public string Ambiente;
    }

    public Wsfe.FECompConsultaResponse ConsultarFE(ConsultarComprobanteRequest consultarComprobanteRequest)
    {
        //Se obtiene token de seguridad
        GetLoginTokenRequest getTokenRequest = new GetLoginTokenRequest();
        getTokenRequest.ServiceToUse = "wsfe";
        getTokenRequest.Enviroment = consultarComprobanteRequest.Ambiente;
        GetLoginTokenResponse getTokenResponse = this.GetToken(getTokenRequest);

        //Se crean objetos del request para el metodo ConsultarComprobante
        Wsfe.FEAuthRequest requestAuth = new Wsfe.FEAuthRequest();
        requestAuth.Token = getTokenResponse.Token.Token;
        requestAuth.Sign = getTokenResponse.Token.Sign;
        requestAuth.Cuit = Int64.Parse(ConfigurationManager.AppSettings["CLAMASA_CUIT"]);
        
        Wsfe.FECompConsultaReq request = new Wsfe.FECompConsultaReq();
        request.CbteNro = (int)Int64.Parse(consultarComprobanteRequest.ComprobanteNumero);
        request.CbteTipo = (int)Int32.Parse(consultarComprobanteRequest.ComprobanteTipo);
        request.PtoVta = (int)Int64.Parse(consultarComprobanteRequest.PuntoVenta);

        //Se crea el servicio y se consume metodo ConsultarComprobante
        Wsfe.Service service = new Wsfe.Service();
        service.Timeout = GetWsTimeout();
        Wsfe.FECompConsultaResponse response = new Wsfe.FECompConsultaResponse();
        service.Url = ConfigurationManager.AppSettings["WSFE_" + consultarComprobanteRequest.Ambiente + "_URL"];
        response = service.FECompConsultar(requestAuth, request);

        return response;
    }

    public Wsfe.DummyResponse DummyFE(string Ambiente)
    {
        Wsfe.Service service = new Wsfe.Service();
        service.Timeout = GetWsTimeout();
        service.Url = ConfigurationManager.AppSettings["WSFE_" + Ambiente + "_URL"];

        Wsfe.DummyResponse response = new Wsfe.DummyResponse();
        response = service.FEDummy();
        return response;
    }

    public class CrearFERequest
    {
        public string DatosFeDetReq;
        public string DatosFeCabReq;
        public string DatosCbteAsoc;
        public string DatosTributo;
        public string DatosAlicIva;
        public string DatosOpcional;
        public string Ambiente;
    }

    public Wsfe.FECAEResponse CrearFE(CrearFERequest crearFERequest)
    {
        //Se obtiene token de seguridad
        GetLoginTokenRequest getTokenRequest = new GetLoginTokenRequest();
        getTokenRequest.ServiceToUse = "wsfe";
        getTokenRequest.Enviroment = crearFERequest.Ambiente;
        GetLoginTokenResponse getTokenResponse = this.GetToken(getTokenRequest);

        //Se crean objetos del request para el metodo ConsultarComprobante
        Wsfe.FEAuthRequest requestAuth = new Wsfe.FEAuthRequest();
        requestAuth.Token = getTokenResponse.Token.Token;
        requestAuth.Sign = getTokenResponse.Token.Sign;
        requestAuth.Cuit = Int64.Parse(ConfigurationManager.AppSettings["CLAMASA_CUIT"]);

        DataParser objParser = new DataParser();
        Wsfe.FECAERequest request = new Wsfe.FECAERequest();
        //DatosFeCabReq
        objParser.Parse(crearFERequest.DatosFeCabReq);
        request.FeCabReq = new Wsfe.FECAECabRequest();
        request.FeCabReq.CantReg = (int)Int64.Parse(objParser.GetValue("CantReg"));
        request.FeCabReq.CbteTipo = (int)Int64.Parse(objParser.GetValue("CbteTipo")); ;
        request.FeCabReq.PtoVta = (int)Int64.Parse(objParser.GetValue("PtoVta")); ;
        Wsfe.FECAEDetRequest detalle = new Wsfe.FECAEDetRequest();
        //DatosFeDetReq
        objParser.Parse(crearFERequest.DatosFeDetReq);
        detalle.CbteDesde = Int64.Parse(objParser.GetValue("CbteDesde"));
        detalle.CbteFch = objParser.GetValue("CbteFch");
        detalle.CbteHasta = Int64.Parse(objParser.GetValue("CbteHasta"));
        detalle.Concepto = (int)Int64.Parse(objParser.GetValue("Concepto"));
        detalle.DocNro = Int64.Parse(objParser.GetValue("DocNro"));
        detalle.DocTipo = (int)Int64.Parse(objParser.GetValue("DocTipo"));
        detalle.FchServDesde = objParser.GetValue("FchServDesde");
        detalle.FchServHasta = objParser.GetValue("FchServHasta");
        detalle.FchVtoPago = objParser.GetValue("FchVtoPago"); 
        detalle.ImpIVA = Double.Parse(objParser.GetValue("ImpIVA"));
        detalle.ImpNeto = Double.Parse(objParser.GetValue("ImpNeto"));
        detalle.ImpOpEx = Double.Parse(objParser.GetValue("ImpOpEx"));
        detalle.ImpTotal = Double.Parse(objParser.GetValue("ImpTotal"));
        detalle.ImpTotConc = Double.Parse(objParser.GetValue("ImpTotConc"));
        detalle.ImpTrib = Double.Parse(objParser.GetValue("ImpTrib"));        
        detalle.MonCotiz = Double.Parse(objParser.GetValue("MonCotiz"));
        detalle.MonId = objParser.GetValue("MonId");
        //DatosCbteAsoc         
        objParser.Parse(crearFERequest.DatosCbteAsoc,3);
        if (objParser.GetQtyItems() > 0)
        {
            detalle.CbtesAsoc = new Wsfe.CbteAsoc[objParser.GetQtyItems()];
            for (int i = 0; i < objParser.GetQtyItems(); i++)
            {
                Wsfe.CbteAsoc cbteAsoc = new Wsfe.CbteAsoc();
                cbteAsoc.Nro = (objParser.GetValue("Nro", i) == null) ? cbteAsoc.Nro : (int)Int64.Parse(objParser.GetValue("Nro", i).ToString());
                cbteAsoc.PtoVta = (objParser.GetValue("PtoVta", i) == null) ? cbteAsoc.PtoVta : (int)Int64.Parse(objParser.GetValue("PtoVta", i).ToString());
                cbteAsoc.Tipo = (objParser.GetValue("Tipo", i) == null) ? cbteAsoc.Tipo : (int)Int64.Parse(objParser.GetValue("Tipo", i).ToString());
                detalle.CbtesAsoc[i] = cbteAsoc;
            }
        }
        //DatosTributo        
        objParser.Parse(crearFERequest.DatosTributo,5);
        if (objParser.GetQtyItems() > 0)
        {
            detalle.Tributos = new Wsfe.Tributo[objParser.GetQtyItems()];
            for (int i = 0; i < objParser.GetQtyItems(); i++)
            {
                Wsfe.Tributo tributo = new Wsfe.Tributo();
                tributo.Alic = (objParser.GetValue("Alic", i) == null) ? tributo.Alic : Double.Parse(objParser.GetValue("Alic", i).ToString());
                tributo.BaseImp = (objParser.GetValue("BaseImp", i) == null) ? tributo.BaseImp : Double.Parse(objParser.GetValue("BaseImp", i).ToString());
                tributo.Desc = (objParser.GetValue("Desc", i) == null) ? tributo.Desc : objParser.GetValue("Desc", i).ToString();
                tributo.Id = (objParser.GetValue("Id", i) == null) ? tributo.Id : (short)Int64.Parse(objParser.GetValue("Id", i).ToString());
                tributo.Importe = (objParser.GetValue("Importe", i) == null) ? tributo.Importe : Double.Parse(objParser.GetValue("Importe", i).ToString());
                detalle.Tributos[i] = tributo;
            }
        }
        //DatosAlicIva
        objParser.Parse(crearFERequest.DatosAlicIva, 3);
        if (objParser.GetQtyItems() > 0)
        {
            detalle.Iva = new Wsfe.AlicIva[objParser.GetQtyItems()];
            for (int i = 0; i < objParser.GetQtyItems(); i++)
            {
                Wsfe.AlicIva iva = new Wsfe.AlicIva();
                iva.Id = (objParser.GetValue("Id", i) == null) ? iva.Id : (int)Int64.Parse(objParser.GetValue("Id", i).ToString());
                iva.Importe = (objParser.GetValue("Importe", i) == null) ? iva.Importe : Double.Parse(objParser.GetValue("Importe", i).ToString());
                iva.BaseImp = (objParser.GetValue("BaseImp", i) == null) ? iva.BaseImp : Double.Parse(objParser.GetValue("BaseImp", i).ToString());
                detalle.Iva[i] = iva;
            }
        }
        //DatosOpcional
        objParser.Parse(crearFERequest.DatosOpcional,2);
        if (objParser.GetQtyItems() > 0)
        {
            detalle.Opcionales = new Wsfe.Opcional[objParser.GetQtyItems()];
            for (int i = 0; i < objParser.GetQtyItems(); i++)
            {
                Wsfe.Opcional opcional = new Wsfe.Opcional();
                opcional.Id = (objParser.GetValue("Id", i) == null) ? opcional.Id : objParser.GetValue("Id", i).ToString();
                opcional.Valor = (objParser.GetValue("Valor", i) == null) ? opcional.Valor : objParser.GetValue("Valor", i).ToString();
                detalle.Opcionales[i] = opcional;
            }
        }
        request.FeDetReq = new Wsfe.FECAEDetRequest[1];
        request.FeDetReq[0] = detalle;

        //Se crea el servicio y se consume metodo ConsultarComprobante
        Wsfe.Service service = new Wsfe.Service();
        service.Timeout = GetWsTimeout();
        Wsfe.FECAEResponse response = new Wsfe.FECAEResponse();
        service.Url = ConfigurationManager.AppSettings["WSFE_" + crearFERequest.Ambiente + "_URL"];
        response = service.FECAESolicitar(requestAuth, request);

        return response;
    }
    */
    # endregion
    private int GetWsTimeout()
    {
        return (String.IsNullOrEmpty(ConfigurationManager.AppSettings["WS_AFIP_CLIENT_TIMEOUT"])) ? 60000 : Convert.ToInt32(ConfigurationManager.AppSettings["WS_AFIP_CLIENT_TIMEOUT"]);
    }

}