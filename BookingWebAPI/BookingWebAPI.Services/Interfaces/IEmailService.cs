namespace BookingWebAPI.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendUserConfirmationEmailAsync(Guid userId);
    }
}
