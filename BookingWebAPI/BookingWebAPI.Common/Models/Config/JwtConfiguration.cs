namespace BookingWebAPI.Common.Models.Config
{
    public class JwtConfiguration
    {
        public string Secret { get; set; } = null!;

        public uint? ValidInSeconds { get; set; }
    }
}
