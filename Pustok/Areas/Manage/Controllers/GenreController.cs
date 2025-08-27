using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok.Data;
using Pustok.Models;
namespace Pustok.App.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class GenreController(AppDbContext pustokDbContext) : Controller
    {
        public IActionResult Index()
        {
            var genres = pustokDbContext.Genres.ToList();
            return View(genres);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Genre genre)
        {
            if (!ModelState.IsValid)
                return View();
            if (pustokDbContext.Genres.Any(g => g.Name == genre.Name))
            {
                ModelState.AddModelError("Name", "This genre already exists");
                return View();
            }

            pustokDbContext.Genres.Add(genre);
            pustokDbContext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        
        public IActionResult Delete(int id)
        {
            var genre = pustokDbContext.Genres.Find(id);
            if (genre == null) return NotFound();
            pustokDbContext.Genres.Remove(genre);
            pustokDbContext.SaveChanges();
            return Ok();
        }

        public IActionResult Edit(int id)
        {
            var genre = pustokDbContext.Genres.Find(id);
            if (genre == null) return NotFound();
            return View(genre);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Genre genre)
        {
            if (!ModelState.IsValid)
                return View(genre);
            var existGenre = pustokDbContext.Genres.FirstOrDefault(g => g.Id == genre.Id);
            if (existGenre == null) return NotFound();
            if (pustokDbContext.Genres.Any(g => g.Name == genre.Name && g.Id != genre.Id))
            {
                ModelState.AddModelError("Name", "This genre already exists");
                return View(genre);
            }
            existGenre.Name = genre.Name;
            pustokDbContext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}

