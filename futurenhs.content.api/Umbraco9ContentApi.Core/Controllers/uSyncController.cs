namespace Umbraco9ContentApi.Core.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using Umbraco.Cms.Web.Common.Controllers;
    using Umbraco9ContentApi.Core.Handlers.uSync.Interface;

    /// <summary>
    /// uSync Api controller to access uSync handlers.
    /// </summary>
    /// <seealso cref="UmbracoApiController" />
    [Route("api/usync")]
    public sealed class uSyncController : UmbracoApiController
    {
        private readonly IuSyncHandler _uSyncHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="uSyncController"/> class.
        /// </summary>
        /// <param name="uSyncHandler">The uSync handler.</param>
        public uSyncController(IuSyncHandler uSyncHandler)
        {
            _uSyncHandler = uSyncHandler;
        }

        /// <summary>
        /// Imports all usync items.
        /// </summary>
        /// <param name="isActive">if set to <c>true</c> [import].</param>
        /// <remarks></remarks>
        /// <returns>True or false.</returns>
        [HttpPost("{isActive:bool}/import")]
        public async Task<ActionResult> DoUSyncImportAsync(bool isActive)
        {
            bool result = false;

            if (isActive)
            {
                result = _uSyncHandler.RunImportAsync().Result;
            }

            // import end
            return Ok(result);
        }

    }
}