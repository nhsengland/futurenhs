using System.Configuration;
using MvcForum.Core.Utilities;

namespace MvcForum.Plugins.Providers
{
    using Azure.Storage.Blobs;
    using Core;
    using Core.Interfaces.Providers;
    using System.Collections.Generic;
    using System.IO;
    using System.Web;
    using System.Web.Hosting;


    public class AzureBlobStorageProvider : IStorageProvider
    {
        private BlobServiceClient _blobServiceClient;
        private string _endpoint;
        private string _container;


        public string BuildFileUrl(params object[] subPath)
        {
            var file = (string)subPath[2];

            InitialiseConnection();

            var blobClient = _blobServiceClient.GetBlobContainerClient(_container);

            return $"{blobClient.Uri}/{file}{(string)subPath[3]}";
        }


        public string GetUploadFolderPath(bool createIfNotExist, params object[] subFolders)
        {
            var sf = new List<object>();
            sf.AddRange(subFolders);

            var folder =
                HostingEnvironment.MapPath(
                    string.Concat(ForumConfiguration.Instance.UploadFolderPath, string.Join("\\", sf)));

            if (createIfNotExist && !Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            return folder;
        }

        public string SaveAs(string uploadFolderPath, string fileName, HttpPostedFileBase file)
        {
            InitialiseConnection();

            // Get a reference to a blob
            var blobClient = _blobServiceClient.GetBlobContainerClient(_container);

            var response = blobClient.UploadBlob(fileName, file.InputStream);

            return $"{blobClient.Uri}/{fileName}";
        }

        private void InitialiseConnection()
        {
            if (_endpoint.IsNullEmpty())
            {
                _endpoint = ConfigurationManager.ConnectionStrings["AzureBlobStorage:PrimaryConnectionString"].ConnectionString;
                _container = ConfigurationManager.AppSettings["BlobContainer"];
                _blobServiceClient = new BlobServiceClient(_endpoint);
            }
           
        }
    }
}