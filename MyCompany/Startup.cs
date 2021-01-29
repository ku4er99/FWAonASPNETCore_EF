using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MyCompany.Domain;
using MyCompany.Domain.Repositories.Abstract;
using MyCompany.Domain.Repositories.EntityFramework;
using MyCompany.Service;

namespace MyCompany
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration) => Configuration = configuration;


        public void ConfigureServices(IServiceCollection services)
        {
            // Подключаем кофиг из appsettings.json
            Configuration.Bind("Project", new Config()); 

            // Подключаем нужный функционал приложения в качестве сервисов (функциональная возможность смены системы связи с бд)
            services.AddTransient<ITextFieldsRepository, EFTextFieldsRepository>();// Транзитивность запроса означает, что в рамках одного http запроса может быть создано несколько таких объектов
            services.AddTransient<IServiceItemsRepository, EFServiceItemsRepository>();
            services.AddTransient<DataManager>();

            // Подключаем контекст базы данных
            services.AddDbContext<AppDbContext>(x => x.UseSqlServer(Config.ConnectionString));

            // Настраиваем identity систему (почитать про нее) // определение требований для системы безопасности
            services.AddIdentity<IdentityUser, IdentityRole>(opts =>
            {
                opts.User.RequireUniqueEmail = true;
                opts.Password.RequiredLength = 6;
                opts.Password.RequireNonAlphanumeric = false;
                opts.Password.RequireLowercase = false;
                opts.Password.RequireUppercase = false;
                opts.Password.RequireDigit = false;
            }).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

            // Настраиваем autentification cookie
            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Name = "myCompanyAuth";
                options.Cookie.HttpOnly = true;
                options.LoginPath = "/account/login"; // По этому пути будет лежать акаунт контроллер для Админ панели
                options.AccessDeniedPath = "/account/accessdenied";
                options.SlidingExpiration = true;
            });

            // Настраиваем политику авторизации для Admin area
            services.AddAuthorization(x =>
            {
                x.AddPolicy("AdminArea", policy => { policy.RequireRole("admin"); });
            });

            // Добавляем поддержку контроллеров и представлений (MVC)
            services.AddControllersWithViews(x =>
                {
                    x.Conventions.Add(new AdminAreaAuthorization("Admin","AdminArea"));
                })
                // Выставляем совместимость с asp.net core 3.0 
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0).AddSessionStateTempDataProvider();         }

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

            // подключаем аутентификацию и авторизацию (обязательно после подключения маршрутизации, но до ее определения!!!)
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseStaticFiles(); // Подключаем поддержку статичных файлов (css, js, etc..)

            app.UseEndpoints(endpoints => // Регистрируем нужные нам маршруты (ендпоинты)
            {
                endpoints.MapControllerRoute("admin", "{area:exists}/{controller=Home}/{action=Index}/{id?}"); // Объявляем спец маршрут для области admin
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}"); // Условно: При переходе на site.com/ система маршрутизации будет использовать контроллер HOME, действие INDEX
            });
        }
    }
}
