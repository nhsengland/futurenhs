using FutureNHS.Api.Configuration;
using FutureNHS.Api.DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System.Net;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;

namespace FutureNHS.Api.Attributes
{
    public class ETagFilter : ActionFilterAttribute, IAsyncActionFilter
    {
        private readonly IOptions<AzureImageBlobStorageConfiguration> _options;

        public ETagFilter(IOptions<AzureImageBlobStorageConfiguration> options)
        {
            _options = options;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext executingContext, ActionExecutionDelegate next)
        {
            var request = executingContext.HttpContext.Request;
            var executedContext = await next();

            if (executedContext.Result is NotFoundResult) return;

            var response = executedContext.HttpContext.Response;
            if (request.Method == HttpMethod.Get.Method && response.StatusCode == (int)HttpStatusCode.OK)
            {
                ValidateEntityTag(executedContext);
            }
        }

        private void ValidateEntityTag(ActionExecutedContext executedContext)
        {
            if (executedContext.Result == null)
            {
                return;
            }

            var request = executedContext.HttpContext.Request;
            var response = executedContext.HttpContext.Response;
            var result = (executedContext.Result as ObjectResult).Value;
            string etag = string.Empty;
            if (result.GetType().GetProperty("RowVersion") is not null)
            {
              
                etag = Convert.ToBase64String(((BaseData)result).RowVersion);
            }
            else
            {
                etag = Convert.ToBase64String(Encoding.Default.GetBytes(ComputeHash(result)));
            }

            if (request.Headers.ContainsKey(HeaderNames.IfNoneMatch))
            {
                var incomingEtag = request.Headers[HeaderNames.IfNoneMatch].ToString();

                if (incomingEtag.Equals(etag))
                {
                    executedContext.Result = new StatusCodeResult((int)HttpStatusCode.NotModified);
                }
            }

            response.Headers.Add(HeaderNames.ETag, etag);
        }

        private static string ComputeHash(object instance)
        {
            using (MD5 hash = MD5.Create())
            {
                var serializer = new DataContractSerializer(instance.GetType());

                using (var memoryStream = new MemoryStream())
                {
                    serializer.WriteObject(memoryStream, instance);
                    hash.ComputeHash(memoryStream.ToArray());

                    return String.Join("", hash.Hash.Select(c => c.ToString("x2")));
                }
            }
        }
    }
}