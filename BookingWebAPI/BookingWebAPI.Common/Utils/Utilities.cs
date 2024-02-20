using BookingWebAPI.Common.Constants;
using Fare;
using Microsoft.IdentityModel.Tokens;
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
            if(length <= 0)
            {
                throw new ArgumentException("Not valid string length.", nameof(length));
            }

            var positions = GenerateUniqueRandoms(3, 0, length);
            int? digitPos = hasDigits ? positions.ElementAt(0) : null;
            int? uppercasePos = hasUppercaseLetter ? positions.ElementAt(1) : null;
            int? specialPos = hasSpecialCharacters ? positions.ElementAt(2) : null;

            string? digit = digitPos.HasValue ? new Xeger("[0-9]").Generate() : null;
            string? uppercase = uppercasePos.HasValue ? new Xeger("[A-Z]").Generate() : null;
            string? special = specialPos.HasValue ? new Xeger(@"[""~`!#\$%\^&\*\+,\-\./:;<>\?@\[\]_\{\}\|\(\)\|\\]").Generate() : null;

            string regex = $@"({(hasDigits ? "[0-9]|" : string.Empty)}{(hasUppercaseLetter ? "[A-Z]|" : string.Empty)}{(hasSpecialCharacters ? @"[""~`!#\$%\^&\*\+,\-\./:;<>\?@\[\]_\{\}\|\(\)\|\\]|" : string.Empty)}[a-z]){{{length}}}";
            string randomString = new Xeger(regex).Generate();

            char[] letters = randomString.ToArray();
            if (digitPos.HasValue) letters[digitPos.Value] = digit![0];
            if (uppercasePos.HasValue) letters[uppercasePos.Value] = uppercase![0];
            if (specialPos.HasValue) letters[specialPos.Value] = special![0];

            return new string(letters);
        }

        public static IEnumerable<int> GenerateUniqueRandoms(int qunatity, int lowerLimit, int upperLimit, IEnumerable<int>? valuesToExclude = null)
        {
            if(qunatity <= 0)
            {
                throw new ArgumentException($"Argument {nameof(qunatity)} must be a positive number.", nameof(qunatity));
            }
            if (lowerLimit + qunatity > upperLimit)
            {
                throw new ArgumentException($"Argument {nameof(upperLimit)} is too low.", nameof(upperLimit));
            }
            if (valuesToExclude != null && valuesToExclude.Any())
            {
                var valuesToExcludeInRange = valuesToExclude.Where(v => v >= lowerLimit && v < upperLimit);
                int numberOfPossibleValues = upperLimit - lowerLimit - qunatity;
                if (numberOfPossibleValues < qunatity)
                {
                    throw new ArgumentException($"Cannot generate {qunatity} random values, too many values have been exluded.");
                }
                if (numberOfPossibleValues == qunatity)
                {
                    // return all numbers from the range, which are left after the exclusion
                    return Enumerable.Range(lowerLimit, upperLimit - lowerLimit).Where(v => !valuesToExcludeInRange.Contains(v));
                }
            }

            var baseSet = Enumerable.Range(lowerLimit, upperLimit - lowerLimit);
            var allPossibleValues = valuesToExclude?.Any() ?? false ? baseSet.Where(v => !valuesToExclude.Contains(v)) : baseSet;
            var random = new Random();
            var randomsSelected = new HashSet<int>();
            for (int i = 0; i < qunatity; i++)
            {
                int v = allPossibleValues.ElementAt(random.Next(allPossibleValues.Count()));
                randomsSelected.Add(v);
                allPossibleValues = allPossibleValues.Where(val => val != v);
            }

            return randomsSelected;
        }
    }
}
