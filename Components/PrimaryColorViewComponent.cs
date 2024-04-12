using LegosWithAurora.Models;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;

namespace LegosWithAurora.Components
{
    public class PrimaryColorViewComponent : ViewComponent
    {

        private ILegoRepository _repo;

        public PrimaryColorViewComponent(ILegoRepository temp)
        {
            _repo = temp;
        }

        public IViewComponentResult Invoke()
        {
            ViewBag.SelectedColor = RouteData?.Values["color"];
            
            var color = _repo.Products
               .Select(x => x.PrimaryColor)
               .Distinct()
               .OrderBy(x => x);

            return View(color);
        }
    }
}
