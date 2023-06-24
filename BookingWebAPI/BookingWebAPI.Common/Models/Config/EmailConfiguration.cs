namespace BookingWebAPI.Common.Models.Config
{
    public class EmailConfiguration
    {
        public string SmtpServer { get; set; } = null!;

        public short SmtpPort { get; set; }

        public string SmtpUsername { get; set; } = null!;

        public string SmtpPassword { get; set; } = null!;

        public string FromAddress { get; set; } = null!;
    }
}
