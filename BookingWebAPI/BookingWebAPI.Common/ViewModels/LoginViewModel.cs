using System.ComponentModel.DataAnnotations;

namespace BookingWebAPI.Common.ViewModels
{
    public class LoginViewModel
    {
        [Required()]
        public string Email { get; set; } = null!;

        [Required()]
        public string Password { get; set; } = null!;
    }
}
