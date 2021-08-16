using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using ProjetoAspNetAPI01.Data.Interfaces;
using ProjetoAspNetAPI01.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoAspNetAPI01.Services
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
            services.AddControllers();

            //Capturar a string de conexão mapeada no arquivo /appsettings.json
            var connectionstring = Configuration.GetConnectionString("Conexao");
            //Configurar as classes e interfaces do repositorio passando para elas
            //o valor da connectionstring do banco de dados..
            services.AddTransient<IClienteRepository, ClienteRepository>
                (map => new ClienteRepository(connectionstring));

            //Fazendo a configuração para gerar a documentação da API.
            services.AddSwaggerGen(
                swagger =>
                {
                    swagger.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Title = "API para controle de clientes - Treinamento em C# WebDeveloper.",
                        Description = "Projeto desenvolvido em AspNet 5 API com SqlServer e Dapper.",
                        Version = "v1",
                        Contact = new OpenApiContact
                        {
                            Name = "COTI Informática - Escola de NERDS",
                            Url = new Uri("http://www.cotiinformatica.com.br"),
                            Email = "contato@cotiinformatica.com.br"
                        }
                    });
                }
                );

            //Fazendo a configuração do CORS (Permissão de chamadas para a API)
            services.AddCors(
                    s => s.AddPolicy("DefaultPolicy", builder => 
                    {
                        builder.AllowAnyOrigin() //qualquer origem pode acessar a API
                               .AllowAnyMethod() //qualquer método (POST, PUT, DELETE, GET)
                               .AllowAnyHeader(); //qualquer informação de cabeçalho
                    })
                );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //incluindo uma configuração adicional para gerar a documentação do swagger
            app.UseSwagger();
            app.UseSwaggerUI(s => { s.SwaggerEndpoint("/swagger/v1/swagger.json", "COTI API"); });

            app.UseRouting();

            app.UseCors("DefaultPolicy");

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
