using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using TodoistSync.Extensions;
using TodoistSync.Services;

namespace TodoistSync
{
    public class Startup
    {
        private IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson(opts =>
            {
                opts.SerializerSettings.Converters.Add(new StringEnumConverter());
                opts.SerializerSettings.ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                };
            });

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Converters = new JsonConverter[] {new StringEnumConverter()},
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                }
            };

            services.AddCors(opts => opts.AddPolicy("AllowOrigin", policyOpts =>
            {
                policyOpts.AllowAnyOrigin();
                policyOpts.AllowAnyMethod();
                policyOpts.AllowAnyHeader();
            }));

            services.AddTodoistHttpClient<ISectionService, SectionService>(Configuration);
            services.AddTodoistHttpClient<IProjectService, ProjectService>(Configuration);
            services.AddTodoistHttpClient<IItemService, ItemService>(Configuration);
            services.AddTodoistHttpClient<ICommentsService, CommentsService>(Configuration);

            services.AddTransient<IWebhookService, WebhookService>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("AllowOrigin");

            app.UseRouting();

            app.UseEndpoints(builder => builder.MapControllers());
        }
    }
}
