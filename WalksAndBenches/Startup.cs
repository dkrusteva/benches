using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalksAndBenches.Data;
using WalksAndBenches.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace WalksAndBenches
{
    public class Startup
    {
        private readonly IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<BenchContext>(cfg =>
            {
                string connecitonString = _config.GetConnectionString("BenchConnectionString");
                cfg.UseSqlServer(connecitonString);
            });


            services.AddTransient<BenchSeeder>();
            services.AddTransient<IAssetService, WalksService>();

            services.AddOptions();
            services.Configure<AzureStorageConfig>(_config.GetSection("AzureStorageConfig"));

            // Wire up a single instance of BlobStorage, calling Initialize() when we first use it.
            services.AddSingleton<IStorageService>(serviceProvider => {
                var blobStorage = new BlobStorageService(serviceProvider.GetService<IOptions<AzureStorageConfig>>());
                blobStorage.ConfigureBlobStorage().GetAwaiter().GetResult();
                return blobStorage;
            });

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
                app.UseDeveloperExceptionPage();
            //}
            //else
            //{
            //    app.UseExceptionHandler("/error");
            //}

            app.UseStaticFiles();

            app.UseMvc(cfg =>
            {
                cfg.MapRoute("Default", "/{controller}/{action}/{id?}",
                    new { controller = "App", Action = "Index" });
            });
        }
    }
}
