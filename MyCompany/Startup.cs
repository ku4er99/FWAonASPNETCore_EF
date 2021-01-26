using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MyCompany
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews() // Добавляем поддержку контроллеров и представлений (MVC)
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0).AddSessionStateTempDataProvider(); // Выставляем совместимость с asp.net core 3.0 
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            ///
            /// ОЧЕНЬ ВАЖЕН ПОРЯДОК РЕГИСТРАЦИИ MIDDLEWARE (функционала)
            /// 

            if (env.IsDevelopment()) // при окружении == Development
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting(); // Подключение системы маршрутизации

            app.UseStaticFiles(); // Подключаем поддержку статичных файлов (css, js, etc..)

            app.UseEndpoints(endpoints => // Регистрируем нужные нам маршруты (ендпоинты)
            {
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}"); // Условно: При переходе на site.com/ система маршрутизации будет использовать контроллер HOME, действие INDEX
            });
        }
    }
}
