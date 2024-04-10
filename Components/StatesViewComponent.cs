using Microsoft.AspNetCore.Mvc;

namespace LegosWithAurora.Components
{
    public class StatesViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
