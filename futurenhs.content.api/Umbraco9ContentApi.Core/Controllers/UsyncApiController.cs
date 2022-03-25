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
    public sealed class uSyncApiController : UmbracoApiController
    {
        private readonly IuSyncHandler _uSyncHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="uSyncApiController"/> class.
        /// </summary>
        /// <param name="uSyncHandler">The uSync handler.</param>
        public uSyncApiController(IuSyncHandler uSyncHandler)
        {
            _uSyncHandler = uSyncHandler;
        }

        /// <summary>
        /// Imports all usync items.
        /// </summary>
        /// <param name="import">if set to <c>true</c> [import].</param>
        /// <remarks></remarks>
        /// <response code="200">Success! Import successful.</response>
        /// <response code="500">Error. Import unsuccessful.</response>
        /// <returns>True or false.</returns>
        [HttpPost("{import:bool}/import")]
        public async Task<ActionResult> DoUSyncImportAsync(bool import)
        {
            bool result = false;

            if (import)
            {
                result = _uSyncHandler.RunImportAsync().Result;
            }

            // import end
            return Ok(result);
        }

    }
}