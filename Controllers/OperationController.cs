using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Basic.Controllers
{
    public class OperationController : Controller
    {
        private readonly IAuthorizationService _authorizationservice;

        public OperationController(IAuthorizationService authorizationService)
        {

            _authorizationservice = authorizationService;
        }

        public async Task<IActionResult> Open()
        {
            var cookiejar = new CookieJar(); //get cookie jar from database

            await _authorizationservice.AuthorizeAsync(User, cookiejar, CookieJarAuthOperations.Open);
            return View();
        }
    }

    public class CookieJarAurthorizationHandler
        : AuthorizationHandler<OperationAuthorizationRequirement, CookieJar>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            OperationAuthorizationRequirement requirement,
            CookieJar cookiejar)
        {
            if (requirement.Name == CookieJarOperations.Look)
            {
                if (context.User.Identity.IsAuthenticated)
                { context.Succeed(requirement); }
            }
            else if (requirement.Name == CookieJarOperations.ComeNear)
            {
                if (context.User.HasClaim("friend", "good"))
                {
                    context.Succeed(requirement);
                }
            }
            return Task.CompletedTask;
        }
    }

    public static class CookieJarAuthOperations
    {
        public static OperationAuthorizationRequirement Open = new OperationAuthorizationRequirement
        {
            Name = CookieJarOperations.Open
        };
    }

    public static class CookieJarOperations
    {
        public static string Open = "open";
        public static string TakeCookie = "TakeCookie";
        public static string ComeNear = "ComeNear";
        public static string Look = "Look";
    }


    public class CookieJar
    {
        public string name { get; set; }
    }


}