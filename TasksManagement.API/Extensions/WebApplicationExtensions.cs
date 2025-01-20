﻿using Microsoft.EntityFrameworkCore;
using TasksManagement.Infrastructure.Database;

namespace TasksManagement.API.Extensions;

public static class WebApplicationExtensions
{
    public static void ApplyMigrations(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<SqlDbContext>();

            // Verifica se existem migrations pendentes
            var pendingMigrations = dbContext.Database.GetPendingMigrations();

            if (pendingMigrations.Any())
            {
                dbContext.Database.Migrate();
            }
        }
    }
}
