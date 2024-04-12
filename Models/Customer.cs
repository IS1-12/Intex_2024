using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LegosWithAurora.Models;

public partial class Customer
{
    [Key]
    [Required]
    public int CustomerId { get; set; }

    [Required(ErrorMessage = "Sorry, you need to enter a first name")]
    public string? FirstName { get; set; }

    [Required(ErrorMessage = "Sorry, you need to enter a last name")]
    public string? LastName { get; set; }

    [Required(ErrorMessage = "Sorry, you need to enter a birth date")]
    public string? BirthDate { get; set; }

    [Required(ErrorMessage = "Sorry, you need to enter a country of residence")]
    public string? CountryOfResidence { get; set; }

    public string? Gender { get; set; }

    public int? Age { get; set; }
}
