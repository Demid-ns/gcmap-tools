using System;
using GCMAP.Areas.Identity.Data;
using GCMAP.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(GCMAP.Areas.Identity.IdentityHostingStartup))]
namespace GCMAP.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
                //Добавляем контекст БД (обычно делалось в методе StartUp
                services.AddDbContext<GCMAPContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("GCMAPContextConnection")));

                //Определяем опции Identity для аккаунтов приложения
                services.AddIdentity<GCMAPUser, IdentityRole>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = false;
                    options.Password.RequireDigit = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequiredLength = 7;
                    options.Password.RequireNonAlphanumeric = false;
                    options.User.RequireUniqueEmail = true;
                    options.User.AllowedUserNameCharacters = ".@abcdefghijklmnopqrstuvwxyz1234567890_-";
                })
                    .AddEntityFrameworkStores<GCMAPContext>().AddDefaultTokenProviders();

                //Определяем страницы для входа в аккаунт и страницу запрета доступа
                services.ConfigureApplicationCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.AccessDeniedPath = "/Account/Denied";
                });

                //Устанавливаем значение ValidationInterval в 0, чтобы отслеживать изменение роли
                //если это не сделать, то админ может забрать у юзера роль, но пользователя по прежнему может
                //выполнять действия авторизированного юзера пока не выйдет из профиля 
                services.Configure<SecurityStampValidatorOptions>(options =>
                {
                    options.ValidationInterval = TimeSpan.Zero;
                });
            });
        }
    }
}