using Microsoft.AspNetCore.Mvc;

namespace LegosWithAurora.Components;

public class EditProductViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View();
    }
}