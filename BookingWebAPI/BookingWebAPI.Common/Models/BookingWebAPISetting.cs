using BookingWebAPI.Common.Attributes;
using BookingWebAPI.Common.Constants;
using BookingWebAPI.Common.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BookingWebAPI.Common.Models
{
    [Index(nameof(Name), nameof(Category), Name = DatabaseConstraintNames.Setting_NameCategory_UQ, IsUnique = true)]
    public class BookingWebAPISetting : ModelBaseVersioned
    {
        [Required]
        [MaxLength(500)]
        public string Name { get; set; } = null!;

        [RequiredNotDefault]
        public SettingValueType ValueType { get; set; }

        [Required]
        [MaxLength(2000)]
        public string RawValue { get; set; } = null!;

        [RequiredNotDefault]
        public SettingCategory Category { get; set; }
    }
}
