
using Microsoft.EntityFrameworkCore;
using todolist.iservice;
using todolist.service;
using user.data;
using user.iservice;
using user.service;

namespace extensions;

public static class ApplicationExtensions
{
  // Add all application services
  public static IServiceCollection AddApplicationServices(this IServiceCollection services)
  {
    //TodoList
    services.AddScoped<ITodoService, TodoService>();

    // User Services
    services.AddScoped<IUserService, UserService>();
    return services;
  }

  // Configure database connection
  public static IServiceCollection AddDatabaseServices(this IServiceCollection services, string connectionString)
  {
    services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(connectionString));

    return services;
  }

  // Configure CORS policy
  public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
  {
    services.AddCors(options =>
    {
      options.AddPolicy("AllowAll", policy =>
      {
        policy.AllowAnyOrigin()
          .AllowAnyMethod()
          .AllowAnyHeader();
      });
    });
        
    return services;
  }
}