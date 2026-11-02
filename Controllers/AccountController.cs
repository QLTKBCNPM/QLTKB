

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using QLTKB.Data;
using QLTKB.Models;

namespace QLTKB.Controllers
{
    public class AccountController : Controller
    {
        private readonly QLTKBContext _context;

        public AccountController(QLTKBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(
            string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin.";
                return View();
            }

            // Tìm tài khoản trực tiếp trong CSDL
            var taiKhoan = await _context.TaiKhoan
                .Include(t => t.VaiTro)
                .FirstOrDefaultAsync(t =>
                    t.TenDangNhap == username);

            if (taiKhoan == null || !taiKhoan.TrangThai)
            {
                ViewBag.Error =
                    "Tên đăng nhập hoặc mật khẩu không đúng.";
                return View();
            }

            // So sánh mật khẩu trực tiếp
            if (taiKhoan.MatKhau != password)
            {
                ViewBag.Error =
                    "Tên đăng nhập hoặc mật khẩu không đúng.";
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    taiKhoan.TaiKhoanID.ToString()
                ),

                new Claim(
                    ClaimTypes.Name,
                    taiKhoan.TenDangNhap
                ),

                new Claim(
                    ClaimTypes.Role,
                    taiKhoan.VaiTro?.TenVaiTro ?? ""
                ),

                new Claim("HoTen", taiKhoan.HoTen)
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal
            );

            // Chuyển trang theo vai trò tài khoản
            if (taiKhoan.VaiTro?.TenVaiTro == "Admin")
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Index", "User");
            }
        }
        
    [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            return RedirectToAction("Index", "Home");
        }
    }
}
