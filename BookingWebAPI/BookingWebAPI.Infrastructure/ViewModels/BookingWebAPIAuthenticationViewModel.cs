namespace BookingWebAPI.Common.ViewModels
{
    public class BookingWebAPIAuthenticationViewModel
    {
        public BookingWebAPIUserViewModel User { get; set; } = null!;

        public string Token { get; set; } = null!;
    }
}
