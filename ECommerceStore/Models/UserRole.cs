using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ECommerceStore.Models;

[Index("UserRoleName", Name = "UQ__UserRole__518ED90A73163F87", IsUnique = true)]
public partial class UserRole
{
    [Key]
    public int UserRoleId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string UserRoleName { get; set; } = null!;

    [InverseProperty("UserRole")]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
