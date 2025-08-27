using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok.Data;
using Pustok.ViewModels;

namespace Pustok.Controllers;

public class BookController : Controller
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public BookController(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    // GET
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Detail(int id)
    {
        var book = _context.Books
            .Include(b => b.Author)
            .Include(b => b.Genre)
            .Include(b => b.BookImages)
            .Include(b => b.BookTags)
            .ThenInclude(bt => bt.Tag)
            .FirstOrDefault(b => b.Id == id);

        if (book == null)
        {
            return NotFound();
        }
        BookDetailVm bookDetailVm = new BookDetailVm
        {
            Book = book,
            RelatedBooks = _context.Books
                .Include(b => b.BookImages)
                .Where(b => b.GenreId == book.GenreId && b.Id != book.Id)
                .Take(4)
                .ToList()
        };

        return View(bookDetailVm);
    }

    public IActionResult BookModal(int id)
    {
        var book = _context.Books
            .Include(b => b.Author)
            .Include(b => b.Genre)
            .Include(b => b.BookImages)
            .Include(b => b.BookTags)
            .ThenInclude(bt => bt.Tag)
            .FirstOrDefault(b => b.Id == id);

        if (book == null)
        {
            return NotFound();
        }

        return PartialView("_BookModalPartial",book);
    }
    
    
    public IActionResult Test(int id)
    {
        var book = _context.Books
            .Include(b => b.Author)
            .Include(b => b.Genre)
            .Include(b => b.BookImages)
            .Include(b => b.BookTags)
            .ThenInclude(bt => bt.Tag)
            .FirstOrDefault(bt => bt.Id == id);
        

        if (book == null)
        {
            return NotFound();
        }
        BookTestVm bookTestVm = _mapper.Map<BookTestVm>(book);
        return Ok(bookTestVm);
    
    }
}