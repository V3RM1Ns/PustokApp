using Microsoft.AspNetCore.Mvc;

namespace Pustok.App.Areas.Manage.Controllers
{
    [Area("Manage")]    
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
