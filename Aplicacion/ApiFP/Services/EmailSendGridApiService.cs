using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace ApiFP.Services
{
    public class EmailSendGridApiService
    {
        private string _urlService;
        private string _endpointSendMail;
        private string _contentType;
        public string _apikey;

        public EmailSendGridApiService()
        {
            _contentType = "application/json";
            _urlService = "https://api.sendgrid.com";
            _endpointSendMail = "/v3/mail/send";
            _apikey = ConfigurationManager.AppSettings["SENDGRID_APIKEY"];
        }

        public class Personalization
        {
            public List<Email> to;
            public string subject;
        }

        public class Email
        {
            public string email;
        }

        public class Content
        {
            public string type;
            public string value;
        }
        public class SendMailRequest {
            public List<Personalization> personalizations;
            public Email from;
            public List<Content> content;
        }

        #region "METHOD_SEND_MAIL"
        public void methodSendMail(SendMailRequest request)
        {
            //se arma request en json
            string dataIN = JsonConvert.SerializeObject(request);
            //se define url del metodo
            string urlServiceMethod = this._urlService + this._endpointSendMail;
            //se obtiene respuesta del servicio
            string dataOUT = CallService(dataIN, urlServiceMethod);
            //obtener Response
            //SendMailResponse response;
            //response = JsonConvert.DeserializeObject<SendMailResponse>(dataOUT);
            //return response;
        }
        #endregion

        private string CallService(string dataIN, string urlServiceMethod)
        {
            WebRequest request = HttpWebRequest.Create(urlServiceMethod);
            request.Method = "POST";
            request.ContentType = this._contentType;
            request.Headers.Add("Authorization", "Bearer " + this._apikey);

            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            System.Byte[] bytes = encoding.GetBytes(dataIN);

            request.ContentLength = bytes.Length;
            Stream requestStream;
            requestStream = request.GetRequestStream();
            using (requestStream)
            {
                requestStream.Write(bytes, 0, bytes.Length);
            }
            WebResponse response;
            string responseFromServer;

            try
            {
                response = request.GetResponse();
                var dataStream = response.GetResponseStream();
                StreamReader reader;
                reader = new StreamReader(dataStream);
                responseFromServer = reader.ReadToEnd();
                response.Close();
                dataStream.Close();
            }
            catch (WebException ex)
            {
                var readerError = new StreamReader(ex.Response.GetResponseStream());
                responseFromServer = readerError.ReadToEnd();
            }

            return responseFromServer;
        }
    }
}

