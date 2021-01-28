using GCMAP.Areas.Identity.Data;
using GCMAP.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GCMAP.Controllers
{
    [Authorize(Roles = "admin, root")]
    public class AdminController : Controller
    {
        private readonly UserManager<GCMAPUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        //Инжектим необходимые для Identity сервисы
        public AdminController(UserManager<GCMAPUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        //Возвращаем список пользователей
        public IActionResult Users()
        {
            return View("Users", _userManager.Users.ToList());
        }

        //Представление для регистрации админом пользователя
        public IActionResult Register()
        {
            return View("~/Views/Account/Register.cshtml");
        }

        //Обрабатываем пост запрос регистрации
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            //Валидация модели
            if (ModelState.IsValid)
            {
                GCMAPUser user = new GCMAPUser
                {
                    UserName = model.Email,
                    NickName = model.NickName,
                    Email = model.Email,
                    EmailConfirmed = true
                };
                // добавляем пользователя
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("Users");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View("~/Views/Account/Register.cshtml", model);
        }

        //Редактируем пользователя
        public async Task<IActionResult> EditUser(string id)
        {
            GCMAPUser user = await _userManager.FindByIdAsync(id);
            //Является ли найденный юзер root (имеет ли роль рут)?
            bool isRoot = await _userManager.IsInRoleAsync(user, "root");
            if (user == null)
            {
                return NotFound();
            }
            //root нельзя изменить
            if (isRoot)
            {
                return StatusCode(403);
            }
            EditUserViewModel model = new EditUserViewModel
            {
                Id = id,
                NickName = user.NickName,
                Email = user.Email
            };
            return View(model);
        }

        //Обрабатываем post-запрос на редактирование пользователя
        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                GCMAPUser user = await _userManager.FindByIdAsync(model.Id);
                //Является ли найденный юзер root (имеет ли роль рут)?
                bool isRoot = await _userManager.IsInRoleAsync(user, "root");
                //root нельзя изменить
                if (user!=null && isRoot)
                {
                    return StatusCode(403);
                }
                else if (user != null)
                {
                    //Изменяем данные пользователя на пришедшие в модели запроса
                    user.UserName = model.Email;
                    user.Email = model.Email;
                    user.NickName = model.NickName;

                    //обновляем юзера методами Identity
                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Users");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
            }
            return View(model);
        }

        //Удаляем пользователя
        [HttpPost]
        public async Task<ActionResult> Delete(string id)
        {
            if (id != null)
            {
                GCMAPUser user = await _userManager.FindByIdAsync(id);
                //Является ли найденный юзер root (имеет ли роль рут)?
                bool isRoot = await _userManager.IsInRoleAsync(user, "root");
                //root нельзя удалить
                if (user!=null && isRoot)
                {
                    return StatusCode(403);
                }
                else if (user != null)
                {
                    await _userManager.DeleteAsync(user);
                }
                return RedirectToAction("Users");
            }
            return NotFound();
        }

        //Изменяем пароль пользователя с id
        public async Task<IActionResult> ChangePassword(string id)
        {
            GCMAPUser user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            ChangePasswordViewModel model = new ChangePasswordViewModel { Id = user.Id, Email = user.Email };
            return View(model);
        }

        //Обрабатываем запрос на изменение пароля пользователя
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                GCMAPUser user = await _userManager.FindByIdAsync(model.Id);
                //Является ли найденный юзер root (имеет ли роль рут)?
                bool isRoot = await _userManager.IsInRoleAsync(user, "root");
                //Пароль для root-пользователя изменить нельзя
                if (user != null && !isRoot)
                {
                    //Определяем сервисы для валидации и хэширования пароля, чтобы создать и хэшировать новый пароль
                    var _passwordValidator =
                        HttpContext.RequestServices.GetService(typeof(IPasswordValidator<GCMAPUser>)) as IPasswordValidator<GCMAPUser>;
                    var _passwordHasher =
                        HttpContext.RequestServices.GetService(typeof(IPasswordHasher<GCMAPUser>)) as IPasswordHasher<GCMAPUser>;

                    IdentityResult result =
                        await _passwordValidator.ValidateAsync(_userManager, user, model.NewPassword);
                    if (result.Succeeded)
                    {
                        //хэшируем пароль
                        user.PasswordHash = _passwordHasher.HashPassword(user, model.NewPassword);
                        //Обновляем пароль пользователя
                        await _userManager.UpdateAsync(user);
                        return RedirectToAction("Users");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Пользователь не найден или является root");
                }
            }
            return View(model);
        }

        //Возвращаем роли
        public IActionResult Roles() => View(_roleManager.Roles.ToList());

        //Возвращаем страницу для создания ролей
        public IActionResult CreateRole() => View();

        [HttpPost]
        public async Task<IActionResult> CreateRole(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                //Создаем роль
                IdentityResult result = await _roleManager.CreateAsync(new IdentityRole(name));
                if (result.Succeeded)
                {
                    return RedirectToAction("Roles");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(name);
        }

        //Удаляем роль
        [HttpPost]
        public async Task<IActionResult> DeleteRole(string id)
        {
            //поиск удаляемой роли
            IdentityRole role = await _roleManager.FindByIdAsync(id);
            if (role != null)
            {
                await _roleManager.DeleteAsync(role);
            }
            return RedirectToAction("Roles");
        }

        //Изменяем роли
        public async Task<IActionResult> ChangeRole(string id)
        {
            // получаем пользователя
            GCMAPUser user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                // получем список ролей пользователя
                var userRoles = await _userManager.GetRolesAsync(user);
                //роль root не может быть передана в представление для изменения ролей
                var rootRole = await _roleManager.FindByNameAsync("root");
                var allRoles = _roleManager.Roles.ToList();
                allRoles.Remove(rootRole);
                ChangeRoleViewModel model = new ChangeRoleViewModel
                {
                    UserId = user.Id,
                    UserEmail = user.Email,
                    UserRoles = userRoles,
                    AllRoles = allRoles
                };
                return View(model);
            }
            return NotFound();
        }

        //обрабатываем post-запрос на смену роли
        [HttpPost]
        public async Task<IActionResult> ChangeRole(string userId, List<string> roles)
        {
            // получаем пользователя
            GCMAPUser user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                // получем список ролей пользователя
                var userRoles = await _userManager.GetRolesAsync(user);
                // получаем все роли
                var allRoles = _roleManager.Roles.ToList();
                // получаем список ролей, которые были добавлены
                var addedRoles = roles.Except(userRoles);
                // получаем роли, которые были удалены
                var removedRoles = userRoles.Except(roles);

                await _userManager.AddToRolesAsync(user, addedRoles);

                await _userManager.RemoveFromRolesAsync(user, removedRoles);

                return RedirectToAction("Users");
            }

            return NotFound();
        }
    }
}
