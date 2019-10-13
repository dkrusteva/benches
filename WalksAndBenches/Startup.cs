using WalksAndBenches.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using WalksAndBenches.Identity.Entities;
using WalksAndBenches.Identity;

namespace WalksAndBenches
{
    public class Startup
    {
        private readonly IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddIdentity<BenchUser, IdentityRole>(cfg =>
                {
                    cfg.User.RequireUniqueEmail = true;
                })
                .AddEntityFrameworkStores<BenchContext>();

            services.AddDbContext<BenchContext>(cfg =>
            {
                string connecitonString = _config.GetConnectionString("BenchConnectionString");
                cfg.UseSqlServer(connecitonString);
            });


            services.AddTransient<BenchSeeder>();
            services.AddTransient<IAssetService, WalksService>();

            services.AddOptions();
            services.Configure<AzureStorageConfig>(_config.GetSection("AzureStorageConfig"));

            services.AddSingleton<IStorageService>(serviceProvider => {
                var blobStorage = new BlobStorageService(serviceProvider.GetService<IOptions<AzureStorageConfig>>());
                blobStorage.ConfigureBlobStorage().GetAwaiter().GetResult();
                return blobStorage;
            });

            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
            }

            app.UseStaticFiles();
            app.UseAuthentication();

            app.UseMvc(cfg =>
            {
                cfg.MapRoute("Default", "/{controller}/{action}/{id?}",
                    new { controller = "App", Action = "Index" });
            });
        }
    }
}
