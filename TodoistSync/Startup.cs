using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
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

            services.AddCors(opts => opts.AddPolicy("AllowOrigin", policyOpts =>
            {
                policyOpts.AllowAnyOrigin();
                policyOpts.AllowAnyMethod();
                policyOpts.AllowAnyHeader();
            }));
            
            services.Configure<TodoistConfig>(Configuration);

            services.AddHttpClient<IProjectService, ProjectService>(client =>
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Configuration.GetValue<string>("ApiKey")}");
                client.BaseAddress = new Uri("https://api.todoist.com/sync/v8/");
            });

            services.AddHttpClient<ITemplateService, TemplateService>(client =>
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Configuration.GetValue<string>("ApiKey")}");
                client.BaseAddress = new Uri("https://api.todoist.com/sync/v8/");
            });
                
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
