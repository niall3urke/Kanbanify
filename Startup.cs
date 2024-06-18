using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kanbanify.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TG.Blazor.IndexedDB;

namespace Kanbanify
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddScoped<ModalService>();
            ConfigureDatabase(services);
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
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }

        private void ConfigureDatabase(IServiceCollection services)
        {
            // Create our tables first

            var users = new StoreSchema()
            {
                Name = "Users",
                PrimaryKey = new IndexSpec
                {
                    Name = "id",
                    KeyPath = "id",
                    Unique = true
                }
            };

            var projects = new StoreSchema()
            {
                Name = "Projects",
                PrimaryKey = new IndexSpec
                {
                    Name = "id",
                    KeyPath = "id",
                    Unique = true
                }
            };

            var stages = new StoreSchema()
            {
                Name = "Stages",
                PrimaryKey = new IndexSpec
                {
                    Name = "id",
                    KeyPath = "id",
                    Unique = true
                }
            };

            var items = new StoreSchema()
            {
                Name = "Items",
                PrimaryKey = new IndexSpec
                {
                    Name = "id",
                    KeyPath = "id",
                    Unique = true
                }
            };

            var tasks = new StoreSchema()
            {
                Name = "Tasks",
                PrimaryKey = new IndexSpec
                {
                    Name = "id",
                    KeyPath = "id",
                    Unique = true
                }
            };

            services.AddIndexedDB(db =>
            {
                db.DbName = "Kanbanify";
                db.Version = 1;
                db.Stores.Add(users);
                db.Stores.Add(projects);
                db.Stores.Add(stages);
                db.Stores.Add(items);
                db.Stores.Add(tasks);
            });

            services.AddScoped<DatabaseOperations>();
        }

    }
}
