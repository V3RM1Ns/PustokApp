using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok.Data;
using Pustok.Models;
using Pustok.ViewModels;

namespace Pustok.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _appDbContext;
    public HomeController(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }
    
    public async Task<IActionResult> Index()
    {
        var viewModel = new HomeVm()
        { 
            Features = await _appDbContext.Features.ToListAsync(),
            Sliders = await _appDbContext.Sliders.ToListAsync()
            
            , FeaturedBooks = await _appDbContext.Books
                .Where(b => b.IsFeatured)
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .ToListAsync()
            
            
            , NewBooks = await _appDbContext.Books
                .Where(b => b.IsNew)
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .ToListAsync()
            
            
            , DiscountedBooks = await _appDbContext.Books
                .Where(b => b.DiscountPrice > 0)
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .ToListAsync()
        };
        return View(viewModel);
    }

}