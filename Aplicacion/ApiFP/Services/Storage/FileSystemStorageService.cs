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
    public class FileSystemStorageService : StorageService
    {
        public override StoreResult Store(string filename, string contentFile)
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

            fileFullPath = volumePath + filename;

            StoreResult result = new StoreResult();
            try
            {
                File.WriteAllBytes(fileFullPath, Convert.FromBase64String(contentFile));
               
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

        public override string Restore(string volume, string fileFullPath)
        {                            
            byte[] bytes = File.ReadAllBytes(fileFullPath);
            string file = Convert.ToBase64String(bytes);

            return file;                        
        }

        public override void Delete(string volume, string fileFullPath)
        {
            File.Delete(fileFullPath);
        }
    }
}