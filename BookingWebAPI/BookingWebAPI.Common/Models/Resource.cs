using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BookingWebAPI.Common.Models
{
    [Index(nameof(Name), Name = "UQ_Resource_Name", IsUnique = true)]
    public class Resource : ModelBaseVersioned
    {
        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = null!;

        [MaxLength(1000)]
        public string? Description { get; set; }

        public Guid? ResourceCategoryId { get; set; }

        public virtual ResourceCategory? ResourceCategory { get; set; }
    }
}
