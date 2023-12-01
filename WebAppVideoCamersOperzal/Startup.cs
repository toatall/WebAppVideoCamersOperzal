using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebAppVideoCamersOperzal.Models;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Authorization;

namespace WebAppVideoCamersOperzal
{
    public class Startup
    {
        private string connectionString;
        private IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            connectionString = configuration.GetConnectionString("Default");            
            _configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
                .AddNegotiate();
            services.Configure<AuthorizationOptions>(options =>
            {
                options.AddPolicy("Admin", policy =>
                {
                    string[] allowedRoles = _configuration.GetSection("AllowedRoles:Roles").Get<string[]>();
                    policy.RequireRole(allowedRoles);
                });
            });

            services.AddControllersWithViews();            
            services.AddSingleton(_configuration);
            services.AddDbContext<ApplicationContext>(options => options.UseSqlite(connectionString));
            services.AddMemoryCache();
            services.AddScoped<SaveVisitAttribute>();
            services.AddSingleton<AuthorizeService>();
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
            }
            app.UseStaticFiles();
            app.UseDefaultFiles();

            app.UseRouting();

            app.UseAuthentication();
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
