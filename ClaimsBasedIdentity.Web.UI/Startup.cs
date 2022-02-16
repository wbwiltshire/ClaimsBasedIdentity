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

            // Needed by IsAuthorized Custom Authorization Policy
            services.AddHttpContextAccessor();     
            
            // Initiliaze Data Repository
            services.AddSingleton<IDBConnection, DBConnection>();

            // Custom Authorization Policy Handlers
            services.AddSingleton<IAuthorizationHandler, IsAuthorizedHandler>();
            services.AddSingleton<IAuthorizationHandler, IsAdultHandler>();
            services.AddSingleton<IIdentityManager, IdentityManager>();

            // Use Cookie Scheme for Authentication/Authorization
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,
                    options =>
                    {
                        options.Cookie.Name = "ClaimsBasedIdentity.Cookie";
                        options.LoginPath = new PathString("/Account/LoginRegister");
                        options.AccessDeniedPath = new PathString("/Account/Forbidden/");
                        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                        
                        // Don't redirect unauthorized Webapi calls to login page
                        options.Events.OnRedirectToLogin = context =>
                        {
                            if (context.Request.Path.StartsWithSegments("/api")) {
                                context.Response.Clear();
                                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                                return Task.CompletedTask;
                            }
                            context.Response.Redirect(context.RedirectUri);
                            return Task.CompletedTask;
                        };
                    }
                )
                .AddCookie("Webapi", options => {
                    // Need this so unauthorized Webapi call throws a 401
                    //options.LoginPath = PathString.Empty;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.Events.OnRedirectToAccessDenied = context =>
                    {
                        context.Response.Clear();
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return Task.CompletedTask;
                    };
                });

            services.AddAuthorization(config => {
                AuthorizationPolicyBuilder builder = new AuthorizationPolicyBuilder();
                AuthorizationPolicy defaultAuthorizationPolicy = builder
                    .RequireAuthenticatedUser()                                     // Require users to be authenticated by default
                    .Build();
                config.DefaultPolicy = defaultAuthorizationPolicy;
                config.AddPolicy("IsAuthorized", policy =>                          // Customer policy
                    policy.Requirements.Add(new IsAuthorizedRequirement())
                );
                config.AddPolicy("ITDepartment", policy =>                          // Simple claims policy
                    policy.RequireClaim("Department", new string[] { "IT" })
                );
                config.AddPolicy("IsAdult", policy =>                               // Customer policy
                    policy.Requirements.Add(new IsAdultRequirement(21))
                );
            });

            // Add Swagger service
            services.AddSwaggerGen();

            // Add MVC and Razore runtime compilation support
            services.AddControllersWithViews()
                .AddRazorRuntimeCompilation();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                // Add Swagger middleware to application (localhost:<port>//swagger/index.html
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // Include standard middleware
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
