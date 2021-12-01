using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MvcForum.Core.Interfaces.Providers;
using MvcForum.Core.Interfaces.Services;
using MvcForum.Core.Models.Collabora;
using Newtonsoft.Json;

namespace MvcForum.Core.Services
{
    public sealed class FileServerService : IFileServerService
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly string _fileUrl;
        public FileServerService(IConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
            _fileUrl = configurationProvider.FileServerTemplateUrl;
        }

        public async Task<FileServerResponse> GetCollaboraFileUrl(Guid file, CookieContainer cookies)
        {
            var fileRequestUrl = _fileUrl.Replace(_configurationProvider.FileServerTemplateUrlFileIdPlaceholder, file.ToString());
            var request = (HttpWebRequest)WebRequest.Create(fileRequestUrl);

            var postData = "permission=" + Uri.EscapeDataString("view");


            var data = Encoding.ASCII.GetBytes(postData);

            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;
            request.Timeout = 3000;
            request.Method = "POST";
            request.CookieContainer = cookies;

            // If required by the server, set the credentials.
            request.Credentials = CredentialCache.DefaultCredentials;


            using (var stream = request.GetRequestStream())
            {
                await stream.WriteAsync(data, 0, data.Length);
            }

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (var dataStream = response.GetResponseStream())
                    {
                        // Open the stream using a StreamReader for easy access.
                        using (var reader = new StreamReader(dataStream ?? throw new InvalidOperationException()))
                        {
                            // Read the content.
                            var responseFromServer = await reader.ReadToEndAsync();
                            var fileServerResponse = JsonConvert.DeserializeObject<FileServerResponse>(responseFromServer);
                            return fileServerResponse;
                        }
                    }
                }
            }

            return null;
        }

       
        }
    }


