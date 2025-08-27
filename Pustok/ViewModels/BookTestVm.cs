namespace Pustok.ViewModels;

public class BookTestVm
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string MainImage { get; set; }
    public string HoverImage { get; set; }
    public decimal Price { get; set; }
    public string AuthorName { get; set; }
    public string GenreName { get; set; }
    public bool InStock { get; set; }
    public bool IsNew { get; set; }
    public bool IsFeatured { get; set; }
    public string Description { get; set; }
    public List<string> BookTagNames { get; set; }
    public List<string> BookmageUrls { get; set; }
    
}