using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace ApiFP.Services
{
    public class StorageServiceFactory
    {
        public static StorageService GetDefault()
        {
            return Get(ConfigurationManager.AppSettings["DEFAULT_STORAGE_SERVICE_TYPE"]);
        }

        /// <summary>
        /// Decides which class to instantiate.
        /// </summary>
        public static StorageService Get(string type)
        {
            StorageService ss = null;
            switch (type)
            {
                case "FSS":
                    ss = new FileSystemStorageService();
                    break;
                case "GCS":
                    ss = new CloudStorageService();
                    break;
            }
            return ss;
        }
    }
}