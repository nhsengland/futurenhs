using System;
using System.Web.Http;
using MvcForum.Core.Interfaces.Services;
using MvcForum.Core.Ioc;
using Unity;

namespace MvcForum.Web.Controllers
{
    [RoutePrefix("api/HealthCheck")]
    public partial class HealthCheckController : ApiController
    {
        [Route("HeartBeat")]
        [HttpGet]
        public IHttpActionResult HeartBeat()
        {
            return Ok();
        }
    }
}
