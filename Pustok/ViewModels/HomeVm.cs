using Pustok.Models;

namespace Pustok.ViewModels;

public class HomeVm
{
    public List<Slider> Sliders { get; set; }
    public List<Book> FeaturedBooks { get; set; }
    public List<Book> NewBooks { get; set; }
    public List<Book> DiscountedBooks { get; set; }
    public List<Featured> Features { get; set; }
    
}