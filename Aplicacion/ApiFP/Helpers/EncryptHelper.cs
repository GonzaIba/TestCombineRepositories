using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using EncryptionLibrary;

namespace ApiFP.Helpers
{
    public class EncryptHelper
    {        
        static public string Encript(string data)
        {
            var content = DateTime.Now.ToString() + "|" + data;
            return EncryptionLibrary.clsBase64.CodeToBase64(clsEncriptar.Encriptar(content, ConfigurationManager.AppSettings["ENCRYPT_KEY"]));            
        }

        static public string Decript(string data)
        {
            var content = clsEncriptar.Desencriptar(clsBase64.DecodeFromBase64(data), ConfigurationManager.AppSettings["ENCRYPT_KEY"]);
            return content.Substring(content.LastIndexOf("|") + 1);
        }
    }
}