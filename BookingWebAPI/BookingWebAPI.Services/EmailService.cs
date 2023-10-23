using BookingWebAPI.Common.Constants;
using BookingWebAPI.Common.Models.Config;
using BookingWebAPI.Services.Interfaces;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace BookingWebAPI.Services
{
    internal class EmailService : IEmailService
    {
        private readonly IOptions<EmailConfiguration> _emailConfig;
        private readonly ISettingService _settingService;
        private readonly IUserService _userService;

        public EmailService(IOptions<EmailConfiguration> emailConfig, ISettingService settingService, IUserService userService) 
        {
            _emailConfig = emailConfig;
            _settingService = settingService;
            _userService = userService;
        }

        public async Task SendUserConfirmationEmailAsync(Guid userId)
        {
            var newUser = await _userService.GetAsync(userId);
            if (newUser == null)
            {
                throw new ArgumentException($"The following userId does not exist: {userId}.", nameof(userId));
            }
            var emailSubject = await _settingService.GetValueBySettingNameAsync<string>(ApplicationConstants.UserRegistrationConfirmationEmailSubject);
            var emailContent = await _settingService.GetValueBySettingNameAsync<string>(ApplicationConstants.UserRegistrationConfirmationEmailContent);

            var confirmationEmail = new MimeMessage
            {
                Subject = emailSubject,
                Body = new BodyBuilder { HtmlBody = emailContent }.ToMessageBody()
            };
            confirmationEmail.To.Add(MailboxAddress.Parse(newUser.Email));
            confirmationEmail.From.Add(MailboxAddress.Parse(_emailConfig.Value.FromAddress));

            using var emailClient = new SmtpClient();
            await emailClient.ConnectAsync(_emailConfig.Value.SmtpServer, _emailConfig.Value.SmtpPort);
            await emailClient.AuthenticateAsync(_emailConfig.Value.SmtpUsername, _emailConfig.Value.SmtpPassword);
            await emailClient.SendAsync(confirmationEmail);
            await emailClient.DisconnectAsync(true);
        }
    }
}
