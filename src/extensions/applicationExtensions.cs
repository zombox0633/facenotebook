
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using todolist.iservice;
using todolist.service;
using user.data;
using user.iservice;
using user.service;
using utils.ihashPassword;
using utils.hashPassword;
using utils.ijwt;
using utils.jwt;

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
        ValidateLifetime = true, // เช็ค token หมดอายุ
        ClockSkew = TimeSpan.Zero // ไม่เผื่อเวลา
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
}