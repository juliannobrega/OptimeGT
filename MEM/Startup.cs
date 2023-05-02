using FluentNHibernate.Cfg.Db;
using GQService.com.gq.dto;
using GQService.com.gq.log;
using GQService.com.gq.service;
using log4net.Repository;
using MEM.com.gq.security;
using MEM.Controllers;
using MEMDataService.com.gq.mapping;
using MEMDataService.com.gq.migrations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StructureMap;
using System;

namespace MEM
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddJsonFile("config.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            //if (env.IsDevelopment())
            //{
            //    // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
            //    builder.AddApplicationInsightsSettings(developerMode: true);
            //}

            Configuration = builder.Build();

            ILoggerRepository rep = log4net.LogManager.CreateRepository("GeminusQhom");

            log4net.Config.XmlConfigurator.Configure(rep, new System.IO.FileInfo(env.ContentRootPath + "\\log4netConfig.xml"));

            Log.Info("****************************************************************************************");
            Log.Info("****************************************************************************************");
            Log.Info("************************************   Startup  ****************************************");
            Log.Info("****************************************************************************************");
            Log.Info("****************************************************************************************");
        }

        public static IConfigurationRoot Configuration { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            //services.AddApplicationInsightsTelemetry(Configuration);

            services.AddMvc();

            /// Esto es para que maneje sessiones
            services.AddSession();

            // Ponemos en Singleton el HTTPContext
            services.AddSingleton<Microsoft.AspNetCore.Http.IHttpContextAccessor, Microsoft.AspNetCore.Http.HttpContextAccessor>();

            // Usamos StructureMap
            GQService.com.gq.structureMap.ObjectFactory.Container.Populate(services);

            return GQService.com.gq.structureMap.ObjectFactory.Container.GetInstance<IServiceProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IServiceProvider svp)
        {
            // Porque no carga el Assembly
            var a = new MapGq_accesos();
            //Para que podamos usar el HTTPContext
            System.Web.HttpContext.ServiceProvider = svp;
            

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            //loggerFactory.AddDebug();

            Log.Info("************************************   Migrator  ****************************************");
            GQService.com.gq.service.Migrator.Configure(Configuration.GetSection("ConnectionDB"));
            GQService.com.gq.service.Migrator.Start(MigratorConfig.GetAssembly());
            Log.Info("************************************   Migrator  ****************************************");

            Log.Info("************************************   ServiceConfigure  ****************************************");
            ServiceConfigure.Configure(MigratorConfig.GetAssembly(), MySQLConfiguration.Standard
                     .QuerySubstitutions("1 true, 0 false")
                     .ConnectionString(Configuration.GetSection("ConnectionDB").GetSection("ConnectionString").Value)
                     .Driver<NHibernate.Driver.MySqlDataDriver>());
            Log.Info("************************************   ServiceConfigure  ****************************************");

            Log.Info("************************************   DtoConfiguration  ****************************************");
            DtoConfiguration.Configure();
            Log.Info("************************************   DtoConfiguration  ****************************************");

            Log.Info("************************************   SecurityConfigure  ****************************************");
            GQService.com.gq.security.SecurityConfigure.Configure(
                TimeSpan.FromHours(24),
                Security.hasPermission,
                Security.CheckUsuarioLoginKey,
                new Type[] { typeof(LoginController) /*, typeof(MovileController)*/ });
            Security.CreateAccessSecurity();
            Log.Info("************************************   SecurityConfigure  ****************************************");

            //app.UseApplicationInsightsRequestTelemetry();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            //app.UseApplicationInsightsExceptionTelemetry();
            app.UseStaticFiles();

            //Indicamos que vamos a usas sesioens
            app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
