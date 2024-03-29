﻿namespace Tapas.Web
{
    using System;
    using System.Linq;
    using System.Reflection;

    using AutoMapper;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Newtonsoft.Json;
    using Tapas.Common;
    using Tapas.Data;
    using Tapas.Data.Common;
    using Tapas.Data.Common.Repositories;
    using Tapas.Data.Models;
    using Tapas.Data.Repositories;
    using Tapas.Data.Seeding;
    using Tapas.Services;
    using Tapas.Services.Contracts;
    using Tapas.Services.Data;
    using Tapas.Services.Data.Contracts;
    using Tapas.Services.Mapping;
    using Tapas.Services.Messaging;
    using Tapas.Web.Hubs;
    using Tapas.Web.MappingConfig;
    using Tapas.Web.ViewModels;

    public class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(
                options => options.UseSqlServer(this.configuration.GetConnectionString("DefaultConnection"))
                                   .UseLazyLoadingProxies());
            services.AddDefaultIdentity<ApplicationUser>(IdentityOptionsProvider.GetIdentityOptions)
                .AddRoles<ApplicationRole>()
                .AddErrorDescriber<LocalizedIdentityErrorDescriber>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.Configure<CookiePolicyOptions>(
                options =>
                    {
                        options.CheckConsentNeeded = context => true;
                        options.MinimumSameSitePolicy = SameSiteMode.None;
                    });

            services.AddSignalR(
               options =>
               {
                   options.EnableDetailedErrors = true;
               }).AddMessagePackProtocol();

            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddControllersWithViews().AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);
            services.AddRazorPages();
            services.AddSingleton(this.configuration);
            services.AddResponseCompression();
            services.AddResponseCaching();

            // Data repositories
            services.AddScoped(typeof(IDeletableEntityRepository<>), typeof(EfDeletableEntityRepository<>));
            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            services.AddScoped<IDbQueryRunner, DbQueryRunner>();
            services.AddScoped<IAlarm, Alarm>();

            // Application services
            services.AddTransient<IEmailSender>(x => new SendGridEmailSender(
                this.configuration.GetSection("EmailGridSender:ApiKey").Value));
            services.AddTransient<IGeolocationService>(x => new GeolocationService(
                this.configuration.GetSection("GeolocationSettings:ApiKey").Value));
            services.AddTransient<ICloudService>(x => new CloudService(
                this.configuration.GetSection("CloudSettings:CloudName").Value,
                this.configuration.GetSection("CloudSettings:ApiKey").Value,
                this.configuration.GetSection("CloudSettings:ApiSecret").Value));
            services.AddTransient<IMistralService>(x => new MistralService(
                this.configuration.GetSection("Mistral:Password").Value));
            services.AddTransient<ITwilioSmsSenderService>(x => new TwilioSmsSenderService(
                this.configuration.GetSection("Twilio:accountSid").Value,
                this.configuration.GetSection("Twilio:authToken").Value,
                this.configuration.GetSection("Twilio:twilioNumber").Value));

            services.AddTransient<IAllergensService, AllergensService>();
            services.AddTransient<ICategoriesService, CategoriesService>();
            services.AddTransient<IProductsService, ProductsService>();
            services.AddTransient<IShopingCartService, ShopingCartService>();
            services.AddTransient<IOrdersService, OrdersService>();
            services.AddTransient<IAddressesService, AddressesService>();
            services.AddTransient<IPackagesService, PackagesService>();
            services.AddTransient<ISizesService, SizesService>();
            services.AddTransient<ICateringFoodService, CateringFoodService>();
            services.AddTransient<ICateringEquipmentService, CateringEquipmentService>();
            services.AddTransient<IExtrasService, ExtrasService>();
            services.AddTransient<IDeliveryTaxService, DeliveryTaxService>();
            services.AddTransient<IImageService, ImageService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            AutoMapperConfig.RegisterMappings(typeof(ErrorViewModel).GetTypeInfo().Assembly);

            // Seed data on application startup
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                if (env.IsDevelopment())
                {
                    dbContext.Database.Migrate();
                }

                new ApplicationDbContextSeeder().SeedAsync(dbContext, serviceScope.ServiceProvider).GetAwaiter().GetResult();

                GlobalConstants.IsOpen = Convert.ToBoolean(dbContext.Users.Find("229c10f5-1596-4300-aa09-17c84cb44e56").Claims.First().ClaimValue);
            }

            if (!env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseResponseCaching();
            app.UseResponseCompression();
            app.UseHttpsRedirection();
            app.UseStaticFiles(new StaticFileOptions()
            {
                HttpsCompression = Microsoft.AspNetCore.Http.Features.HttpsCompressionMode.Compress,
                OnPrepareResponse = (context) =>
                {
                    var headers = context.Context.Response.GetTypedHeaders();
                    headers.CacheControl = new Microsoft.Net.Http.Headers.CacheControlHeaderValue
                    {
                        Public = true,
                        MaxAge = TimeSpan.FromDays(365),
                    };
                },
            });
            app.UseCookiePolicy();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(
                endpoints =>
                    {
                        endpoints.MapHub<OrderHub>("/orderHub");
                        endpoints.MapHub<UserOrdersHub>("/userOrdersHub");
                        endpoints.MapControllerRoute("areaRoute", "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                        endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
                        endpoints.MapRazorPages();
                    });
        }
    }
}
