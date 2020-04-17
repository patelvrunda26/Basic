using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;
using Basic.AuthorizationRequirements;
using Microsoft.AspNetCore.Mvc.Authorization;
using Basic.Controllers;
using Basic.Transformer;
using Microsoft.AspNetCore.Authentication;
using Basic.CustomPolicyProvider;

namespace Basic
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication("CookieAuth")
                .AddCookie("CookieAuth", config =>
                {
                    config.Cookie.Name = "grandmas.cookie";
                    config.LoginPath = "/Home/Authenticate";
                });

            services.AddAuthorization(config =>
            {
                //var defaultauthbuilder = new AuthorizationPolicyBuilder();
                //var defaultpolicybuilder = defaultauthbuilder
                //.RequireAuthenticatedUser()
                //.RequireClaim(ClaimTypes.DateOfBirth)
                //.Build();

                //config.DefaultPolicy = defaultpolicybuilder;

                //config.AddPolicy("Claim.DoB", policyBuilder =>
                // {
                //     policyBuilder.RequireClaim(ClaimTypes.DateOfBirth)
                // });
                config.AddPolicy("Admin", policybuilder => policybuilder.RequireClaim(ClaimTypes.Role, "Admin"));

                config.AddPolicy("claim.dob", policybuilder =>
                {
                    policybuilder.requirecustomclaim(ClaimTypes.DateOfBirth);
                });

            });

            services.AddSingleton<IAuthorizationPolicyProvider, CustomAuthorizationPolicyProvider>();

            services.AddScoped<IAuthorizationHandler, SecurityLevelHandler>();

            services.AddScoped<IAuthorizationHandler, customrequireclaimhandler>();

            services.AddScoped<IAuthorizationHandler, CookieJarAurthorizationHandler>();

            services.AddScoped<IClaimsTransformation, ClaimsTransformation>();

            services.AddControllersWithViews(config =>
            {
                var defaultauthbuilder = new AuthorizationPolicyBuilder();
                var defaultauthpolicy = defaultauthbuilder
                .RequireAuthenticatedUser()
                .Build();

                //Global authorization filter
                //config.Filters.Add(new AuthorizeFilter(defaultauthpolicy));
            });

            services.AddRazorPages()
                .AddRazorPagesOptions(config =>
                {
                    config.Conventions.AuthorizePage("/Razor/Secured");
                    //suppose to be failed to check it
                    config.Conventions.AuthorizePage("/Razor/Policy", "Admintwo");

                    config.Conventions.AuthorizeFolder("/RazorSecured");
                    //allow other people to access the secured page without having any claims
                    config.Conventions.AllowAnonymousToPage("/RazorSecured/anon");
                });
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }


            app.UseRouting();


            //who are you?
            app.UseAuthentication();

            //are you allowed?
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapRazorPages();
            });
        }
    }
}
