namespace MvcForum.Web.Controllers.ApiControllers
{
    using MvcForum.Core.Interfaces.Services;
    using MvcForum.Core.Models.Entities;
    using MvcForum.Core.Models.Enums;
    using System;
    using System.Threading.Tasks;
    using System.Web.Http;

    /// <summary>
    /// Defines the AutomationController to expose api endpoint to aid automation testing.
    /// </summary>
    [RoutePrefix("api/automation")]
    [Authorize(Roles = "Admin")]
    public class AutomationController : ApiController
    {
        /// <summary>
        /// Gets or sets teh _membershipService.
        /// </summary>
        private IMembershipService _membershipService { get; set; }

        /// <summary>
        /// Gets or sets the _logger.
        /// </summary>
        private ILoggingService  _logger { get; set; }

        /// <summary>
        /// Constructs a new instance of the AutomationController.
        /// </summary>
        /// <param name="membershipService"></param>
        /// <param name="logger"></param>
        public AutomationController(IMembershipService membershipService, ILoggingService logger)
        {
            _membershipService = membershipService;
            _logger = logger;
        }

        /// <summary>
        /// Method to generate a defined amount of users in the system.
        /// </summary>
        /// <param name="numberOfUsers">Amount of users to create.</param>
        /// <returns>List of MembershipUsers created. <see cref="MembershipUser"/></returns>
        [Route("CreateUsers")]
        [HttpPost]
        public async Task<IHttpActionResult> CreateUsers(int numberOfUsers)
        {
            for (int i=0; i < numberOfUsers; i++)
            {
                MembershipUser user = new MembershipUser 
                { 
                    UserName = String.Format("PF{0}", i+1),
                    Email=String.Format("PF{0}@test.com", i+1),
                    Password = "Password"
                };
                try
                {
                    var result = await _membershipService.CreateUser(user, LoginType.Standard);
                    
                    if (!result.Successful)
                    {
                        throw new Exception(String.Format("Failed to create user PF{0}.", i+1));
                    }
                    
                } catch(Exception ex)
                {
                    _logger.Error(String.Format("Failed to create user PF{0}. {1}.", i+1, ex));
                    return InternalServerError();
                }
            }

            return Ok();
        }
    }
}
