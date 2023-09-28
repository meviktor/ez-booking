using BookingWebAPI.Common.Attributes;
using BookingWebAPI.Common.Constants;
using BookingWebAPI.Common.ErrorCodes;
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
        [BookingWebAPIEmailAddress(ErrorMessage = ApplicationErrorCodes.UserEmailInvalidFormat)]
        public string Email { get; set; } = null!;

        [Column(TypeName = DatabaseConstraintNames.User_PasswordHash_ColumnType)]
        public string? PasswordHash { get; set; } = null!;

        public bool EmailConfirmed { get; set; }

        public Guid? Token { get; set; }

        public bool LockoutEnabled { get; set; }

        public int AccessFailedCount { get; set; }

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public int? WorkHoursWeekly { get; set; }

        public Guid SiteId { get; set; }

        public virtual Site Site { get; set; } = null!;
    }
}
