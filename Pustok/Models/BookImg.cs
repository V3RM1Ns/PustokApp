namespace Pustok.Models;

public class BookImg:BaseEntity
{
    public string Image { get; set; }
    public int BookId { get; set; }
    public Book Book { get; set; }
}