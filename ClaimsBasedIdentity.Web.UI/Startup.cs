using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using ClaimsBasedIdentity.Data.POCO;
using ClaimsBasedIdentity.Web.UI.Identity;
using System.Data;
using ClaimsBasedIdentity.Data.Interfaces;
using ClaimsBasedIdentity.Data.Repository;

namespace ClaimsBasedIdentity.Web.UI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Required to use the Options<T> pattern
            services.AddOptions();
            services.Configure<AppSettingsConfiguration>(Configuration);

            // Initiliaze Data Repository
            services.AddHttpContextAccessor();                                      // Needed by IsAuthorized Custom Authorization Policy
            services.AddSingleton<IDBConnection, DBConnection>();
            services.AddSingleton<IAuthorizationHandler, IsAuthorizedHandler>();
            services.AddSingleton<IAuthorizationHandler, IsAdultHandler>();

            // Use Cookie Scheme for Authentication/Authorization
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,
                    options =>
                    {
                        options.Cookie.Name = "Identity.Cookie";
                        options.LoginPath = new PathString("/Account/Login/");
                        options.AccessDeniedPath = new PathString("/Account/Forbidden/");
                    }
                );

            services.AddAuthorization(config => {
                AuthorizationPolicyBuilder builder = new AuthorizationPolicyBuilder();
                AuthorizationPolicy defaultAuthorizationPolicy = builder
                    .RequireAuthenticatedUser()
                    .Build();
                config.DefaultPolicy = defaultAuthorizationPolicy;
                config.AddPolicy("IsAuthorized", policy =>                          // Customer policy
                    policy.Requirements.Add(new IsAuthorizedRequirement())
                );
                config.AddPolicy("ITDepartment", policy =>                          // Simple claims policy
                    policy.RequireClaim("Department", new string[] { "IT" })
                );
                config.AddPolicy("IsAdult", policy =>                          // Customer policy
                    policy.Requirements.Add(new IsAdultRequirement(21))
                );
            });

            services.AddControllersWithViews()
                .AddRazorRuntimeCompilation();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            // Who are users
            app.UseAuthentication();
            // What can users do
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
