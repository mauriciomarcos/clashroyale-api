using System;
using System.IO;
using System.Reflection;
using ClashRoyaleService;
using ClashRoyaleService.ServiceInterfaces;
using ClashRoyaleUtils.Configurations;
using ClashRoyaleUtils.HealthCheckCustons;
using ClashRoyaleUtils.RouteConstraints;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace ClashRoyaleAPI
{
    /// <summary>
    /// 
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// 
        /// </summary>
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddSingleton<IConfiguration>(Configuration);

            CarregarDependenciasDominioAplicacao(services);
            ConfigurarDependenciasSwagger(services);

            /*
             * O MemoryCache não é indicado para API que possívelmente poderão ser escaladas horizontalmente.
             * 
             * Exemplo: se a appicação por necessidade possuir duas instâncias de uma mesma API em servidores diferentes,
             * haverá um disperdício de memória, uma vez que os dados de cache estarão ocupando recurso de dois servidores, e também,
             * as informações em cache serão diferentes.
             */
            services.AddMemoryCache();
            services.AddDistributedMemoryCache();

            // Criando a injeção de dependência da classe criada [ConfigurationKeyAPI] para possibilitar o Bind da chave "ConfigurationKeyAPI" do arquivo appsettings.json
            // O nome da classe ConfigurationKeyApi é igual ao nome da Section no arquivo appsettings.json e a Propriedade da classe ApiKey é igual ao nome e tipo da chave que se quer recuperar do arquivo appsettings.json.
            services.Configure<ConfigurationAPI>(Configuration.GetSection("ConfigurationAPI"));

            // Adicionada a injeção para utilização do Microsoft.Extensions.Diagnostics.HealthChecks (instalado versão 2.2.0 via NuGet)
            var hc = services.AddHealthChecks();

            // Install-Package AspNetCore.HealthChecks.Uris (a partir desse pacote é possível utilizar o extension method AddUrlGroup() para indicar a URI que será monitorada)
            // NOTE: Neste repositório do GitHub https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks, é listado os principais monitoramentos do HealthChecks disponibilizado pelo pacote 
            hc.AddUrlGroup(
                uri: new Uri("https://localhost:44381/api/card"), 
                name: "endpoint: https://localhost:44381/api/card",
                httpMethod: System.Net.Http.HttpMethod.Get
                );

            // Utilizando um HealthCheck customizado
            hc.AddCheck<HealthCheckRandonNumber>(name: "Monitoramento Randon Number");

            // Adicionada Microsoft.Extensions.Diagnostics.HealthChecks(instalado versão 2.2.0 via NuGet)
            services.AddHealthChecksUI(setupSettings: setup => {
                setup.AddHealthCheckEndpoint("Health Checks API Clash Royale", "https://localhost:44381/healthChecks");                                                                                       
            });

            // Adicionando e configurando o serviço de roteamento para habilitar a constraint criada: RouteConstraintIDCardValidation. 
            services.AddRouting(configureOptions: options => {
                                       // key: nome da constraint value: é o tipo da cnstraint, neste caso RouteConstraintIDCardValidation                     
                options.ConstraintMap.Add(key:"idCardConstraint", value: typeof(RouteConstraintIDCardValidation));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
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
            app.UseSwaggerUI(setupAction: config => {
                config.SwaggerEndpoint("/swagger/Doc.v1.0/swagger.json", "Open API Clash Royale - Versão 1.0");
            });

            app.UseMvc();

            //Aspnetcore.Healthchecks.UI (instalado versão 2.2.27 via NuGet)
            var options = new HealthCheckOptions()
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            };

            // Configuração da rota (path) ao qual o HealthChecks deve responder.
            // Microsoft.Extensions.Diagnostics.HealthChecks(instalado versão 2.2.0 via NuGet)
            app.UseHealthChecks(path:"/healthChecks", options: options);

            //Configuração da rota (path) ao qual o HealthChecks UI deve responder.
            app.UseHealthChecksUI(setup: config => {
                config.UIPath = "/healthChecks-ui";
            });
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
