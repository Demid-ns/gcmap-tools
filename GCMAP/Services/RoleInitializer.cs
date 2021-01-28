using GCMAP.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GCMAP.Services
{
    public class RoleInitializer
    {
        //Инициализация БД ролиями
        public static async Task InitializeAsync(UserManager<GCMAPUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            //Конфигурация для root-пользователя
            string rootEmail = configuration["Root:Email"];
            string password = configuration["Root:Password"];
            if (await roleManager.FindByNameAsync("root") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("root"));
            }
            if (await roleManager.FindByNameAsync("admin") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("admin"));
            }
            if (await roleManager.FindByNameAsync("moderator") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("moderator"));
            }
            if (await userManager.FindByNameAsync(rootEmail) == null)
            {
                GCMAPUser root = new GCMAPUser
                {
                    Email = rootEmail,
                    UserName = rootEmail,
                    EmailConfirmed = true,
                    NickName = "root"
                };
                IdentityResult result = await userManager.CreateAsync(root, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(root, "root");
                }
            }
        }
    }
}
