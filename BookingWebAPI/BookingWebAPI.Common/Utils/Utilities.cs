using BookingWebAPI.Common.Constants;
using Microsoft.IdentityModel.Tokens;
using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace BookingWebAPI.Common.Utils
{
    public static class Utilities
    {
        public static bool IsValidEmail(string email) => 
            email != null ? 
                new Regex(ApplicationConstants.EmailRegex, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture).IsMatch(email) && email.Length <= ApplicationConstants.EmailMaximumLength :
                false;

        public static string CreateJwtToken(Guid userId, string userEmail, string userName, int expirationInSecs, string jwtSecret)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim(ApplicationConstants.JwtClaimId, $"{userId}"),
                    new Claim(ApplicationConstants.JwtClaimEmail, $"{userEmail}"),
                    new Claim(ApplicationConstants.JwtClaimUserName, $"{userName}")
                }),
                Expires = DateTime.UtcNow.AddSeconds(expirationInSecs),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public static string RandomString(int length, bool hasUppercaseLetter, bool hasDigits, bool hasSpecialCharacters)
        {
            var stringRegex = $"^{(hasUppercaseLetter ? "(?=.*[A-Z])" : string.Empty)}{(hasDigits ? "(?=.*\\d)" : string.Empty)}{(hasSpecialCharacters ? "(?=.*[^\\w\\s\\d])" : string.Empty)}.{{{length}}}$";
            var randomizerTextRegex = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = stringRegex });
            return randomizerTextRegex.Generate();
        }
    }
}
