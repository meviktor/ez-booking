using BookingWebAPI.Common.Attributes;
using System.ComponentModel.DataAnnotations;

namespace BookingWebAPI.Common.ViewModels
{
    public class ConfirmRegistrationViewModel
    {
        [RequiredNotDefault]
        public Guid UserId { get; set; }

        [RequiredNotDefault]
        public Guid Token { get; set; }

        [Required]
        public string Password { get; set; } = null!;
    }
}
