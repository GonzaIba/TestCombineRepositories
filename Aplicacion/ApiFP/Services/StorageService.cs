using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace ApiFP.Services
{
    public class StorageService
    {
        public class StoreResult
        {
            public int Result;
            public string StorageType;
            public string Volume;
            public string FullPath;
        }
        public StoreResult Store(string fileName, string fileContent)
        {

            string volume = ConfigurationManager.AppSettings["FSS_CURRENT_VOLUME"];
            string pathType = ConfigurationManager.AppSettings["FSS_CURRENT_PATH_TYPE"];
            string volumePath = ConfigurationManager.AppSettings["FSS_VOLUME_" + volume + "_" + pathType];
            string fileFullPath;
            /*
            if (pathType == "ABSOLUTE")
            {
                volumePath = ConfigurationManager.AppSettings["FSS_VOLUME_" + volume + "_" + pathType];
                
            }else 
            */
            if (pathType == "RELATIVE")
            {
                volumePath = System.Web.Hosting.HostingEnvironment.MapPath("~");
                volumePath = volumePath + ConfigurationManager.AppSettings["FSS_VOLUME_" + volume + "_" + pathType];
            }

            fileFullPath = volumePath + fileName;

            StoreResult result = new StoreResult();
            try
            {
                File.WriteAllBytes(fileFullPath, Convert.FromBase64String(fileContent));
               
                result.Result = 0;
                result.Volume = volume;
                result.FullPath = fileFullPath;
                result.StorageType = "FSS";
            }
            catch (Exception ex)
            {
                result.Result = 1;
                throw new Exception(ex.Message);
            }

            return result;
        }

        public string Restore(string storageType, string volume, string fileFullPath)
        {                            
            byte[] bytes = File.ReadAllBytes(fileFullPath);
            string file = Convert.ToBase64String(bytes);

            return file;                        
        }
    }
}