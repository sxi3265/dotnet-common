using System;
using Microsoft.Extensions.DependencyInjection;

namespace EasyNow.Office
{
    public static class ExcelExtensions
    {
        public static IServiceCollection AddExcelUtil(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof (services));
            services.AddSingleton<IExcelUtil, NpoiExcelUtil>();
            return services;
        }
    }
}