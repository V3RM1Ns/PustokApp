using System.ComponentModel.DataAnnotations;

namespace Pustok.Models;

public class Genre:BaseEntity
{
    [Required(ErrorMessage = "Genre name is required.")]
    [StringLength(100, ErrorMessage = "Genre name cannot exceed 100 characters.")]
    [Display(Name = "Genre Name")]
    public string Name { get; set; } = string.Empty;
    
    public List<Book> Books { get; set; } = new();
}