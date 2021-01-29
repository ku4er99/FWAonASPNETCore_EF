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
            // ���������� ����� �� appsettings.json
            Configuration.Bind("Project", new Config()); 

            // ���������� ������ ���������� ���������� � �������� �������� (�������������� ����������� ����� ������� ����� � ��)
            services.AddTransient<ITextFieldsRepository, EFTextFieldsRepository>();// �������������� ������� ��������, ��� � ������ ������ http ������� ����� ���� ������� ��������� ����� ��������
            services.AddTransient<IServiceItemsRepository, EFServiceItemsRepository>();
            services.AddTransient<DataManager>();

            // ���������� �������� ���� ������
            services.AddDbContext<AppDbContext>(x => x.UseSqlServer(Config.ConnectionString));

            // ����������� identity ������� (�������� ��� ���) // ����������� ���������� ��� ������� ������������
            services.AddIdentity<IdentityUser, IdentityRole>(opts =>
            {
                opts.User.RequireUniqueEmail = true;
                opts.Password.RequiredLength = 6;
                opts.Password.RequireNonAlphanumeric = false;
                opts.Password.RequireLowercase = false;
                opts.Password.RequireUppercase = false;
                opts.Password.RequireDigit = false;
            }).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

            // ����������� autentification cookie
            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Name = "myCompanyAuth";
                options.Cookie.HttpOnly = true;
                options.LoginPath = "/account/login"; // �� ����� ���� ����� ������ ������ ���������� ��� ����� ������
                options.AccessDeniedPath = "/account/accessdenied";
                options.SlidingExpiration = true;
            });

            // ����������� �������� ����������� ��� Admin area
            services.AddAuthorization(x =>
            {
                x.AddPolicy("AdminArea", policy => { policy.RequireRole("admin"); });
            });

            // ��������� ��������� ������������ � ������������� (MVC)
            services.AddControllersWithViews(x =>
                {
                    x.Conventions.Add(new AdminAreaAuthorization("Admin","AdminArea"));
                })
                // ���������� ������������� � asp.net core 3.0 
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0).AddSessionStateTempDataProvider();         }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            ///
            /// ����� ����� ������� ����������� MIDDLEWARE (�����������)
            /// 

            if (env.IsDevelopment()) // ��� ��������� == Development
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting(); // ����������� ������� �������������

            // ���������� �������������� � ����������� (����������� ����� ����������� �������������, �� �� �� �����������!!!)
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseStaticFiles(); // ���������� ��������� ��������� ������ (css, js, etc..)

            app.UseEndpoints(endpoints => // ������������ ������ ��� �������� (���������)
            {
                endpoints.MapControllerRoute("admin", "{area:exists}/{controller=Home}/{action=Index}/{id?}"); // ��������� ���� ������� ��� ������� admin
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}"); // �������: ��� �������� �� site.com/ ������� ������������� ����� ������������ ���������� HOME, �������� INDEX
            });
        }
    }
}
