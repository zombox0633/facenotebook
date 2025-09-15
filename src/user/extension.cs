using user.iservice;
using user.service;

namespace user.extension;

public static class ServicesExtension
{
  public static IServiceCollection AddUserApplicationServices(this IServiceCollection services)
  {
    services.AddScoped<IUserService, UserService>();
    return services;
  }
}