using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
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
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using TeamA.PurchaseOrders.Data;
using TeamA.PurchaseOrders.Repository.Interfaces;
using TeamA.PurchaseOrders.Repository.Repositories;
using TeamA.PurchaseOrders.Services.BackgroundServices;
using TeamA.PurchaseOrders.Services.Factories;
using TeamA.PurchaseOrders.Services.Interfaces;
using TeamA.PurchaseOrders.Services.Interfaces.External;
using TeamA.PurchaseOrders.Services.Services;
using TeamA.PurchaseOrders.Services.Services.External;

namespace TeamA.PurchaseOrdersAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration, Microsoft.AspNetCore.Hosting.IHostingEnvironment environment)

        {
            Configuration = configuration;
            Environment = environment;
        }

        private Microsoft.AspNetCore.Hosting.IHostingEnvironment Environment;

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder =>
                    {
                        builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                    });
            });

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddDbContext<PurchaseOrdersDb>(options => options.UseSqlServer(
                Configuration.GetConnectionString("PurchaseOrders")));

            services.AddScoped<IProductsService, ProductsService>();
            services.AddScoped<IUndercuttersService, UndercuttersService>();
            services.AddScoped<IDodgyDealersService, DodgyDealersService>();
            services.AddScoped<IBazzasBazaarService, BazzasBazaarService>();
            services.AddScoped<IOrdersRepository, OrdersRepository>();
            services.AddScoped<IProductsRepository, ProductsRepository>();

            services.AddScoped<StoreClient>();
            services.AddScoped<IOrdersFactory, OrdersFactory>();

            services.AddHostedService<ProductIngestionService>();

            services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
            options.Audience = "staff_api"; ;
            options.Authority = "https://threeamigosauth.azurewebsites.net/";
            });

            if(Environment.IsDevelopment())
            {
                services.AddScoped<IStockService, StockServiceFake>();
            } else
            {
                //var stockServiceAddress = Configuration.GetValue<Uri>("StockServiceUri");
                //services.AddHttpClient<IStockService, StockService>(c =>
                //{
                //    c.BaseAddress = stockServiceAddress;
                //    c.DefaultRequestHeaders.Accept.Clear();
                //    c.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                //}).AddTransientHttpErrorPolicy(p =>
                //    p.OrResult(r => !r.IsSuccessStatusCode)
                //        .WaitAndRetryAsync(3, retry => TimeSpan.FromSeconds(Math.Pow(2, retry))))
                //.AddTransientHttpErrorPolicy(p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));
                //services.AddScoped<IStockService, StockService>();
                services.AddScoped<IStockService, StockServiceFake>();
            }
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Customer", builder =>
                {
                    builder.RequireClaim("role", "Customer", "Admin", "Staff");
                });
                options.AddPolicy("Staff", builder =>
                {
                    builder.RequireClaim("role", "Staff", "Admin");
                });
            });

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
                            .AddPolicyHandler(dodgyDealersTimeoutPolicy);

            // Add a http client for the background services - this can be much more lenient than others as its doing its work as an inexpensive background task. 
            services.AddHttpClient();
            services.AddHttpClient("background", c => 
            {
                c.DefaultRequestHeaders.Accept.Clear();
                c.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            }).AddTransientHttpErrorPolicy(p =>
                p.OrResult(r => !r.IsSuccessStatusCode)
                    .WaitAndRetryAsync(8, retry => TimeSpan.FromSeconds(Math.Pow(2, retry))));


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
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

            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
