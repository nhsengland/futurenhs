using System.ComponentModel.DataAnnotations;
using FutureNHS.Api.Application.Application.HardCodedSettings;
using FutureNHS.Api.DataAccess.Database.Read.Interfaces;
using FutureNHS.Api.Models.Pagination.Filter;
using FutureNHS.Api.Models.Pagination.Helpers;
using FutureNHS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FutureNHS.Api.Controllers
{
    [Authorize]
    [Route("api/v{version:apiVersion}")]
    [ApiController]
    [ApiVersion("1.0")]
    //[FeatureGate(FeatureFlags.Groups)]
    public sealed class SearchController : ControllerIdentityBase
    {
        private readonly ILogger<SearchController> _logger;
        private readonly ISearchDataProvider _searchDataProvider;

        public SearchController(ILogger<ControllerIdentityBase> baseLogger, IUserService userService, ILogger<SearchController> logger, ISearchDataProvider searchDataProvider) : base(baseLogger, userService)
        {
            _logger = logger;
            _searchDataProvider = searchDataProvider;
        }

        [HttpGet]
        [Route("search")]
        public async Task<IActionResult> Search([FromQuery, MinLength(SearchSettings.TermMinimum), MaxLength(SearchSettings.TermMaximum)] string term, [FromQuery] PaginationFilter filter, CancellationToken cancellationToken)
        {
            var route = Request.Path.Value;
            var identity = await GetUserIdentityAsync(cancellationToken);

            var (totalCount, results) = await _searchDataProvider.Search(identity.MembershipUserId, term, filter.Offset,filter.Limit, cancellationToken);

            var pagedResponse = PaginationHelper.CreatePagedResponse(results, filter, totalCount, route);

            return Ok(pagedResponse);
        }
    }
}