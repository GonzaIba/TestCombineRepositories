using System;
using System.Configuration;
using System.IO;
using ApiFP.Helpers;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;


namespace ApiFP.Services
{
    public class CloudStorageService : StorageService
    {

        public StoreResult Store(string fileName, string fileContent)
        {

            string projectId = ConfigurationManager.AppSettings["GCS_PROJECT_ID"];
            string bucketName = ConfigurationManager.AppSettings["GCS_BUCKET_NAME"];

            string credentialFile = System.Web.Hosting.HostingEnvironment.MapPath("~");
            credentialFile = credentialFile + ConfigurationManager.AppSettings["GCS_CRED_FILE"];
            //string bucketName = projectId + "-test-bucket";
            //string bucketName = projectId + "-" + bucket;

            // Instantiates a client.
            var credential = GoogleCredential.FromFile(credentialFile);            
            StorageClient storageClient = StorageClient.Create(credential);
            StoreResult result = new StoreResult();
            try
            {
                var bytes = Convert.FromBase64String(fileContent);
                var fileStream = new MemoryStream(bytes);
                storageClient.UploadObject(bucketName, fileName, null, fileStream);

                result.Result = 0;
                result.Volume = bucketName;
                result.FullPath = projectId;
                result.StorageType = "GCS";
            }
            catch (Exception ex)            
            {                
                LogHelper.GenerateLog(ex);
                result.Result = 1;
                throw new Exception(ex.Message);
            }

            return result;
        }

        public string Restore(string storageType, string volume, string fileFullPath, string fileName)
        {
            string credentialFile = System.Web.Hosting.HostingEnvironment.MapPath("~");
            credentialFile = credentialFile + ConfigurationManager.AppSettings["GCS_CRED_FILE"];
            var credential = GoogleCredential.FromFile(credentialFile);
            StorageClient storageClient = StorageClient.Create(credential);
            
            using (var outputFile = new MemoryStream())
            {
                storageClient.DownloadObject(volume, fileName, outputFile);
                return Convert.ToBase64String(outputFile.ToArray());
            }

            return "";                        
        }

        public void Delete(string storageType, string volume, string fileFullPath)
        {
            //
        }
    }
}