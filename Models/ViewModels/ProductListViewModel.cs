namespace LegosWithAurora.Models.ViewModels;

public class ProductListViewModel
{
    public IQueryable<Product> Products { get; set; }
    public PaginationInfo PaginationInfo { get; set; } = new PaginationInfo();
    public string? CurrentProductType { get; set; }
    public string? CurrentColor { get; set; }
}