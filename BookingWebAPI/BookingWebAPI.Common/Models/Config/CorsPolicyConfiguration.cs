namespace BookingWebAPI.Common.Models.Config
{
    public class CorsPolicyConfiguration
    {
        public string[] AllowedOrigins { get; set; } = Array.Empty<string>();
    }
}
