

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLTKB.Data;
using QLTKB.Models;

namespace QLTKB.Controllers
{
    [Authorize(Roles = "Admin,AcademicStaff")]
    public class HocPhanController : Controller
    {
        private readonly QLTKBContext _context;

        public HocPhanController(QLTKBContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách học phần
        public async Task<IActionResult> Index()
        {
            var danhSach = await _context.HocPhan
                .OrderBy(h => h.HocPhanID)
                .ToListAsync();

            return View(danhSach);
        }

        // Mở trang thêm học phần
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // Lưu học phần mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HocPhan hocPhan)
        {
            bool trungMa = await _context.HocPhan
                .AnyAsync(h => h.MaHocPhan == hocPhan.MaHocPhan);

            if (trungMa)
            {
                ModelState.AddModelError(
                    "MaHocPhan",
                    "Mã học phần đã tồn tại."
                );
            }

            if (ModelState.IsValid)
            {
                hocPhan.TrangThai = true;

                _context.HocPhan.Add(hocPhan);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Thêm học phần thành công.";
                return RedirectToAction(nameof(Index));
            }

            return View(hocPhan);
        }

        // Mở trang sửa học phần
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var hocPhan = await _context.HocPhan
                .FirstOrDefaultAsync(h => h.HocPhanID == id);

            if (hocPhan == null)
                return NotFound();

            return View(hocPhan);
        }

        // Lưu thông tin sau khi sửa
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, HocPhan hocPhan)
        {
            if (id != hocPhan.HocPhanID)
                return NotFound();

            bool trungMa = await _context.HocPhan
                .AnyAsync(h => h.HocPhanID != id &&
                               h.MaHocPhan == hocPhan.MaHocPhan);

            if (trungMa)
            {
                ModelState.AddModelError(
                    "MaHocPhan",
                    "Mã học phần đã được sử dụng."
                );
            }

            if (ModelState.IsValid)
            {
                var duLieuCu = await _context.HocPhan
                    .FirstOrDefaultAsync(h => h.HocPhanID == id);

                if (duLieuCu == null)
                    return NotFound();

                duLieuCu.MaHocPhan = hocPhan.MaHocPhan;
                duLieuCu.TenHocPhan = hocPhan.TenHocPhan;
                duLieuCu.SoTinChi = hocPhan.SoTinChi;
                duLieuCu.SoTietLyThuyet = hocPhan.SoTietLyThuyet;
                duLieuCu.SoTietThucHanh = hocPhan.SoTietThucHanh;
                duLieuCu.MoTa = hocPhan.MoTa;
                duLieuCu.TrangThai = hocPhan.TrangThai;

                await _context.SaveChangesAsync();

                TempData["Success"] = "Cập nhật học phần thành công.";
                return RedirectToAction(nameof(Index));
            }

            return View(hocPhan);
        }

        // Mở trang xác nhận khóa học phần
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var hocPhan = await _context.HocPhan
                .FirstOrDefaultAsync(h => h.HocPhanID == id);

            if (hocPhan == null)
                return NotFound();

            return View(hocPhan);
        }

        // Xác nhận khóa học phần
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hocPhan = await _context.HocPhan
                .FirstOrDefaultAsync(h => h.HocPhanID == id);

            if (hocPhan == null)
                return NotFound();

            hocPhan.TrangThai = false;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Đã khóa học phần thành công.";
            return RedirectToAction(nameof(Index));
        }
    }
}
