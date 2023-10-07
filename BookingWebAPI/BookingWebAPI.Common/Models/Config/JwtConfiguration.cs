namespace BookingWebAPI.Common.Models.Config
{
    public class JwtConfiguration
    {
        public string Secret { get; set; } = null!;

        public int? ValidInSeconds { get; set; }
    }
}
