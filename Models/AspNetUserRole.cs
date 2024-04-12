
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LegosWithAurora.Models;
public class AspNetUserRole
{
    [Key]
    [Required]
    AspNetUser User { get; set; }
    AspNetRole Role { get; set; } = new AspNetRole { Id = "b9afe3d1-f5f3-45b6-8b81-b2e6623d1e7c", Name = "Admin", NormalizedName = "ADMIN"};

}
