using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BookingWebAPI.Common.Models
{
    [Index(nameof(Name), Name = "UQ_Site_Name", IsUnique = true)]
    public class Site : ModelBaseVersioned
    {
        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = null!;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        [MaxLength(100)]
        public string Country { get; set; } = null!;

        [Required]
        [MaxLength(25)]
        public string ZipCode { get; set; } = null!;

        [MaxLength(100)]
        public string? State { get; set; }

        [MaxLength(100)]
        public string? County { get; set; }

        [Required]
        [MaxLength(100)]
        public string City { get; set; } = null!;

        [Required]
        [MaxLength(150)]
        public string Street { get; set; } = null!;

        [Required]
        [MaxLength(25)]
        public string HouseOrFlatNumber { get; set; } = null!;
    }
}
