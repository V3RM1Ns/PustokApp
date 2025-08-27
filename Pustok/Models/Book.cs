using System.ComponentModel.DataAnnotations.Schema;

namespace Pustok.Models;

public class Book:AuditEntity
{
    public string Title { get; set; }
    public string Description { get; set; }
    
    public string MainImage { get; set; }
    public string HoverImage { get; set; }
    
    public Author Author { get; set; }
    public int AuthorId { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }
    public int DiscountPrice { get; set; }
    
    public bool InStock { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsNew { get; set; }
    
    public Genre Genre { get; set; }
    public int GenreId { get; set; }
    
    public List<BookImg> BookImages { get; set; }
    
    public List<BookTag> BookTags { get; set; }
}