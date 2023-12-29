using BookingWebAPI.Common.Attributes;
using BookingWebAPI.Common.ErrorCodes;
using System.ComponentModel.DataAnnotations;

namespace BookingWebAPI.Common.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = ApplicationErrorCodes.UserEmailRequired)]
        [BookingWebAPIEmailAddress(ErrorMessage = ApplicationErrorCodes.UserEmailInvalidFormat)]
        public string EmailAddress { get; set; } = null!;

        [Required(ErrorMessage = ApplicationErrorCodes.UserFirstNameRequired)]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = ApplicationErrorCodes.UserLastNameRequired)]
        public string LastName { get; set; } = null!;

        [RequiredNotDefault(ErrorMessage = ApplicationErrorCodes.UserSiteIdRequired)]
        public Guid SiteId { get; set; }
    }
}
