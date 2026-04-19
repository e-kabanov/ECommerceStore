using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ECommerceStore.Models;

[Index("StatusName", Name = "UQ__OrderSta__05E7698AF136F52F", IsUnique = true)]
public partial class OrderStatus
{
    [Key]
    public int StatusId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string StatusName { get; set; } = null!;

    [StringLength(255)]
    [Unicode(false)]
    public string? StatusDescription { get; set; }

    [InverseProperty("Status")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
