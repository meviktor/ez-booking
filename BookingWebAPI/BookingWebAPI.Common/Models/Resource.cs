﻿using BookingWebAPI.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BookingWebAPI.Common.Models
{
    [Index(nameof(Name), Name = DatabaseConstraintNames.Resource_Name_UQ, IsUnique = true)]
    public class Resource : ModelBaseVersioned
    {
        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = null!;

        [MaxLength(1000)]
        public string? Description { get; set; }

        public Guid ResourceCategoryId { get; set; }

        public virtual ResourceCategory ResourceCategory { get; set; } = null!;

        public virtual Guid SiteId { get; set; }

        public virtual Site Site { get; set; } = null!;
    }
}
