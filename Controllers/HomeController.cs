using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Basic.CustomPolicyProvider;

namespace Basic.Controllers
{
    public class HomeController : Controller
    {

        //Dostuff()

        //private readonly IAuthorizationService _authorizationservice;

        //public HomeController(IAuthorizationService authorizationService)
        //{
        //    _authorizationservice = authorizationService;
        //}

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]   //question
        public IActionResult Secret()
        {
            return View();
        }

        [Authorize(Policy = "claim.dob")]
        public IActionResult SecretPolicy()
        {
            return View("Secret");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult SecretRole()
        {
            return View("Secret");
        }


        [SecurityLevel(5)]
        public IActionResult SecretLevel()
        {
            return View("Secret");
        }

        [SecurityLevel(10)]
        public IActionResult SecretHigherLevel()
        {
            return View("Secret");
        }


        [AllowAnonymous]
        public IActionResult authenticate()
        {

            //building a ID

            var grandmaClaims = new List<Claim>()
            {
            new Claim(ClaimTypes.Name,"Vrunda"),
            new Claim(ClaimTypes.Email,"patelvrunda26@gmail.com"),
            new Claim(ClaimTypes.DateOfBirth,"05/26/1998"),
            new Claim(ClaimTypes.Role,"Admin"),
            new Claim(dynamicPolicies.Securitylevel,"7"),
            new Claim("Grandma.says","beautiful"),
            };

            var licenseclaims = new List<Claim>()
            {
            new Claim(ClaimTypes.Name,"My license"),
            new Claim("Drivingtype", "A+"),
            };


            var grandmaIdentity = new ClaimsIdentity(grandmaClaims, "Grandma Claims");
            var licenseIdentity = new ClaimsIdentity(licenseclaims, "Government");


            var userprincipal = new ClaimsPrincipal(new[] { grandmaIdentity, licenseIdentity });




            HttpContext.SignInAsync(userprincipal);


            return RedirectToAction("Index");
        }

        public async Task<IActionResult> dostuff(
            [FromServices] IAuthorizationService authorizationService)
        {
            //let you do something

            var builder = new AuthorizationPolicyBuilder("Schema");
            var custompolicy = builder.RequireClaim("Hello").Build();
            var authresult = await authorizationService.AuthorizeAsync(HttpContext.User, "claim.dob");

            if (authresult.Succeeded)
            {
                return View("Index");
            }
            return View("Index");
        }
    }
}