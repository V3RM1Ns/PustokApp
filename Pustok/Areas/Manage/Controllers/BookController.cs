using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok.Data;
using Pustok.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Pustok.Areas.Manage.Controllers;

[Area("Manage")]
public class BookController(AppDbContext context) : Controller
{
    public IActionResult Index()
    {
        var books = context.Books
            .Include(b => b.BookImages)
            .Include(b => b.Author)
            .Include(b => b.Genre)
            .Include(b => b.BookTags)
            .ToList();
        return View(books);
    }

    public IActionResult Create()
    {
        ViewBag.Authors = new SelectList(context.Authors, "Id", "Name");
        ViewBag.Genres = new SelectList(context.Genres, "Id", "Name");
        ViewBag.Tags = context.BookTags.ToList();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Book book, IFormFile? MainImageFile, IFormFile? HoverImageFile, List<IFormFile>? ImageFiles, List<int>? TagIds)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Authors = new SelectList(context.Authors, "Id", "Name");
            ViewBag.Genres = new SelectList(context.Genres, "Id", "Name");
            ViewBag.Tags = context.BookTags.ToList();
            return View(book);
        }

        if (context.Books.Any(b => b.Title == book.Title))
        {
            ModelState.AddModelError("Title", "This book title already exists");
            ViewBag.Authors = new SelectList(context.Authors, "Id", "Name");
            ViewBag.Genres = new SelectList(context.Genres, "Id", "Name");
            ViewBag.Tags = context.BookTags.ToList();
            return View(book);
        }

        // Main Image Upload
        if (MainImageFile != null)
        {
            book.MainImage = await SaveImageAsync(MainImageFile);
        }

        // Hover Image Upload
        if (HoverImageFile != null)
        {
            book.HoverImage = await SaveImageAsync(HoverImageFile);
        }

        context.Books.Add(book);
        await context.SaveChangesAsync();

        // Save additional images
        if (ImageFiles != null && ImageFiles.Any())
        {
            foreach (var imageFile in ImageFiles)
            {
                if (imageFile != null)
                {
                    var imagePath = await SaveImageAsync(imageFile);
                    var bookImg = new BookImg
                    {
                        BookId = book.Id,
                        Image = imagePath
                    };
                    context.BookImages.Add(bookImg);
                }
            }
        }

        // Save tags
        if (TagIds != null && TagIds.Any())
        {
            foreach (var tagId in TagIds)
            {
                var bookTag = new BookTag
                {
                    BookId = book.Id,
                    TagId = tagId
                };
                context.BookTags.Add(bookTag);
            }
        }

        await context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Detail(int id)
    {
        var book = context.Books
            .Include(b => b.Author)
            .Include(b => b.Genre)
            .Include(b => b.BookImages)
            .Include(b => b.BookTags)
            .ThenInclude(bt => bt.Tag)
            .FirstOrDefault(b => b.Id == id);

        if (book == null) return NotFound();
        return View(book);
    }

    public IActionResult Edit(int id)
    {
        var book = context.Books
            .Include(b => b.BookImages)
            .Include(b => b.BookTags)
            .FirstOrDefault(b => b.Id == id);

        if (book == null) return NotFound();

        ViewBag.Authors = new SelectList(context.Authors, "Id", "Name", book.AuthorId);
        ViewBag.Genres = new SelectList(context.Genres, "Id", "Name", book.GenreId);
        ViewBag.Tags = context.BookTags.ToList();
        ViewBag.SelectedTags = book.BookTags.Select(bt => bt.TagId).ToList();
        return View(book);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Book book, IFormFile? MainImageFile, IFormFile? HoverImageFile, List<IFormFile>? ImageFiles, List<int>? TagIds)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Authors = new SelectList(context.Authors, "Id", "Name", book.AuthorId);
            ViewBag.Genres = new SelectList(context.Genres, "Id", "Name", book.GenreId);
            ViewBag.Tags = context.BookTags.ToList();
            return View(book);
        }

        var existingBook = context.Books
            .Include(b => b.BookImages)
            .Include(b => b.BookTags)
            .FirstOrDefault(b => b.Id == book.Id);

        if (existingBook == null) return NotFound();

        if (context.Books.Any(b => b.Title == book.Title && b.Id != book.Id))
        {
            ModelState.AddModelError("Title", "This book title already exists");
            ViewBag.Authors = new SelectList(context.Authors, "Id", "Name", book.AuthorId);
            ViewBag.Genres = new SelectList(context.Genres, "Id", "Name", book.GenreId);
            ViewBag.Tags = context.BookTags.ToList();
            return View(book);
        }

        // Update basic properties
        existingBook.Title = book.Title;
        existingBook.Description = book.Description;
        existingBook.AuthorId = book.AuthorId;
        existingBook.GenreId = book.GenreId;
        existingBook.Price = book.Price;
        existingBook.DiscountPrice = book.DiscountPrice;
        existingBook.InStock = book.InStock;
        existingBook.IsFeatured = book.IsFeatured;
        existingBook.IsNew = book.IsNew;

        // Update Main Image
        if (MainImageFile != null)
        {
            if (!string.IsNullOrEmpty(existingBook.MainImage))
            {
                DeleteImage(existingBook.MainImage);
            }
            existingBook.MainImage = await SaveImageAsync(MainImageFile);
        }

        // Update Hover Image
        if (HoverImageFile != null)
        {
            if (!string.IsNullOrEmpty(existingBook.HoverImage))
            {
                DeleteImage(existingBook.HoverImage);
            }
            existingBook.HoverImage = await SaveImageAsync(HoverImageFile);
        }

        // Update additional images
        if (ImageFiles != null && ImageFiles.Any())
        {
            // Remove existing images
            foreach (var existingImage in existingBook.BookImages)
            {
                DeleteImage(existingImage.Image);
            }
            context.BookImages.RemoveRange(existingBook.BookImages);

            // Add new images
            foreach (var imageFile in ImageFiles)
            {
                if (imageFile != null)
                {
                    var imagePath = await SaveImageAsync(imageFile);
                    var bookImg = new BookImg
                    {
                        BookId = book.Id,
                        Image = imagePath
                    };
                    context.BookImages.Add(bookImg);
                }
            }
        }

        // Update tags
        context.BookTags.RemoveRange(existingBook.BookTags);
        if (TagIds != null && TagIds.Any())
        {
            foreach (var tagId in TagIds)
            {
                var bookTag = new BookTag
                {
                    BookId = book.Id,
                    TagId = tagId
                };
                context.BookTags.Add(bookTag);
            }
        }

        await context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var book = context.Books
            .Include(b => b.BookImages)
            .Include(b => b.BookTags)
            .FirstOrDefault(b => b.Id == id);

        if (book == null) return NotFound();

        // Delete images from file system
        if (!string.IsNullOrEmpty(book.MainImage))
        {
            DeleteImage(book.MainImage);
        }
        if (!string.IsNullOrEmpty(book.HoverImage))
        {
            DeleteImage(book.HoverImage);
        }
        foreach (var bookImage in book.BookImages)
        {
            DeleteImage(bookImage.Image);
        }

        context.Books.Remove(book);
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