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
    public class StoreResult
    {
        public int Result;
        public string StorageType;
        public string Volume;
        public string FullPath;
    }
    public abstract class StorageService
    {
        public abstract StoreResult Store(string filename, string contentFile);
        public abstract string Restore(string volume, string fileFullPath);
        public abstract void Delete(string volume, string fileFullPath);
    }
}