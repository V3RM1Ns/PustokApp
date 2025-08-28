using System.ComponentModel.DataAnnotations;

namespace Pustok.Models;

public class Author:AuditEntity
{
    [Required(ErrorMessage = "Yazar adı gereklidir")]
    [StringLength(100, ErrorMessage = "Yazar adı en fazla 100 karakter olabilir")]
    public string Name { get; set; } = string.Empty;
    
    public List<Book> Books { get; set; } = new();
}