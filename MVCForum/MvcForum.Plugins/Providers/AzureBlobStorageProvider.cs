using MvcForum.Core.Utilities;
using System.Configuration;

namespace MvcForum.Plugins.Providers
{
    using Azure.Storage.Blobs;
    using Azure.Storage.Blobs.Models;
    using Core;
    using Core.Interfaces.Providers;
    using System;
    using System.Collections.Generic;
    using System.IO;
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

        public string SaveAs(string uploadFolderPath, string fileName, Stream file)
        {
            InitialiseConnection();

            // Get a reference to a blob
            var blobServiceClient = _blobServiceClient.GetBlobContainerClient(_container);
            file.Position = 0;
            var blobClient = blobServiceClient.GetBlobClient(fileName);

            _ = blobClient.Upload(file, true);

            file.Dispose();

            return $"{blobServiceClient.Uri}/{fileName}";
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