using GCMAP.Areas.Identity.Data;
using GCMAP.Services;
using GCMAP.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GCMAP.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<GCMAPUser> _userManager;
        private readonly SignInManager<GCMAPUser> _signInManager;
        private readonly IConfiguration _configuration;
        
        //Инжектим необходимые для Identity сервисы и конфигурацию
        public AccountController(UserManager<GCMAPUser> userManager, SignInManager<GCMAPUser> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        //Возвращаем прдеставление регистрации
        public IActionResult Register()
        {
            return View();
        }

        //Обрабатываем Post-запрос регистраци
        //в этом же запросе отправляется код подтверждения e-mail
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            //Валидация модели
            if (ModelState.IsValid)
            {
                //Создаем юзера, что будет зарегистрирован
                GCMAPUser user = new GCMAPUser
                {
                    UserName = model.Email,
                    NickName = model.NickName,
                    Email = model.Email
                };
                // добавляем пользователя
                var result = await _userManager.CreateAsync(user, model.Password);
                //Необходимый для подтверждения эмэйла код
                if (result.Succeeded)
                {
                    // генерация токена для пользователя
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Action(
                        "ConfirmEmail",
                        "Account",
                        new { userId = user.Id, code = code },
                        protocol: HttpContext.Request.Scheme);
                    EmailService emailService = new EmailService();
                    string subject = "Подтвердите ваш аккаунт";
                    string message = $"Подтвердите регистрацию, перейдя по ссылке: <a href='{callbackUrl}'>link</a>";
                    await emailService.SendEmailAsync(model.Email, subject, message, _configuration);
                    return View("EmailSended");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }

        //Действие, по которому переходит пользователь,
        //желая подтвердить эмэйл, ссылка на ConfirmEmail
        //приходит в callback-url сгенерированном в [HttpPost]Register
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            //Проверка значений из запроса
            if (userId == null || code == null)
            {
                return View("Failed");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View("Failed");
            }
            //Вызов подтерждения e-mail
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
                return View("OK");
            else
                return View("Failed");
        }

        //Метод, возвращающий представление логина
        //return url - ссылка по которой вернется пользователь после успешного логина
        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        //Обрабатываем Post-запрос логина
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            //Валидация модели
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Email);
                if (user != null)
                {
                    // проверяем, подтвержден ли email
                    if (!await _userManager.IsEmailConfirmedAsync(user))
                    {
                        ModelState.AddModelError(string.Empty, "Вы не подтвердили свой email, проверьте почту");
                        return View(model);
                    }
                }

                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    // проверяем, принадлежит ли URL приложению
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Непраивльный пароль или (и) логин");
                }
            }
            return View(model);
        }

        //метод выхода из системы
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // удаляем аутентификационные куки
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        //доступ к ресурсу запрещен (смотреть IdentityHostingStartUp)
        public IActionResult Denied() => StatusCode(403);

        //Обрабатываем переход в личный кабинет
        [Authorize]
        [Route("/Account")]
        [Route("/Account/Cabinet")]
        public IActionResult Cabinet()
        {
            return View();
        }
    }
}
