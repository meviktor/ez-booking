using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BookingWebAPI.Common.Models
{
    [Index(nameof(Name), Name = "UQ_ResourceCategory_Name", IsUnique = true)]
    public class ResourceCategory : ModelBaseVersioned
    {
        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = null!;

        [MaxLength(1000)]
        public string? Description { get; set; }

        public Guid? BaseCategoryId { get; set; }

        public virtual ResourceCategory? BaseCategory { get; set; }

        public virtual ICollection<Resource> Resources { get; set; } = new List<Resource>();
    }
}
