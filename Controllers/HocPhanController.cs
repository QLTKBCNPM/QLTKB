using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLTKB.Data;
using QLTKB.Models;

namespace QLTKB.Controllers
{
    [Authorize]
    public class HocPhanController : Controller
    {
        private readonly QLTKBContext _context;

        public HocPhanController(QLTKBContext context)
        {
            _context = context;
        }

        // GET: /HocPhan
        // Danh sách học phần + Tìm kiếm theo từ khóa
        public async Task<IActionResult> Index(string? search)
        {
            var query = _context.HocPhan.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                query = query.Where(hp => hp.MaHocPhan.Contains(search) || hp.TenHocPhan.Contains(search));
                ViewBag.CurrentSearch = search;
            }

            var list = await query.OrderBy(hp => hp.MaHocPhan).ToListAsync();
            return View(list);
        }

        // GET: /HocPhan/Create
        public IActionResult Create()
        {
            return View(new HocPhan());
        }

        // POST: /HocPhan/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HocPhan model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra trùng mã học phần
                bool exists = await _context.HocPhan
                    .AnyAsync(h => h.MaHocPhan.ToUpper() == model.MaHocPhan.Trim().ToUpper());
                if (exists)
                {
                    ModelState.AddModelError("MaHocPhan", "Mã học phần này đã tồn tại trong hệ thống.");
                    return View(model);
                }

                model.MaHocPhan = model.MaHocPhan.Trim().ToUpper();
                model.TenHocPhan = model.TenHocPhan.Trim();
                _context.HocPhan.Add(model);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Thêm mới học phần thành công!";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: /HocPhan/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var hocPhan = await _context.HocPhan.FindAsync(id);
            if (hocPhan == null) return NotFound();

            return View(hocPhan);
        }

        // POST: /HocPhan/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, HocPhan model)
        {
            if (id != model.HocPhanID) return NotFound();

            if (ModelState.IsValid)
            {
                // Kiểm tra xem mã học phần mới có bị trùng với học phần khác không
                bool exists = await _context.HocPhan
                    .AnyAsync(h => h.MaHocPhan.ToUpper() == model.MaHocPhan.Trim().ToUpper() && h.HocPhanID != id);
                if (exists)
                {
                    ModelState.AddModelError("MaHocPhan", "Mã học phần này đã được sử dụng bởi môn khác.");
                    return View(model);
                }

                try
                {
                    model.MaHocPhan = model.MaHocPhan.Trim().ToUpper();
                    model.TenHocPhan = model.TenHocPhan.Trim();
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật học phần thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.HocPhan.AnyAsync(e => e.HocPhanID == id))
                        return NotFound();
                    else
                        throw;
                }
            }
            return View(model);
        }

        // POST: /HocPhan/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var hocPhan = await _context.HocPhan.FindAsync(id);
            if (hocPhan != null)
            {
                _context.HocPhan.Remove(hocPhan);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Đã xóa học phần {hocPhan.TenHocPhan}!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

