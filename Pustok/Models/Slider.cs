namespace Pustok.Models;

public class Slider:AuditEntity
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string Image { get; set; }
    public string ButtonText { get; set; }
    public string ButtonUrl { get; set; }
    public int Order { get; set; }
}