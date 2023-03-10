using BookingWebAPI.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingWebAPI.Common.Models
{
    [Index(nameof(UserName), Name = DatabaseConstraintNames.User_UserName_UQ, IsUnique = true)]
    [Index(nameof(Email), Name = DatabaseConstraintNames.User_Email_UQ, IsUnique = true)]
    public class BookingWebAPIUser : ModelBase
    {
        [Required]
        [MaxLength(ApplicationConstants.UserNameMaximumLength)]
        public string UserName { get; set; } = null!;

        [Required]
        [MaxLength(ApplicationConstants.EmailMaximumLength)]
        public string Email { get; set; } = null!;

        public bool EmailConfirmed { get; set; }

        [Required]
        [Column(TypeName = DatabaseConstraintNames.User_PasswordHash_ColumnType)]
        public string PasswordHash { get; set; } = null!;

        public bool LockoutEnabled { get; set; }

        public int AccessFailedCount { get; set; }
    }
}
