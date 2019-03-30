using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiFP.Services
{
    public class StorageServiceFactory
    {
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