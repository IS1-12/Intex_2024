using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LegosWithAurora.Models;

public partial class LineItem
{
    public int? TransactionId { get; set; }

    [Required(ErrorMessage = "Sorry, you must have an item in your cart to check out")]
    public int? ProductId { get; set; }

    public int? Qty { get; set; }

    public int? Rating { get; set; }
}
