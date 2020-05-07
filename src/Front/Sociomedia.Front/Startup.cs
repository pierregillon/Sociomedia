using System;
using LinqToDB.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sociomedia.FeedAggregator.Infrastructure;
using Sociomedia.Front.Data;
using Sociomedia.ReadModel.DataAccess;
using Sotsera.Blazor.Toaster.Core.Models;
using StructureMap;

namespace Sociomedia.Front
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();

            services.AddToaster(config => {
                config.PositionClass = Defaults.Classes.Position.TopRight;
                config.PreventDuplicates = true;
                config.NewestOnTop = true;
                config.VisibleStateDuration = 5000;
            });
        }

        public void ConfigureContainer(Registry services)
        {
            services.For<ArticleFinder>();
            services.For<MediaFinder>();
            services.IncludeRegistry<SociomediaRegistry>();

            DataConnection.DefaultSettings = new DbSettings(Configuration.GetSection("sqldatabase").Get<SqlDatabaseConfiguration>());
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            else {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints => {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}