using System;
using System.IO;
using System.Reflection;
using ClashRoyaleService;
using ClashRoyaleService.ServiceInterfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace ClashRoyaleAPI
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
            services.AddSingleton<IConfiguration>(Configuration);

            CarregarDependenciasDominioAplicacao(services);
            ConfigurarDependenciasSwagger(services);

            services.AddMemoryCache();
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
            app.UseSwagger();
            app.UseSwaggerUI(config => {
                config.SwaggerEndpoint("/swagger/Doc.v1.0/swagger.json", "Open API Clash Royale - Versão 1.0");
            });
            app.UseMvc();
        }

        private void CarregarDependenciasDominioAplicacao(IServiceCollection services)
        {
            services.AddSingleton<ICardService, CardService>();
        }

        private void ConfigurarDependenciasSwagger(IServiceCollection services)
        {
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = $"{Path.Combine(AppContext.BaseDirectory, xmlFile)}";

            services.AddSwaggerGen(config => {
                config.SwaggerDoc("Doc.v1.0", new OpenApiInfo() {
                    Title = "Open API Clash Royale",
                    Version = "v1.0",
                    Description = "API aberta do Clash Royale. Recursos aberto para integração com sites e outros formatos de mídias relacionado ao jogo Clash Royale.",
                    Contact = new OpenApiContact() {
                        Name = "Maurício Marcos",
                        Email = "mauriciomarcos.consultoria@gmail.com",
                        Url = new Uri("https://www.linkedin.com/in/mmarcos001") 
                    },
                    TermsOfService = new Uri("https://github.com/GITMMarcos/clashroyale-api")
                });
                config.IncludeXmlComments(xmlPath);
            });
        }
    }
}
