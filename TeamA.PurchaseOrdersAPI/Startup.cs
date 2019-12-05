using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
using Polly;
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

            services.AddScoped<IProductsService, ProductsService>();
            services.AddScoped<IUndercuttersService, UndercuttersService>();
            services.AddScoped<IDodgyDealersService, DodgyDealersService>();

            var undercuttersAddress = Configuration.GetValue<Uri>("UndercuttersUri");

            services.AddHttpClient<IUndercuttersService, UndercuttersService>(c =>
            {
                c.BaseAddress = undercuttersAddress;
                c.DefaultRequestHeaders.Accept.Clear();
                c.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            }).AddTransientHttpErrorPolicy(p =>
                p.OrResult(r => !r.IsSuccessStatusCode)
                    .WaitAndRetryAsync(3, retry => TimeSpan.FromSeconds(Math.Pow(2, retry))))
                        .AddTransientHttpErrorPolicy(p => p.CircuitBreakerAsync(3, TimeSpan.FromSeconds(30)));

            var dodgyDealersAddress = Configuration.GetValue<Uri>("DodgyDealersUri");
            var dodgyDealersTimeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(20);

            services.AddHttpClient<IDodgyDealersService, DodgyDealersService>(c =>
            {
                c.BaseAddress = dodgyDealersAddress;
                c.DefaultRequestHeaders.Accept.Clear();
                c.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            }).AddTransientHttpErrorPolicy(p =>
                p.OrResult(r => !r.IsSuccessStatusCode)
                    .WaitAndRetryAsync(3, retry => TimeSpan.FromSeconds(Math.Pow(2, retry))))
                        .AddTransientHttpErrorPolicy(p => p.CircuitBreakerAsync(3, TimeSpan.FromSeconds(30)))
                            .AddPolicyHandler(dodgyDealersTimeoutPolicy); // This timeout is for the entire call, ensuring it just errors instead of leaving user waiting for > 30s

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
