namespace BookingWebAPI.Common.Models
{
    public class BookingWebAPIUser : ModelBase
    {
        public string? UserName { get; set; }

        public string? Email { get; set; }

        public bool EmailConfirmed { get; set; }

        public virtual string? PasswordHash { get; set; }

        public bool LockoutEnabled { get; set; }

        public int AccessFailedCount { get; set; }
    }
}
