using Microsoft.AspNetCore.Identity;

namespace LegosWithAurora.Models.ViewModels
{
    public class UserViewModel
    {
        public AspNetUser User { get; set; }
        public bool IsInRole { get; set; }
    }
}
