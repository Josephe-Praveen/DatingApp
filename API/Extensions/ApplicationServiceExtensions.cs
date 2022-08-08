using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationsService(this IServiceCollection services , IConfiguration Configuration  )
        {
            services.AddScoped<iTokenService ,TokenService>();
            services.AddDbContext<DataContext>(options =>{options.UseSqlite(Configuration.GetConnectionString("DefaultConnection"));});
           return services; 
        }
    }
}