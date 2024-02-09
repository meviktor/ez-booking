using BookingWebAPI.Common.Constants;
using BookingWebAPI.Common.Enums;
using BookingWebAPI.Common.Models.Config;
using BookingWebAPI.Services.Interfaces;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace BookingWebAPI.Services
{
    internal class EmailService : IEmailService
    {
        private readonly IOptions<EmailConfiguration> _emailConfig;
        private readonly IOptions<BackEndConfiguration> _backEndConfig;
        private readonly ISettingService _settingService;
        private readonly IUserService _userService;
        private readonly IEmailConfirmationAttemptService _emailConfirmationAttemptService;

        public EmailService(IOptions<EmailConfiguration> emailConfig, ISettingService settingService, IUserService userService, IEmailConfirmationAttemptService emailConfirmationAttemptService, IOptions<BackEndConfiguration> backEndConfig)
        {
            _emailConfig = emailConfig;
            _settingService = settingService;
            _userService = userService;
            _emailConfirmationAttemptService = emailConfirmationAttemptService;
            _backEndConfig = backEndConfig;
        }

        public async Task SendUserConfirmationEmailAsync(Guid userId, string tempKey)
        {
            var newUser = await _userService.GetAsync(userId);
            if (newUser == null)
            {
                throw new ArgumentException($"The following userId does not exist: {userId}.", nameof(userId));
            }

            var emailConfirmationAttempt = (await _emailConfirmationAttemptService.GetByStatusAsync(userId, EmailConfirmationStatus.Initiated)).SingleOrDefault();
            if(emailConfirmationAttempt == null)
            {
                throw new InvalidOperationException($"The user with id {userId} does not have any ongoing e-mail confirmation attempt in {EmailConfirmationStatus.Initiated} state. Could not send out the confirmation email.");
            }

            try
            {
                var emailSubject = await _settingService.GetValueBySettingNameAsync<string>(ApplicationConstants.UserRegistrationConfirmationEmailSubject);
                var emailContent = (await _settingService.GetValueBySettingNameAsync<string>(ApplicationConstants.UserRegistrationConfirmationEmailContent))
                    .Replace(ApplicationConstants.UserRegistrationConfirmationEmailLinkPlaceholder, $"{_backEndConfig.Value.Address}/{_backEndConfig.Value.PathConfimEmailAddress}/{emailConfirmationAttempt.Id}")
                    //.Replace(ApplicationConstants.UserRegistrationConfirmationAttemptIdPlaceHolder, $"{emailConfirmationAttempt.Id}")
                    .Replace(ApplicationConstants.UserRegistrationConfirmationEmailFirstNamePlaceholder, newUser.FirstName)
                    .Replace(ApplicationConstants.UserRegistrationConfirmationEmailTemporaryKeyPlaceholder, tempKey);

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

                emailConfirmationAttempt.Status = EmailConfirmationStatus.InProgress;
                await _emailConfirmationAttemptService.CreateOrUpdateAsync(emailConfirmationAttempt);
            }
            catch
            {
                emailConfirmationAttempt.Status = EmailConfirmationStatus.Failed;
                emailConfirmationAttempt.FailReason = EmailConfirmationFailReason.TechnicalError;
                await _emailConfirmationAttemptService.CreateOrUpdateAsync(emailConfirmationAttempt);

                throw;
            }
        }
    }
}
