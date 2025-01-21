using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using System.Reflection;
using TasksManagement.API.Extensions;
using TasksManagement.Application;
using TasksManagement.Infrastructure;

namespace TasksManagement.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Adicionar serviços ao container
            builder.Services.AddControllers();

            // Configuração de serviços de aplicação e infraestrutura
            builder.Services.AddApplication();
            builder.Services.AddInfrastructure(builder.Configuration);

            // Configuração de Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {

                c.MapType<NoContentResult>(() => new OpenApiSchema { Type = "null" });

                c.EnableAnnotations();

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                c.IncludeXmlComments(xmlPath);


            });

            var app = builder.Build();

            // Configuração do pipeline de requisições HTTP
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();

                // Configuração do Swagger UI
                app.UseSwaggerUI(c =>
                {
                    c.RoutePrefix = string.Empty;
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
                });
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            // Aplicar Migrations automaticamente
            app.ApplyMigrations();

            app.Run();
        }
    }
}
