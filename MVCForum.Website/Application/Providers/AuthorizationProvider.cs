﻿namespace MvcForum.Web.Application.Providers
{
    using Microsoft.Owin.Security;
    using Microsoft.Owin.Security.OAuth;
    using MvcForum.Core.Interfaces.Services;
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;

    internal class AuthorizationProvider : OAuthAuthorizationServerProvider
    {
        /// <summary>
        /// Gets or setsteh _membershipService.
        /// </summary>
        private IMembershipService _membershipService { get; set; }

        /// <summary>
        /// Constructs a new instance of the AuthorizationProvider.
        /// </summary>
        /// <param name="membershipService">Instance of the membershipService <see cref="IMembershipService"/></param>
        public AuthorizationProvider(IMembershipService membershipService)
        {
            _membershipService = membershipService;
        }

        /// <inheritdoc/>
        public override Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            return Task.Factory.StartNew(() =>
            {
                var username = context.UserName;
                var password = context.Password;

                if (_membershipService.ValidateUser(username, password, 3))
                {
                    var user = _membershipService.GetUser(username);

                    var claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.Sid, Convert.ToString(user.Id)),
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(ClaimTypes.Email, user.Email)
                    };

                    string roleString = String.Empty;

                    foreach (var role in user.Roles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role.RoleName));
                        roleString += role.RoleName + ",";
                    }



                    var data = new Dictionary<string, string>
                    {
                        { "userName", user.UserName },
                        { "roles", roleString}
                    };
                    var properties = new AuthenticationProperties(data);

                    ClaimsIdentity oAuthIdentity = new ClaimsIdentity(claims, "Password");

                    var ticket = new AuthenticationTicket(oAuthIdentity, properties);
                    context.Validated(ticket);
                }
                else
                {
                    context.SetError("invalid_grant", "Either email or password is incorrect");
                }
            });
        }

        /// <inheritdoc/>
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            if (context.ClientId == null)
                context.Validated();

            return Task.FromResult<object>(null);
        }

        /// <inheritdoc/>
        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }
            return Task.FromResult<object>(null);
        }
    }
}