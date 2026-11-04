
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLTKB.Data;
using QLTKB.Models;
using System.Security.Claims;

namespace QLTKB.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TaiKhoanController : Controller
    {
        private readonly QLTKBContext _context;

        public TaiKhoanController(QLTKBContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách tài khoản
        public async Task<IActionResult> Index()
        {
            var danhSachTaiKhoan = await _context.TaiKhoan
                .Include(t => t.VaiTro)
                .OrderBy(t => t.TaiKhoanID)
                .ToListAsync();

            return View(danhSachTaiKhoan);
        }

        // Nạp danh sách vai trò vào dropdown
        private async Task NapDanhSachVaiTro(int? vaiTroID = null)
        {
            var danhSachVaiTro = await _context.VaiTro
                .OrderBy(v => v.VaiTroID)
                .ToListAsync();

            ViewBag.VaiTroID = new SelectList(
                danhSachVaiTro,
                "VaiTroID",
                "TenVaiTro",
                vaiTroID
            );
        }

        // Mở trang thêm tài khoản
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await NapDanhSachVaiTro();
            return View();
        }

        // Lưu tài khoản mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            string HoTen,
            string TenDangNhap,
            string Email,
            string MatKhau,
            int VaiTroID)
        {
            HoTen = HoTen?.Trim() ?? "";
            TenDangNhap = TenDangNhap?.Trim() ?? "";
            Email = Email?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(HoTen) ||
                string.IsNullOrWhiteSpace(TenDangNhap) ||
                string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(MatKhau))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin.";
                await NapDanhSachVaiTro(VaiTroID);
                return View();
            }

            bool trungTenDangNhap = await _context.TaiKhoan
                .AnyAsync(t => t.TenDangNhap == TenDangNhap);

            bool trungEmail = await _context.TaiKhoan
                .AnyAsync(t => t.Email == Email);

            if (trungTenDangNhap || trungEmail)
            {
                ViewBag.Error = "Tên đăng nhập hoặc email đã tồn tại.";
                await NapDanhSachVaiTro(VaiTroID);
                return View();
            }

            bool vaiTroTonTai = await _context.VaiTro
                .AnyAsync(v => v.VaiTroID == VaiTroID);

            if (!vaiTroTonTai)
            {
                ViewBag.Error = "Vai trò không hợp lệ.";
                await NapDanhSachVaiTro();
                return View();
            }

            var taiKhoan = new TaiKhoan
            {
                HoTen = HoTen,
                TenDangNhap = TenDangNhap,
                Email = Email,
                MatKhau = MatKhau,
                VaiTroID = VaiTroID,
                TrangThai = true,
                NgayTao = DateTime.Now
            };

            _context.TaiKhoan.Add(taiKhoan);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Mở trang sửa tài khoản
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var taiKhoan = await _context.TaiKhoan
                .FirstOrDefaultAsync(t => t.TaiKhoanID == id);

            if (taiKhoan == null)
                return NotFound();

            await NapDanhSachVaiTro(taiKhoan.VaiTroID);
            return View(taiKhoan);
        }

        // Lưu thông tin sau khi sửa
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int TaiKhoanID,
            string HoTen,
            string TenDangNhap,
            string Email,
            int VaiTroID,
            bool TrangThai)
        {
            var taiKhoan = await _context.TaiKhoan
                .FirstOrDefaultAsync(t => t.TaiKhoanID == TaiKhoanID);

            if (taiKhoan == null)
                return NotFound();

            HoTen = HoTen?.Trim() ?? "";
            TenDangNhap = TenDangNhap?.Trim() ?? "";
            Email = Email?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(HoTen) ||
                string.IsNullOrWhiteSpace(TenDangNhap) ||
                string.IsNullOrWhiteSpace(Email))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin.";
                await NapDanhSachVaiTro(VaiTroID);
                return View(taiKhoan);
            }

            bool trungTenDangNhap = await _context.TaiKhoan
                .AnyAsync(t => t.TaiKhoanID != TaiKhoanID &&
                               t.TenDangNhap == TenDangNhap);

            bool trungEmail = await _context.TaiKhoan
                .AnyAsync(t => t.TaiKhoanID != TaiKhoanID &&
                               t.Email == Email);

            if (trungTenDangNhap || trungEmail)
            {
                ViewBag.Error = "Tên đăng nhập hoặc email đã được tài khoản khác sử dụng.";
                await NapDanhSachVaiTro(VaiTroID);
                return View(taiKhoan);
            }

            bool vaiTroTonTai = await _context.VaiTro
                .AnyAsync(v => v.VaiTroID == VaiTroID);

            if (!vaiTroTonTai)
            {
                ViewBag.Error = "Vai trò không hợp lệ.";
                await NapDanhSachVaiTro();
                return View(taiKhoan);
            }

            // Không cho Admin tự khóa tài khoản đang đăng nhập
            var currentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (currentId == TaiKhoanID.ToString() && !TrangThai)
            {
                ViewBag.Error = "Bạn không thể khóa tài khoản đang đăng nhập.";
                await NapDanhSachVaiTro(VaiTroID);
                return View(taiKhoan);
            }

            taiKhoan.HoTen = HoTen;
            taiKhoan.TenDangNhap = TenDangNhap;
            taiKhoan.Email = Email;
            taiKhoan.VaiTroID = VaiTroID;
            taiKhoan.TrangThai = TrangThai;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Mở trang xác nhận xóa (khóa) tài khoản
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var taiKhoan = await _context.TaiKhoan
                .Include(t => t.VaiTro)
                .FirstOrDefaultAsync(t => t.TaiKhoanID == id);

            if (taiKhoan == null)
                return NotFound();

            var currentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (currentId == id.ToString())
                return BadRequest("Không thể khóa tài khoản đang đăng nhập.");

            return View(taiKhoan);
        }

        // Xác nhận xóa (khóa) tài khoản
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var taiKhoan = await _context.TaiKhoan
                .FirstOrDefaultAsync(t => t.TaiKhoanID == id);

            if (taiKhoan == null)
                return NotFound();

            var currentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (currentId == id.ToString())
                return BadRequest("Không thể khóa tài khoản đang đăng nhập.");

            taiKhoan.TrangThai = false;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}