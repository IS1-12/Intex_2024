using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LegosWithAurora.Models;

//a note on validating this model: We set almost all of the values. Very little needs to be validated. 
public partial class Order
{
    [Key]
    [Required]
    public int TransactionId { get; set; }

    [Required(ErrorMessage = "Sorry, you need to log in")]
    public int? CustomerId { get; set; }

    public string? Date { get; set; } = DateTime.Today.ToString("M/d/yyyy");

    public string? DayOfWeek { get; set; } = DateTime.Today.DayOfWeek.ToString();

    public int? Time { get; set; } = DateTime.Now.Hour;

    public string? EntryMode { get; set; } = "CVC";

    public int? Amount { get; set; }

    public string? TypeOfTransaction { get; set; } = "Online";

    public string? CountryOfTransaction { get; set; }

    public string? ShippingAddress { get; set; } = "USA";

    public string? Bank { get; set; } = "Bank of Juan";

    public string? TypeOfCard { get; set; } = "Visa";

    public int? Fraud { get; set; }
}