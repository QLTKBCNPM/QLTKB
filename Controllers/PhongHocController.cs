
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLTKB.Data;
using QLTKB.Models;

namespace QLTKB.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PhongHocController : Controller
    {
        private readonly QLTKBContext _context;

        public PhongHocController(QLTKBContext context)
        {
            _context = context;
        }

        // Danh sách phòng học
        public async Task<IActionResult> Index()
        {
            var danhSach = await _context.PhongHoc
                .OrderBy(p => p.MaPhong)
                .ToListAsync();

            return View(danhSach);
        }

        // Thêm phòng học
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PhongHoc phongHoc)
        {
            bool trungMa = await _context.PhongHoc
                .AnyAsync(p => p.MaPhong == phongHoc.MaPhong);

            if (trungMa)
            {
                ModelState.AddModelError(
                    "MaPhong", "Mã phòng đã tồn tại.");
            }

            if (!ModelState.IsValid)
            {
                return View(phongHoc);
            }

            _context.PhongHoc.Add(phongHoc);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Thêm phòng học thành công.";
            return RedirectToAction(nameof(Index));
        }

        // Sửa thông tin phòng học
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var phongHoc = await _context.PhongHoc
                .FindAsync(id);

            if (phongHoc == null)
            {
                return NotFound();
            }

            return View(phongHoc);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id, PhongHoc phongHoc)
        {
            if (id != phongHoc.PhongHocID)
            {
                return NotFound();
            }

            bool trungMa = await _context.PhongHoc
                .AnyAsync(p =>
                    p.MaPhong == phongHoc.MaPhong &&
                    p.PhongHocID != id);

            if (trungMa)
            {
                ModelState.AddModelError(
                    "MaPhong", "Mã phòng đã tồn tại.");
            }

            if (!ModelState.IsValid)
            {
                return View(phongHoc);
            }

            _context.PhongHoc.Update(phongHoc);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Cập nhật phòng học thành công.";
            return RedirectToAction(nameof(Index));
        }

        // Xóa phòng học

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var phongHoc = await _context.PhongHoc
                .FindAsync(id);

            if (phongHoc == null)
            {
                return NotFound();
            }

            phongHoc.TrangThai = false;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Đã ngừng hoạt động phòng học.";
            return RedirectToAction(nameof(Index));
        }

        // Khôi phục phòng học
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(int id)
        {
            var phongHoc = await _context.PhongHoc
                .FindAsync(id);

            if (phongHoc == null)
            {
                return NotFound();
            }

            phongHoc.TrangThai = true;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Đã khôi phục phòng học.";
            return RedirectToAction(nameof(Index));
        }


    }
}
