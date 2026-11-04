
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLTKB.Data;
using QLTKB.Models;

namespace QLTKB.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SinhVienController : Controller
    {
        private readonly QLTKBContext _context;

        public SinhVienController(QLTKBContext context)
        {
            _context = context;
        }

        // Danh sách sinh viên
        public async Task<IActionResult> Index()
        {
            var danhSach = await _context.SinhVien
                .Include(sv => sv.TaiKhoan)
                .ToListAsync();

            return View(danhSach);
        }

        // Hiển thị trang thêm sinh viên
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // Lưu sinh viên mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            string fullname,
            string username,
            string email,
            string password,
            string maSinhVien,
            string? lop,
            string? nganh,
            string? khoaHoc,
            string? soDienThoai)
        {
            if (string.IsNullOrWhiteSpace(fullname) ||
                string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(maSinhVien))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin bắt buộc.";
                return View();
            }

            bool trungTaiKhoan = await _context.TaiKhoan.AnyAsync(
                tk => tk.TenDangNhap == username || tk.Email == email);

            bool trungMa = await _context.SinhVien.AnyAsync(
                sv => sv.MaSinhVien == maSinhVien);

            if (trungTaiKhoan || trungMa)
            {
                ViewBag.Error = "Tên đăng nhập, email hoặc mã sinh viên đã tồn tại.";
                return View();
            }

            var vaiTro = await _context.VaiTro
                .FirstOrDefaultAsync(v => v.VaiTroID == 6);

            if (vaiTro == null)
            {
                ViewBag.Error = "Không tìm thấy vai trò Sinh viên.";
                return View();
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
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

                _context.TaiKhoan.Add(taiKhoan);
                await _context.SaveChangesAsync();

                var sinhVien = new SinhVien
                {
                    TaiKhoanID = taiKhoan.TaiKhoanID,
                    MaSinhVien = maSinhVien,
                    Lop = lop,
                    Nganh = nganh,
                    KhoaHoc = khoaHoc,
                    SoDienThoai = soDienThoai
                };

                _context.SinhVien.Add(sinhVien);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                TempData["Success"] = "Thêm sinh viên thành công.";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                await transaction.RollbackAsync();
                ViewBag.Error = "Có lỗi xảy ra khi thêm sinh viên.";
                return View();
            }
        }

        // Hiển thị trang sửa sinh viên
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var sinhVien = await _context.SinhVien
                .Include(sv => sv.TaiKhoan)
                .FirstOrDefaultAsync(sv => sv.SinhVienID == id);

            if (sinhVien == null)
            {
                return NotFound();
            }

            return View(sinhVien);
        }

        // Lưu thông tin sinh viên sau khi sửa
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            string fullname,
            string email,
            string maSinhVien,
            string? lop,
            string? nganh,
            string? khoaHoc,
            string? soDienThoai)
        {
            var sinhVien = await _context.SinhVien
                .Include(sv => sv.TaiKhoan)
                .FirstOrDefaultAsync(sv => sv.SinhVienID == id);

            if (sinhVien == null)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(fullname) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(maSinhVien))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin bắt buộc.";
                return View(sinhVien);
            }

            bool trungEmail = await _context.TaiKhoan
                .AnyAsync(tk => tk.Email == email &&
                               tk.TaiKhoanID != sinhVien.TaiKhoanID);

            bool trungMa = await _context.SinhVien
                .AnyAsync(sv => sv.MaSinhVien == maSinhVien &&
                               sv.SinhVienID != id);

            if (trungEmail || trungMa)
            {
                ViewBag.Error = "Email hoặc mã sinh viên đã tồn tại.";
                return View(sinhVien);
            }

            sinhVien.TaiKhoan!.HoTen = fullname;
            sinhVien.TaiKhoan.Email = email;

            sinhVien.MaSinhVien = maSinhVien;
            sinhVien.Lop = lop;
            sinhVien.Nganh = nganh;
            sinhVien.KhoaHoc = khoaHoc;
            sinhVien.SoDienThoai = soDienThoai;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Cập nhật sinh viên thành công.";

            return RedirectToAction(nameof(Index));
        }

        // Khóa tài khoản sinh viên
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var sinhVien = await _context.SinhVien
                .Include(sv => sv.TaiKhoan)
                .FirstOrDefaultAsync(sv => sv.SinhVienID == id);

            if (sinhVien == null || sinhVien.TaiKhoan == null)
            {
                return NotFound();
            }

            sinhVien.TaiKhoan.TrangThai = false;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Đã khóa tài khoản sinh viên thành công.";

            return RedirectToAction(nameof(Index));
        }


    }
}

