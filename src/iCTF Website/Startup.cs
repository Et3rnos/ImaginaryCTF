using iCTF_Shared_Resources;
using iCTF_Shared_Resources.Models;
using iCTF_Website.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iCTF_Website
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

            services.AddDbContext<DatabaseContext>(options => {
                options.UseMySql(Configuration.GetValue<string>("ConnectionString"),
                new MySqlServerVersion(new Version(5, 7)));
            });

            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddIdentity<ApplicationUser, IdentityRole>(options => {
                options.SignIn.RequireConfirmedAccount = true;
                options.User.RequireUniqueEmail = true;
                }
            )
            .AddEntityFrameworkStores<DatabaseContext>()
            .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = new PathString("/AccessDenied");
                options.LoginPath = new PathString("/Login");
                options.Cookie.Name = "SESSION";
            });
            services.AddAntiforgery(options =>
            {
                options.Cookie.Name = "XSRF-TOKEN";
            });

            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IRecaptchaService, RecaptchaService>();

            services.AddRazorPages();
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, DatabaseContext context, UserManager<ApplicationUser> userManager)
        {
            DatabaseInitializer.SeedUsers(context, userManager);
            //insecure_password_change_asap:AQAAAAEAACcQAAAAEO799gEVUKL13sgJGu3SXOr7OV9GQjF0jxI6I6fVda2qXF6Q0VQZ/668wOmNmSn7ig==

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error/500");
                app.UseHsts();
            }

            app.UseStatusCodePagesWithReExecute("/Error/{0}");

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
            });
        }
    }
}
