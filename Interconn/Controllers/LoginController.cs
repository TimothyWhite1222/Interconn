using Interconn.Models;
using Interconn.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Interconn.Controllers
{ 
    [AllowAnonymous]
    public class LoginController : Controller
    {
        //資料庫注入
        private readonly InterconnDbContext _context;

        public LoginController(InterconnDbContext context)
        {
            _context = context;
        }


        public IActionResult Login()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM value)
        {
            //驗證傳進來的VM
            if (!ModelState.IsValid)
            {
                return View(value);
            }

            //驗證帳號 從資料庫抓帳號
            User user = await _context.Users
                              .FirstOrDefaultAsync(x => x.Email == value.Email);

            if (user == null) 
            {
                ModelState.AddModelError("","帳號錯誤或不存在");
                return View(value);
            }

            //因為密碼是存Hash值 所以要用BCrypt去驗證
            bool PWisValid = BCrypt.Net.BCrypt.Verify(value.Password, user.PasswordHash);

            if (PWisValid) 
            {
                //建立Claim可存放使用者資訊
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Email, user.Email),                   
                };

                //建立登入者的資料包
                var claimsIdentity = new ClaimsIdentity(
                                     claims, CookieAuthenticationDefaults.AuthenticationScheme);

                //把ClaimsPrincipal資料寫到瀏覽器Cookie裡
                await HttpContext.SignInAsync(
                      CookieAuthenticationDefaults.AuthenticationScheme,
                      new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "密碼錯誤");
                return View(value);
            }   
        }

        //登出使用Post+token驗證更安全 有改變伺服器行為的建議都用Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            TempData["Message"] = "登出成功";

            return RedirectToAction("Index");
        }
    }
}
