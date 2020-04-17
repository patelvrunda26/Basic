using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Basic.AuthorizationRequirements
{
    public class customrequireclaim : IAuthorizationRequirement
    {

        public customrequireclaim(string claimType)
        {
            ClaimType = claimType;
        }

        public string ClaimType { get; }
    }

    public class customrequireclaimhandler : AuthorizationHandler<customrequireclaim>
    {

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            customrequireclaim requirement)
        {
            var hasClaim = context.User.Claims.Any(x => x.Type == requirement.ClaimType);
            if (hasClaim)
            {
                context.Succeed(requirement);

            }
            return Task.CompletedTask;

        }
    }

    public static class authorizationpolicybuilderextensions
    {
        public static AuthorizationPolicyBuilder requirecustomclaim(
         this AuthorizationPolicyBuilder builder,
         string claimtype)
        {
            builder.AddRequirements(new customrequireclaim(claimtype));
            return builder;
        }
    }
}