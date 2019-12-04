using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TeamA.PurchaseOrders.Data;
using TeamA.PurchaseOrders.Services;
using TeamA.PurchaseOrders.Services.Interfaces;
using TeamA.PurchaseOrders.Services.Services;

namespace TeamA.PurchaseOrdersAPI
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddDbContext<PurchaseOrdersDb>(options => options.UseSqlServer(
                Configuration.GetConnectionString("PurchaseOrders")));

            services.AddScoped<IUndercuttersService, UndercuttersService>();

            var undercuttersAddress = Configuration.GetValue<Uri>("UndercuttersUri");
            services.AddHttpClient<IUndercuttersService, UndercuttersService>(c =>
            {
                c.BaseAddress = undercuttersAddress;
                c.DefaultRequestHeaders.Accept.Clear();
                c.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            });
            //todo: add polly

            var dodgyDealersAddress = Configuration.GetValue<Uri>("DodgyDealersUri");
            services.AddHttpClient<IDodgyDealersService, DodgyDealersService>(c =>
            {
                c.BaseAddress = dodgyDealersAddress;
                c.DefaultRequestHeaders.Accept.Clear();
                c.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/xml"));
            });
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
