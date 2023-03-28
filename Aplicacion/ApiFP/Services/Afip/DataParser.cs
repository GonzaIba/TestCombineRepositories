using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

/// <summary>
/// Summary description for DataParser
/// </summary>
public class DataParser
{
    private Dictionary<string, string> _dicHttpRequest;
    private Dictionary<string, string>[] _arrDic;
    string _personData;

    public DataParser()
    {
        //
        // TODO: Add constructor logic here
        //
        _personData = "";
    }

    public void Parse(string data)
    {
        Dictionary<string, string> dic = new Dictionary<string, string>();

        String[] substrings = data.Split(',');
        foreach (string parameter in substrings)
        {
            string paramKey = parameter.Substring(0, parameter.LastIndexOf('='));
            string paramValue = parameter.Substring(parameter.LastIndexOf('=') + 1, parameter.Length - parameter.LastIndexOf('=') - 1);
            dic.Add(paramKey, paramValue);
        }
        this._dicHttpRequest = dic;

    }

    public void Parse(string data, int qty)
    {
        Dictionary<string, string> dic = new Dictionary<string, string>();
        int i = 1;
        int dicNumber = 0;
        this._arrDic = null;
        String[] substrings = data.Split(',');
        if (substrings.Length >= qty)
        {
            this._arrDic = new Dictionary<string, string>[substrings.Length / qty];
            foreach (string parameter in substrings)
            {
                if (!string.IsNullOrEmpty(parameter))
                {

                    string paramKey = parameter.Substring(0, parameter.LastIndexOf('='));
                    string paramValue = parameter.Substring(parameter.LastIndexOf('=') + 1, parameter.Length - parameter.LastIndexOf('=') - 1);
                    dic.Add(paramKey, paramValue);

                    if (i == qty)
                    {
                        i = 0;
                        this._arrDic[dicNumber] = dic;
                        dicNumber++;
                        dic = new Dictionary<string, string>();
                    }
                    i++;
                }
            }
        }
    }
    public string GetValue(string key)
    {
        if (this._dicHttpRequest.ContainsKey(key))
        {
            return this._dicHttpRequest[key];
        }
        else
        {
            return "";
        }
    }

    public object GetValue(string key, int qty)
    {
        if (this._arrDic != null && this._arrDic[qty] != null && this._arrDic[qty].ContainsKey(key) && !String.IsNullOrEmpty(this._arrDic[qty][key])) {     
            return this._arrDic[qty][key];
        }
        else
        {
            return null;
        }
    }

    public int GetQtyItems()
    {
        return (this._arrDic != null) ? this._arrDic.Length : 0;
    }
    private void TraverseNodes(XmlNodeList nodes, string name)
    {
        XmlNodeList a = nodes;
        foreach (XmlNode node in nodes)
        {
            if (node.HasChildNodes)
            {
                if (name != "" && name != "personaReturn")
                {
                    TraverseNodes(node.ChildNodes, name + "-" + node.Name);
                }
                else
                {
                    TraverseNodes(node.ChildNodes, node.Name);
                }
            }
            else
            {
                this._personData = this._personData + (name + "=" + node.Value + "|");
            }
        }
    }
    public string ParsePersonData(Object objXML)
    {
        return parseXML(objXML,"//personaReturn");
    }

    public string ParseComprobanteData(Object objXML)
    {
        return parseXML(objXML, "//ResultGet");
    }

    private string parseXML(Object objXML,string node)
    {
        StringWriter sw = new StringWriter();
        XmlSerializer serializador = new XmlSerializer(objXML.GetType());
        serializador.Serialize(sw, objXML);
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(sw.ToString());
        TraverseNodes(doc.SelectNodes(node), "");
        return this._personData.Substring(0, this._personData.Length - 1);
    }
    /*
    public string ParseComprobanteResponse(Wsfe.FECompConsultaResponse response)
    {
        string strResponse = "";
        string strError = "";
        string strObservaciones = "";

        if (response.ResultGet != null && response.ResultGet.Resultado != null)
        {
            strResponse = "resultadoCAE=" + response.ResultGet.Resultado;
        }

        if (response.ResultGet != null && response.ResultGet.CodAutorizacion != null)
        {
            strResponse = strResponse + ",numeroCAE=" + response.ResultGet.CodAutorizacion;
        }       

        if (response.Errors != null)
        {
            foreach (Wsfe.Err item in response.Errors)
            {
                strError = strError + "CodigoError=" + item.Code + ",MensajeError=" + item.Msg;
            }
        }

        //if (response.Events != null)
        //{
        //    foreach (Wsfe.Evt item in response.Events)
        //    {
        //        strObservaciones = strObservaciones + "CodigoObs=" + item.Code + ",MensajeObs=" + item.Msg;                
        //    }
        //}
        
        strResponse = strResponse + "|" + strError + "|" + strObservaciones;

        return strResponse;
    }

    public string ParseCrearComprobanteResponse(Wsfe.FECAEResponse response)
    {
        //"resultadoCAE=" & resultadoCAE & ",numeroCAE=" & numeroCAE
        string strResponse = "";
        string strError = "";
        string strObservaciones = "";

        try
        {

            if (response.FeCabResp != null && response.FeCabResp.Resultado != null)
            {
                strResponse = strResponse + "resultadoCAE=" + response.FeCabResp.Resultado;
            }
            if (response.FeDetResp != null && response.FeDetResp[0] != null)
            {
                strResponse = strResponse + ",numeroCAE=" + response.FeDetResp[0].CAE;
            }
            if (response.Errors != null)
            {
                foreach (Wsfe.Err item in response.Errors)
                {
                    strError = strError + "CodigoError=" + item.Code + ",MensajeError=" + item.Msg;
                }
            }
            if (response.FeDetResp != null && response.FeDetResp[0] != null && response.FeDetResp[0].Observaciones != null)
            {
                foreach (Wsfe.Obs item in response.FeDetResp[0].Observaciones)
                {
                    strObservaciones = strObservaciones + "CodigoObs=" + item.Code + ",MensajeObs=" + item.Msg;
                }
            }
        }
        catch(Exception ex)
        {
            strError = "CodigoError=001,MensajeError=Exception.Net:" + ex.Message;
        }

        return strResponse + "|" + strError + "|" + strObservaciones;
    }
    */
}