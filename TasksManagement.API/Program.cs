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
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configuração do pipeline de requisições HTTP
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
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
