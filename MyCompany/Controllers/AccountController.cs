using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyCompany.Models;

namespace MyCompany.Controllers
{
    [Authorize] // Для данного контроллера действует правило авторизации
    public class AccountController : Controller 
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;

        public AccountController(UserManager<IdentityUser> userMgr, SignInManager<IdentityUser> signinMgr) // прокидываем зависимости
        {
            userManager = userMgr;
            signInManager = signinMgr;
        }

        [AllowAnonymous] // чтобы залогиниться надо быть анонимным
        public IActionResult Login(string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;
            return View(new LoginViewModel()); // Передаем во Вью нашу модель LoginViewModel
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl) // при нажатии в форме кнопки "Войти" введенные данные передаются сюда и тут мы работаем с ними
        {
            if (ModelState.IsValid)
            {
                IdentityUser user = await userManager.FindByNameAsync(model.UserName);
                if (user != null) // если логин найден
                {
                    await signInManager.SignOutAsync(); // выход
                    Microsoft.AspNetCore.Identity.SignInResult result =
                        await signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false); // пытаемся войти по паролю
                    if (result.Succeeded)
                    {
                        return Redirect(returnUrl ?? "/"); // если не получилось отправляем назад или на главную
                    }
                }
                ModelState.AddModelError(nameof(LoginViewModel.UserName), "Неверный логин или пароль"); // если логин не найден
            }

            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
