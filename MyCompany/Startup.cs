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
            services.AddControllersWithViews() // ��������� ��������� ������������ � ������������� (MVC)
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0).AddSessionStateTempDataProvider(); // ���������� ������������� � asp.net core 3.0 
        }

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

            app.UseStaticFiles(); // ���������� ��������� ��������� ������ (css, js, etc..)

            app.UseEndpoints(endpoints => // ������������ ������ ��� �������� (���������)
            {
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}"); // �������: ��� �������� �� site.com/ ������� ������������� ����� ������������ ���������� HOME, �������� INDEX
            });
        }
    }
}
