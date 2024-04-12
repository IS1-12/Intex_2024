using LegosWithAurora.Models;
using Microsoft.AspNetCore.Mvc;

namespace LegosWithAurora.Components;

public class ProductCategoriesViewComponent : ViewComponent
{
    private ILegoRepository _repo;

    public ProductCategoriesViewComponent(ILegoRepository temp)
    {
        _repo = temp;
    }

    public IViewComponentResult Invoke()
    {
        ViewBag.SelectedProductType = RouteData.Values["categories"]!;
       
        var categoryTypes = _repo.Products
            .Select(x => x.Category)
            .Distinct()
            .OrderBy(x => x);
        
        return View(categoryTypes);
    }
}