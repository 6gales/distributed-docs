using System;
using System.IO;
using System.Reflection;
using Microsoft.OpenApi.Models;
using DistributedDocs.DocumentChanges;
using DistributedDocs.FileSystem;
using DistributedDocs.Server.ConnectReceivers;
using DistributedDocs.Server.ConnectSenders;
using DistributedDocs.Server.Services;
using DistributedDocs.Server.Users;
using DistributedDocs.VersionHistory;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DistributedDocs.Server
{
    public sealed class Startup
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
	        services
		        .AddMvc(options => options.EnableEndpointRouting = false)
		        .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddControllersWithViews();
            services.AddRazorPages();

            services.AddSingleton<IUserStorage, UserStorage>();

            services.AddSingleton<ServerSideCommunicator, ServerSideCommunicator>();
            services.AddSingleton<IFileSynchronizerProvider<ITextDiff>, FileSynchronizerProvider>();
            services.AddSingleton<IVersionHistoryProvider<ITextDiff>, VersionHistoryProvider>();

            var authorInfoEditor = new AuthorInfoEditor();
            services.AddSingleton<IAuthorInfoEditor>(authorInfoEditor);
            services.AddSingleton<IAuthorProvider>(authorInfoEditor);

            services.AddSingleton<DocumentContext, DocumentContext>();

			services.AddSingleton<ConnectSender>();
			services.AddSingleton<ConnectReceiver>();

			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", 
					new OpenApiInfo
					{
						Title = "Distributed docs API", 
						Version = "v1"
					});

				var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
				var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
				c.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            //app.UseSwagger(c =>
            //{
	           // c.SerializeAsV2 = true;
            //});

            //app.UseSwaggerUI(c =>
            //{
	           // c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            //});

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
	            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Distributed docs API");
            });

            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseMvcWithDefaultRoute();

            app.UseEndpoints(endpoints =>
            {
	            endpoints.MapControllers();
                endpoints.MapRazorPages();
                endpoints.MapFallbackToFile("index.html");
            });

            //app.UseSwagger();




            app.ApplicationServices.GetService<ConnectSender>();
            app.ApplicationServices.GetService<ConnectReceiver>();
        }
    }
}
