using Microsoft.AspNetCore.Mvc;

namespace LegosWithAurora.Components
{
    public class LegoSetsViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
