using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok.Data;
using Pustok.Models;

namespace Pustok.Areas.Manage.Controllers;

[Area("Manage")]
public class AuthorController(AppDbContext context) : Controller
{
    public IActionResult Index()
    {
        var authors = context.Authors
            .Include(a => a.Books)
            .ToList();
        return View(authors);
    }
    
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Author author)
    {
        if (!ModelState.IsValid) return View(author);
        
        if (context.Authors.Any(a => a.Name == author.Name))
        {
            ModelState.AddModelError("Name", "This author name already exists");
            return View(author);
        }
        
        context.Authors.Add(author);
        context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Detail(int id)
    {
        var author = context.Authors
            .Include(a => a.Books)
            .ThenInclude(b => b.Genre)
            .FirstOrDefault(a => a.Id == id);
        
        if (author == null) return NotFound();
        return View(author);
    }

    public IActionResult Edit(int id)
    {
        var author = context.Authors.Find(id);
        if (author == null) return NotFound();
        return View(author);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Author author)
    {
        if (!ModelState.IsValid) return View(author);
        
        var existAuthor = context.Authors.Find(author.Id);
        if (existAuthor == null) return NotFound();
        
        if (context.Authors.Any(a => a.Name == author.Name && a.Id != author.Id))
        {
            ModelState.AddModelError("Name", "This author name already exists");
            return View(author);
        }
        
        existAuthor.Name = author.Name;
        context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Delete(int id)
    {
        var author = context.Authors
            .Include(a => a.Books)
            .FirstOrDefault(a => a.Id == id);
        
        if (author == null) return NotFound();
        
        if (author.Books?.Any() == true)
        {
            TempData["Error"] = "Cannot delete author with existing books";
            return RedirectToAction(nameof(Index));
        }
        
        context.Authors.Remove(author);
        context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }
}