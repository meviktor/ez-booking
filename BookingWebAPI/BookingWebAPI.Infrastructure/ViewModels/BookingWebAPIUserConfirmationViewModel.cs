using BookingWebAPI.Infrastructure.ViewModels;

namespace BookingWebAPI.Common.ViewModels
{
    public class BookingWebAPIUserConfirmationViewModel
    {
        public BookingWebAPIUserViewModel User { get; set; } = null!;

        public IEnumerable<BookingWebAPISettingViewModel> PasswordSettings { get; set; } = null!;
    }
}
