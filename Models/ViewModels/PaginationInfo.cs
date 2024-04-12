namespace LegosWithAurora.Models.ViewModels;

public class PaginationInfo
{
    public int TotalItems { get; set; }
    public int ItemsPerPage { get; set; } = 9;
    public int CurrentPage { get; set; }
    public int TotalNumPages => (int)(Math.Ceiling((decimal)TotalItems / ItemsPerPage));
}