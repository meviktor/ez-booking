namespace BookingWebAPI.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendUserConfirmationEmail(Guid userId);
    }
}
