using Interconn.Models;
using Interconn.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Interconn.Controllers
{
    [AllowAnonymous]
    public class RegisterController : Controller
    {       
        private readonly InterconnDbContext _context;

        public RegisterController(InterconnDbContext context)
        {
            _context = context;
        }


        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM value)
        {
            if (!ModelState.IsValid) { return View(value); }

            //把Email轉成小寫 Gmail的帳號是沒有分大小寫的
            string email = value.Email.Trim().ToLower();
            //驗證用戶是否已經存在
            User ExistingUser = await _context.Users
                                     .FirstOrDefaultAsync(x => x.Email == email);

            if (ExistingUser != null)
            {
                ModelState.AddModelError("", "用戶已存在");
                return View(value);
            }

            try
            {
                //將post過來的資料對應的資料庫的欄位
                User user = new User()
                {
                    Name = value.Name,
                    Email = value.Email,
                    //密碼不存明文在資料庫,所以先用BCrypt加密,再存進資料庫
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(value.Password, 12),
                    CreatedTime = DateTime.Now
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                TempData["Message"] = "註冊成功 請再次輸入帳號密碼";

                return RedirectToAction("Index", "Login");
            }
            catch
            {
                ModelState.AddModelError("", "發生錯誤，請再試一次");

                return View(value);
            }

        }
    }
}
