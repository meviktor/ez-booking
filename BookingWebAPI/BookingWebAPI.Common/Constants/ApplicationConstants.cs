namespace BookingWebAPI.Common.Constants
{
    public static class ApplicationConstants
    {
        public const int BcryptLength = 60;

        public const int EmailMaximumLength = 320;
        public const string EmailRegex = @"^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$";

        public const int UserNameMaximumLength = 50;

        public const int SiteNameMaximiumLength = 150;
        public const int SiteDescriptionMaximiumLength = 1000;
        public const int SiteCountryMaximiumLength = 150;
        public const int SiteZipCodeMaximiumLength = 25;
        public const int SiteStateMaximiumLength = 100;
        public const int SiteCountyMaximiumLength = 100;
        public const int SiteCityMaximiumLength = 100;
        public const int SiteStreetMaximiumLength = 150;
        public const int SiteHouseOrFlatNumberMaximiumLength = 25;

        public const string JwtClaimId = "id";
        public const string JwtClaimEmail = "email";
        public const string JwtClaimUserName = "username";

        public const string AppStartupErrorNoConnectionString = "No connection string provided for database access. Check your configuration for missing connection string.";

        // Settings: login
        public const string LoginMaxAttempts = nameof(LoginMaxAttempts);

        // Settings: PasswordPolicy
        public const string PasswordPolicyMinLength = nameof(PasswordPolicyMinLength);
        public const string PasswordPolicyMaxLength = nameof(PasswordPolicyMaxLength);
        public const string PasswordPolicyUppercaseLetter = nameof(PasswordPolicyUppercaseLetter);
        public const string PasswordPolicySpecialCharacters = nameof(PasswordPolicySpecialCharacters);
        public const string PasswordPolicyDigits = nameof(PasswordPolicyDigits);

        // Emails:
        public const string UserRegistrationConfirmationEmailSubject = nameof(UserRegistrationConfirmationEmailSubject);
        public const string UserRegistrationConfirmationEmailDefaultSubject = "Confirm your new ez-booking account";
        public const string UserRegistrationConfirmationEmailContent = nameof(UserRegistrationConfirmationEmailContent);
        public const string UserRegistrationConfirmationEmailFirstNamePlaceholder = "##userFirstName##";
        public const string UserRegistrationConfirmationEmailLinkPlaceholder = "##confrimUserLink##";
        public const string UserRegistrationConfirmationEmailTemporaryKeyPlaceholder = "##temporaryKey##";
        public const string UserRegistrationConfirmationAttemptIdPlaceHolder = "##confirmationAttemptId##";

        // TODO: this e-mail content should come from an html file!
        public const string UserRegistrationConfirmationEmailDefaultContent = @"
            <!DOCTYPE html>
            <html>
              <head>
                <meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8""/>
				<style type=""text/css"">
				  .header {
				    height:15%;
                    background-color:#27364f;
                    color:#6781a8;
                    font-family:Arial,Helvetica,sans-serif;
				    font-weight:700;
					font-size:3rem;
                  }
                  .footer {
                    height:10%;
                    background-color:#27364f;
                    color:#fff;
                    font-size:.75rem;
                    text-align:center;
                  }
                  .logo {
                    padding:1rem;
                  }
				</style>
             </head>
             <body>
               <div class=""header"">
                 <div class=""logo"">ez-booking</div>
               </div>
               <div>
                 <p><b>Hi ##userFirstName##!</b></p>
                 <p>A new ez-booking account has been created for this e-mail address. You have to activate it first in order to get an access. To do this, you have to click on the link below:</p>
                 <form method=""post"" action=""##confrimUserLink##"" class=""inline"">
                   <input type=""hidden"" name=""confirmationAttemptId"" value=""##confirmationAttemptId##"">
                   <button type=""submit"" name=""submit_param"" value=""submit_value"" class=""link-button"">
                     Confirm your e-mail address
                   </button>
                 </form>
                 <p>After the activation succeeded, you will be able to log in. Your default password is the following: <b>##temporaryKey##</b><br> For your safety, please change your password after the first successful login.</p>
                 <p>Please consider the displayed policy when choosing an appropriate password.</p>
                 <p>If you were not expecting to get an access to ez-booking and so this confirmation e-mail, please ignore this message.</p>
                 <p>Best wishes,<br>The ez-booking team</p>
               </div>
               <div class=""footer"">
                  <div>This e-mail was sent from an e-mail address not being monitored. Please do not reply to it.</div>
               </div>
              </body>
            </html>";
        // TOODO: this should come from a configuration file!
        public const int ActivationLinkExpirationHours = 48;

        // Authorization
        public const string JwtToken = nameof(JwtToken);
        public const string Authorization = nameof(Authorization);
    }
}
