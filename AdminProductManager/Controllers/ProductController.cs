using AdminProductManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdminProductManager.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly ProductFirebaseService _service;

        public ProductController(IConfiguration configuration)
        {
            _service = new ProductFirebaseService(configuration);
        }

        public async Task<IActionResult> Index()
        {
            var products = await _service.GetAllAsync();
            return View(products);
        }

        public async Task<IActionResult> Details(string id)
        {
            if (id == null) return NotFound();
            var product = await _service.GetByIdAsync(id);
            if (product == null) return NotFound();
            return View(product);
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();
            var product = await _service.GetByIdAsync(id);
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Product product)
        {
            if (id != product.Id) return NotFound();
            if (ModelState.IsValid)
            {
                await _service.UpdateAsync(product);
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound();
            var product = await _service.GetByIdAsync(id);
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(Product product)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Dữ liệu nhập vào không hợp lệ. Vui lòng kiểm tra lại các trường thông tin.");
                return View(product);
            }
            // Gán giá trị mặc định cho Category và Size nếu null
            if (product.Category == null)
                product.Category = string.Empty;
            if (product.Size == null)
                product.Size = new List<int>();
            try
            {
                await _service.CreateAsync(product);
                TempData["SuccessMessage"] = "Thêm sản phẩm thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Lỗi khi tạo sản phẩm: {ex.Message}");
                return View(product);
            }
        }
    }
} 