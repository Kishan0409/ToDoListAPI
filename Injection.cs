using System.Collections.Generic;
using ToDoListAPI.Interface;
using ToDoListAPI.Services;

namespace ToDoListAPI
{
    public static class Injection
    {
        public static IServiceCollection RegisterApplicationServices(this IServiceCollection services,
              IConfiguration configuration)
        {
            services.AddScoped<IToDoList, ToDoServices>();
            return services;
        }
    }
}
