using BookingWebAPI.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BookingWebAPI.Common.Models
{
    [Index(nameof(Name), Name = DatabaseConstraintNames.ResourceCategory_Name_UQ, IsUnique = true)]
    public class ResourceCategory : ModelBaseVersioned
    {
        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = null!;

        [MaxLength(1000)]
        public string? Description { get; set; }

        public Guid? BaseCategoryId { get; set; }

        public ResourceCategory? BaseCategory { get; set; }

        public ICollection<Resource> Resources { get; set; } = new List<Resource>();
    }
}
