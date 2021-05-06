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
            var databaseOk = Database();
            if (databaseOk)
            {
                return Ok();
            }

            return InternalServerError();
        }
      
        private bool Database()
        {
            try
            {
                var settingsService = UnityHelper.Container.Resolve<ISettingsService>();
                var settings = settingsService.GetSettings(false);
                return true;
            }
            catch (Exception ex)
            {

            }
           
            return false;
        }
    }
}
