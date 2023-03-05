using BookingWebAPI.Common.Constants;
using System.Text.RegularExpressions;

namespace BookingWebAPI.Common.Utils
{
    public static class Utilities
    {
        public static bool IsValidEmail(string email) => 
            new Regex(ApplicationConstants.EmailRegex, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture).IsMatch(email) && email.Length <= ApplicationConstants.EmailMaximumLength;
    }
}
