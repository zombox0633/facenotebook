
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using todolist.iservice;
using todolist.service;
using user.data;
using user.iservice;
using user.service;
using helper.ihashPassword;
using helper.hashPassword;
using helper.ijwt;
using helper.jwt;
using System.Text.Json;

namespace extensions;

public static class ApplicationExtensions
{
  //------------------------ Add all application services -------------------------------
  public static IServiceCollection AddApplicationServices(this IServiceCollection services)
  {
    //TodoList
    services.AddScoped<ITodoService, TodoService>();

    // User Services
    services.AddScoped<IUserService, UserService>();
    services.AddScoped<IHashPassword, HashPassword>();
    services.AddScoped<IJwtService, JwtService>();
    return services;
  }

  //------------------------ Configure JWT Authentication ---------------------------------
 public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
{
  services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
  .AddJwtBearer(options =>
  {
    options.TokenValidationParameters = new TokenValidationParameters
    {
      ValidateIssuerSigningKey = true,
      IssuerSigningKey = new SymmetricSecurityKey(
        Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]!)
      ),
      ValidateIssuer = true,
      ValidIssuer = "facenotebook_app",
      ValidateAudience = true,
      ValidAudience = "earth_ten",
      ValidateLifetime = true,
      ClockSkew = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
      OnChallenge = context =>
      {
        context.HandleResponse();
        
        context.Response.StatusCode = 401;
        context.Response.ContentType = "application/json";
        
        var response = new
        {
          status = 401,
          path = context.Request.Path.Value,
          timestamp = DateTime.UtcNow,
          message = "Invalid or missing authentication token"
        };

        var jsonOptions = new JsonSerializerOptions
        {
          PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
          WriteIndented = true
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonOptions));
      },

      OnAuthenticationFailed = context =>
      {
        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<JwtBearerEvents>>();
        logger.LogWarning("JWT Authentication failed: {Error}", context.Exception.Message);
        return Task.CompletedTask;
      }
    };
  });

  services.AddAuthorization();
  return services;
}


  //------------------------ Configure database connection --------------------------------
  public static IServiceCollection AddDatabaseServices(this IServiceCollection services, string connectionString)
  {
    services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(connectionString));

    return services;
  }

  //------------------------ Configure CORS policy --------------------------------------
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

  //------------------------ Configure Error Handler Middleware -----------------------
  public static IApplicationBuilder AddErrorHandler(this IApplicationBuilder builder)
  {
    return builder.UseMiddleware<ErrorHandlerMiddleware>();
  }
}