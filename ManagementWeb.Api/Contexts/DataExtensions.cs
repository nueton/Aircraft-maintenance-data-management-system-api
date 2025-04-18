using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Writers;

namespace ManagementWeb.Api.Contexts;

public static class DataExtensions
{
    public static void MigrateDb(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<MyContext>();
        dbContext.Database.Migrate();
    }
}
