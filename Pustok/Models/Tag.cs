using System.ComponentModel.DataAnnotations;

namespace Pustok.Models;

public class Tag:BaseEntity
{
    [Required(ErrorMessage = "Tag name is required.")]
    [StringLength(50, ErrorMessage = "Tag name cannot exceed 50 characters.")]
    [Display(Name = "Tag Name")]
    public string Name { get; set; } = string.Empty;
    
    public List<BookTag> BookTags { get; set; } = new();
}