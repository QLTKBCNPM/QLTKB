

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
        public async Task<IActionResult> Register(
     string fullname,
     string username,
     string email,
     string password,
     string confirmPassword)
        {
            // Kiểm tra thông tin nhập vào
            if (string.IsNullOrWhiteSpace(fullname) ||
                string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(confirmPassword))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin.";
                return View();
            }

            // Kiểm tra mật khẩu xác nhận
            if (password != confirmPassword)
            {
                ViewBag.Error = "Mật khẩu xác nhận không khớp.";
                return View();
            }

            // Kiểm tra tên đăng nhập đã tồn tại
            bool trungTenDangNhap = await _context.TaiKhoan
                .AnyAsync(t => t.TenDangNhap == username);

            if (trungTenDangNhap)
            {
                ViewBag.Error = "Tên đăng nhập đã tồn tại.";
                return View();
            }

            // Kiểm tra email đã tồn tại
            bool trungEmail = await _context.TaiKhoan
                .AnyAsync(t => t.Email == email);

            if (trungEmail)
            {
                ViewBag.Error = "Email đã được sử dụng.";
                return View();
            }

            // Kiểm tra vai trò Sinh viên
            var vaiTro = await _context.VaiTro
                .FirstOrDefaultAsync(v => v.VaiTroID == 6);

            if (vaiTro == null)
            {
                ViewBag.Error = "Không tìm thấy vai trò Sinh viên.";
                return View();
            }

            // Tạo tài khoản mới
            var taiKhoan = new TaiKhoan
            {
                HoTen = fullname,
                TenDangNhap = username,
                Email = email,
                MatKhau = password,
                VaiTroID = 6,
                TrangThai = true,
                NgayTao = DateTime.Now
            };

            // Lưu trực tiếp vào CSDL
            _context.TaiKhoan.Add(taiKhoan);
            await _context.SaveChangesAsync();

            ViewBag.Success = "Đăng ký thành công! Bạn có thể đăng nhập.";
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            return RedirectToAction("Login", "Account");
        }
    }
}
