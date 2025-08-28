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
    public IActionResult Create(Slider slider, IFormFile? ImageFile)
    {
        if (!ModelState.IsValid)
            return View();
        if (context.Sliders.Any(g => g.Title == slider.Title))
        {
            ModelState.AddModelError("Title", "This Title already exists");
            return View();
        }

        // Handle file upload
        if (ImageFile != null && ImageFile.Length > 0)
        {
            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "image", "products");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            string uniqueFileName = Guid.NewGuid().ToString() + "_" + ImageFile.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                ImageFile.CopyTo(fileStream);
            }

            slider.Image = uniqueFileName;
        }

        context.Sliders.Add(slider);
        context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Detail(int id)
    {
        var slider = context.Sliders.Find(id);
        if (slider == null) 
            return NotFound();
        return PartialView("_DetailPartial", slider);
    }

    public IActionResult Edit(int id)
    {
        Slider slider = context.Sliders.Find(id);
        if (slider == null)
            return NotFound();
        return View(slider);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Slider slider, IFormFile? ImageFile)
    {
        if (!ModelState.IsValid)
            return View(slider);
        var existSlide = context.Sliders.FirstOrDefault(g => g.Id == slider.Id);
        if (existSlide == null) return NotFound();
        if (context.Sliders.Any(g => g.Title == slider.Title && g.Id != slider.Id))
        {
            ModelState.AddModelError("Title", "This Title already exists");
            return View(slider);
        }

        // Handle file upload for edit
        if (ImageFile != null && ImageFile.Length > 0)
        {
            // Delete old image if exists
            if (!string.IsNullOrEmpty(existSlide.Image))
            {
                string oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "image", "products", existSlide.Image);
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }

            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "image", "products");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            string uniqueFileName = Guid.NewGuid().ToString() + "_" + ImageFile.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                ImageFile.CopyTo(fileStream);
            }

            existSlide.Image = uniqueFileName;
        }

        existSlide.Title = slider.Title;
        existSlide.Description = slider.Description;
        existSlide.ButtonText = slider.ButtonText;
        existSlide.ButtonUrl = slider.ButtonUrl;
        existSlide.Order = slider.Order;
        context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }
    public IActionResult Delete(int id)
    {
        Slider slider = context.Sliders.Find(id);
        if (slider == null)
            return NotFound();
        
        // Delete image file if exists
        if (!string.IsNullOrEmpty(slider.Image))
        {
            string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "image", "products", slider.Image);
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }
        }

        context.Sliders.Remove(slider);
        context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }
    
}