using BookingWebAPI.Common.Attributes;
using BookingWebAPI.Infrastructure.ViewModels;

namespace BookingWebAPI.Common.ViewModels
{
    public class BookingWebAPIUserConfirmationViewModel
    {
        public BookingWebAPIUserViewModel User { get; set; } = null!;

        [RequiredNotDefault]
        public Guid OneTimeToken { get; set; }

        public IEnumerable<BookingWebAPISettingViewModel> PasswordSettings { get; set; } = null!;
    }
}
