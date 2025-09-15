using Microsoft.EntityFrameworkCore;
using user.data;

namespace extensions;

public static class DatabaseExtensions
{
  public static async Task InitializeDatabaseAsync(this WebApplication app)
  {
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
      var canConnect = await context.Database.CanConnectAsync();
      if (canConnect)
      {
        Console.WriteLine("✅ Database connection successful!");

        // Optional: Check if tables exist
        var tables = await context.Database.SqlQueryRaw<string>(
        "SELECT tablename FROM pg_tables WHERE schemaname = 'public'"
        ).ToListAsync();
        if (tables.Contains("users"))
        {
          Console.WriteLine("📋 Users table found and ready");
        }
        else
        {
          Console.WriteLine("⚠️ Users table not found - please create it manually");
        }
      }
      else
      {
        Console.WriteLine("❌ Cannot connect to database");
        throw new InvalidOperationException("Database connection failed");
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine($"❌ Database initialization failed: {ex.Message}");
      throw;
    }
  }
}