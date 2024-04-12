using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegosWithAurora.Models;

// note on validation: we have chosen the fields we want here. The others we are okay being null
public partial class Product
{
    [Key]
    [Required]
    public int ProductId { get; set; }

    [Required(ErrorMessage = "Sorry, you need to enter a product name")]
    public string? Name { get; set; }

    [Required(ErrorMessage = "Sorry, you need to enter the product's year")]
    public int? Year { get; set; }

    [Required(ErrorMessage = "Sorry, you need to enter the number of parts")]
    public int? NumParts { get; set; }

    [Required(ErrorMessage = "Sorry, you need to enter the price")]
    public int? Price { get; set; }

    public string? ImgLink { get; set; }

    public string? PrimaryColor { get; set; }

    public string? SecondaryColor { get; set; }

    [Required(ErrorMessage = "Sorry, you need to enter a description")]
    public string? Description { get; set; }
    
    public string? Category { get; set; }
}
