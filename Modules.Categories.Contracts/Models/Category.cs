namespace Modules.Categories.Contracts.Models;

public class Category
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int ListOrder { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime ModificationDate { get; set; }
    public bool IsDeleted { get; set; }
}
