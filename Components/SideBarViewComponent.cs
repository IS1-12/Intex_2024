using Microsoft.AspNetCore.Mvc;

namespace LegosWithAurora.Components
{
    public class SideBarViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
