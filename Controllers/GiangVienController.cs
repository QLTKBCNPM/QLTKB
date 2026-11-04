using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using QLTKB.Data;
using QLTKB.Models;

namespace QLTKB.Controllers
{
    [Authorize(Roles = "Admin")]
    public class GiangVienController : Controller
    {
        private readonly QLTKBContext _context;

        public GiangVienController(QLTKBContext context)
        {
            _context = context;
        }

        // Danh sách giảng viên
        public async Task<IActionResult> Index()
        {
            var danhSach = await _context.GiangVien
                .Include(gv => gv.TaiKhoan)
                .ToListAsync();

            return View(danhSach);
        }

        // Hiển thị form thêm giảng viên
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // Xử lý thêm giảng viên
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            string fullname,
            string username,
            string email,
            string password,
            string maGiangVien,
            string? khoa,
            string? boMon,
            string? hocVi,
            string? soDienThoai)
        {
            // Kiểm tra thông tin bắt buộc
            if (string.IsNullOrWhiteSpace(fullname) ||
                string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(maGiangVien))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin bắt buộc.";
                return View();
            }

            // Kiểm tra tên đăng nhập
            if (await _context.TaiKhoan.AnyAsync(
                t => t.TenDangNhap == username))
            {
                ViewBag.Error = "Tên đăng nhập đã tồn tại.";
                return View();
            }

            // Kiểm tra email
            if (await _context.TaiKhoan.AnyAsync(
                t => t.Email == email))
            {
                ViewBag.Error = "Email đã được sử dụng.";
                return View();
            }

            // Kiểm tra mã giảng viên
            if (await _context.GiangVien.AnyAsync(
                gv => gv.MaGiangVien == maGiangVien))
            {
                ViewBag.Error = "Mã giảng viên đã tồn tại.";
                return View();
            }

            // Kiểm tra vai trò Giảng viên
            if (!await _context.VaiTro.AnyAsync(
                v => v.VaiTroID == 5))
            {
                ViewBag.Error = "Không tìm thấy vai trò Giảng viên.";
                return View();
            }

            // Tạo tài khoản và hồ sơ trong cùng giao dịch
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                var taiKhoan = new TaiKhoan
                {
                    HoTen = fullname,
                    TenDangNhap = username,
                    Email = email,
                    MatKhau = password,
                    VaiTroID = 5,
                    TrangThai = true,
                    NgayTao = DateTime.Now
                };

                _context.TaiKhoan.Add(taiKhoan);
                await _context.SaveChangesAsync();

                var giangVien = new GiangVien
                {
                    TaiKhoanID = taiKhoan.TaiKhoanID,
                    MaGiangVien = maGiangVien,
                    Khoa = khoa,
                    BoMon = boMon,
                    HocVi = hocVi,
                    SoDienThoai = soDienThoai
                };

                _context.GiangVien.Add(giangVien);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                TempData["Success"] = "Thêm giảng viên thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                await transaction.RollbackAsync();
                ViewBag.Error = "Có lỗi xảy ra khi thêm giảng viên.";
                return View();
            }

        }
        // Hiển thị trang sửa giảng viên
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var giangVien = await _context.GiangVien
                .Include(gv => gv.TaiKhoan)
                .FirstOrDefaultAsync(gv => gv.GiangVienID == id);

            if (giangVien == null)
            {
                return NotFound();
            }

            return View(giangVien);
        }

        // Lưu thông tin giảng viên sau khi sửa
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            string fullname,
            string email,
            string maGiangVien,
            string? khoa,
            string? boMon,
            string? hocVi,
            string? soDienThoai)
        {
            var giangVien = await _context.GiangVien
                .Include(gv => gv.TaiKhoan)
                .FirstOrDefaultAsync(gv => gv.GiangVienID == id);

            if (giangVien == null)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(fullname) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(maGiangVien))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ các thông tin bắt buộc.";
                return View(giangVien);
            }

            bool trungEmail = await _context.TaiKhoan
                .AnyAsync(tk => tk.Email == email &&
                               tk.TaiKhoanID != giangVien.TaiKhoanID);

            bool trungMa = await _context.GiangVien
                .AnyAsync(gv => gv.MaGiangVien == maGiangVien &&
                               gv.GiangVienID != id);

            if (trungEmail || trungMa)
            {
                ViewBag.Error = "Email hoặc mã giảng viên đã tồn tại.";
                return View(giangVien);
            }

            giangVien.TaiKhoan!.HoTen = fullname;
            giangVien.TaiKhoan.Email = email;

            giangVien.MaGiangVien = maGiangVien;
            giangVien.Khoa = khoa;
            giangVien.BoMon = boMon;
            giangVien.HocVi = hocVi;
            giangVien.SoDienThoai = soDienThoai;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Cập nhật giảng viên thành công.";

            return RedirectToAction(nameof(Index));
        }
        // Xóa mềm giảng viên bằng cách khóa tài khoản
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var giangVien = await _context.GiangVien
                .Include(gv => gv.TaiKhoan)
                .FirstOrDefaultAsync(gv => gv.GiangVienID == id);

            if (giangVien == null)
            {
                return NotFound();
            }

            if (giangVien.TaiKhoan == null)
            {
                return NotFound();
            }

            giangVien.TaiKhoan.TrangThai = false;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Đã khóa tài khoản giảng viên thành công.";

            return RedirectToAction(nameof(Index));
        }
    }
}
