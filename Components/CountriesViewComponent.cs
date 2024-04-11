using Microsoft.AspNetCore.Mvc;

namespace LegosWithAurora.Components
{
    public class CountriesViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
