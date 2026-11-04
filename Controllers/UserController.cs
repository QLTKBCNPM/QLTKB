

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using QLTKB.Data;

namespace QLTKB.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly QLTKBContext _context;

        public UserController(QLTKBContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        // Hiển thị thông tin cá nhân giảng viên và sinh viên
        [Authorize(Roles = "GiangVien, SinhVien")]
        public async Task<IActionResult> Profile()
        {
            var taiKhoanID = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(taiKhoanID, out int id))
            {
                return Unauthorized();
            }

            // Nếu là giảng viên
            if (User.IsInRole("GiangVien"))
            {
                var giangVien = await _context.GiangVien
                    .Include(gv => gv.TaiKhoan)
                    .FirstOrDefaultAsync(gv => gv.TaiKhoanID == id);

                if (giangVien == null)
                {
                    return NotFound("Không tìm thấy hồ sơ giảng viên.");
                }

                return View("ProfileGiangVien", giangVien);
            }

            // Nếu là sinh viên
            if (User.IsInRole("SinhVien"))
            {
                var sinhVien = await _context.SinhVien
                    .Include(sv => sv.TaiKhoan)
                    .FirstOrDefaultAsync(sv => sv.TaiKhoanID == id);

                if (sinhVien == null)
                {
                    return NotFound("Không tìm thấy hồ sơ sinh viên.");
                }

                return View("ProfileSinhVien", sinhVien);
            }

            return Forbid();
        }
    }
}

