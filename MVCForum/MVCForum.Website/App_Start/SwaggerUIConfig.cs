namespace MvcForum.Web
{
    using Swashbuckle.Application;
    using System.Web.Http;
    public class SwaggerUIConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config
              .EnableSwagger(c => { 
                  c.SingleApiVersion("v1", "FutureNHS Web API"); 
              })
              .EnableSwaggerUi();
        }
    }
}