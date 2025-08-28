using Microsoft.AspNetCore.Mvc;
using Pustok.Data;
using Pustok.Models;

namespace Pustok.Areas.Manage.Controllers;

[Area("Manage")]
public class SliderController(AppDbContext context) : Controller
{
    public IActionResult Index()
    {
        return View(context.Sliders.OrderBy(s => s.Order).ToList());
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Slider slider, IFormFile? ImageFile)
    {
        if (!ModelState.IsValid)
            return View(slider);
            
        if (context.Sliders.Any(g => g.Title == slider.Title))
        {
            ModelState.AddModelError("Title", "This title already exists");
            return View(slider);
        }

        if (ImageFile != null && ImageFile.Length > 0)
        {
            slider.Image = await SaveImageAsync(ImageFile);
        }

        context.Sliders.Add(slider);
        await context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Detail(int id)
    {
        var slider = context.Sliders.Find(id);
        if (slider == null) 
            return NotFound();
        return View(slider);
    }

    public IActionResult Edit(int id)
    {
        var slider = context.Sliders.Find(id);
        if (slider == null)
            return NotFound();
        return View(slider);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Slider slider, IFormFile? ImageFile)
    {
        if (!ModelState.IsValid)
            return View(slider);
            
        var existSlide = context.Sliders.FirstOrDefault(g => g.Id == slider.Id);
        if (existSlide == null) return NotFound();
        
        if (context.Sliders.Any(g => g.Title == slider.Title && g.Id != slider.Id))
        {
            ModelState.AddModelError("Title", "This title already exists");
            return View(slider);
        }

        // Handle file upload for edit
        if (ImageFile != null && ImageFile.Length > 0)
        {
            // Delete old image if exists
            if (!string.IsNullOrEmpty(existSlide.Image))
            {
                DeleteImage(existSlide.Image);
            }
            
            existSlide.Image = await SaveImageAsync(ImageFile);
        }

        existSlide.Title = slider.Title;
        existSlide.Description = slider.Description;
        existSlide.ButtonText = slider.ButtonText;
        existSlide.ButtonUrl = slider.ButtonUrl;
        existSlide.Order = slider.Order;
        existSlide.UpdatedAt = DateTime.Now;
        
        await context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var slider = context.Sliders.Find(id);
        if (slider == null)
            return NotFound();
        
        // Delete image file if exists
        if (!string.IsNullOrEmpty(slider.Image))
        {
            DeleteImage(slider.Image);
        }

        context.Sliders.Remove(slider);
        await context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private async Task<string> SaveImageAsync(IFormFile imageFile)
    {
        var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "image", "products");
        if (!Directory.Exists(uploadsPath))
        {
            Directory.CreateDirectory(uploadsPath);
        }

        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
        var filePath = Path.Combine(uploadsPath, fileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await imageFile.CopyToAsync(fileStream);
        }

        return fileName;
    }

    private void DeleteImage(string fileName)
    {
        if (string.IsNullOrEmpty(fileName)) return;

        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "image", "products", fileName);
        if (System.IO.File.Exists(filePath))
        {
            System.IO.File.Delete(filePath);
        }
    }
}