using todolist.iservice;
using todolist.service;

namespace todolist.extension;

public static class ServicesExtension
{
  public static IServiceCollection AddTodoApplicationServices(this IServiceCollection services)
  {
    services.AddScoped<ITodoService, TodoService>();
    return services;
  }
}