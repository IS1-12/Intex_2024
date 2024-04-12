using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegosWithAurora.Models;

public partial class Product
{
    [Key]
    [Required]
    public int ProductId { get; set; }

    public string? Name { get; set; }

    public int? Year { get; set; }

    public int? NumParts { get; set; }

    public int? Price { get; set; }

    public string? ImgLink { get; set; }

    public string? PrimaryColor { get; set; }

    public string? SecondaryColor { get; set; }

    public string? Description { get; set; }
    
    public string? Category { get; set; }
}
