using BookingWebAPI.Common.Constants;
using BookingWebAPI.Common.Utils;
using BookingWebAPI.Testing.Common;
using FluentAssertions;
using NUnit.Framework;
using System.Text.RegularExpressions;

namespace BookingWebAPI.Common.Tests.Unit
{
    internal class UtilitiesTests : UnitTestBase
    {
        [TestCase(true, 20, -15, 30, null)]
        [TestCase(true, 10, 0, 100, null)]
        [TestCase(true, 10, 0, 100, new int[] { 5, 1, 77, 46, 99 })]
        [TestCase(true, 10, 0, 100, new int[] { 5, 1, 77, 46, 102 })]
        [TestCase(false, 10, 100, 0, null)]
        [TestCase(false, 10, 10, 15, null)]
        [TestCase(false, 10, 10, 10, null)]
        [TestCase(false, 10, 20, 15, null)]
        [TestCase(false, -10, 0, 50, null)]
        [TestCase(false, 5, 10, 20, new int[] { 11, 12, 13, 14, 17, 18, 19 })]
        [TestCase(true, 5, 10, 20, new int[] { 11, 20, 21, 22, 23, 24, 25 })]
        public void GenerateUniqueRandoms_Test(bool shouldSucceed, int qunatity, int lowerLimit, int upperLimit, IEnumerable<int>? valuesToExclude = null)
        {
            // prepare
            var action = () => Utilities.GenerateUniqueRandoms(qunatity, lowerLimit, upperLimit, valuesToExclude);
                
            // action & assert 
            if (!shouldSucceed)
            {
                action.Should().Throw<ArgumentException>();
            }
            else
            {
                var numbers = action();
                var allItemsUnique = numbers.Distinct().Count() == numbers.Count();
                var valuesExcluded = valuesToExclude == null ? true : numbers.All(n => !valuesToExclude.Contains(n));

                numbers.Count().Should().Be(qunatity);
                allItemsUnique.Should().BeTrue();
                valuesExcluded.Should().BeTrue();
            }
        }

        [TestCase( 8, true,  true,   true,  false)]
        [TestCase(16, true,  true,   false, false)]
        [TestCase(12, true,  false,  true,  false)]
        [TestCase( 4, true,  false,  false, false)]
        [TestCase( 8, false, true,   true,  false)]
        [TestCase(16, false, true,   false, false)]
        [TestCase(12, false, false,  true,  false)]
        [TestCase( 4, false, false,  false, false)]
        [TestCase( 0, false, false,  false, true)]
        [TestCase(-1, true,  true,   true,  true)]
        [TestCase( 0, true,  false,  false, true)]
        [TestCase( 1, true,  true,   false, true)]
        [TestCase( 2, true,  true,   true,  true)]
        public void RandomString_Test(int desiredLength, bool shoudldHaveUppercaseLetter, bool shoudldHaveDigits, bool shoudldHaveSpecialCharacters, bool invalidConfig)
        {
            // prepare
            var action = () => Utilities.RandomString(desiredLength, shoudldHaveUppercaseLetter, shoudldHaveDigits, shoudldHaveSpecialCharacters);

            // action & assert
            if (invalidConfig)
            {
                action.Should().Throw<ArgumentException>();
            }
            else
            {
                var generatedString = action();

                generatedString.Length.Should().Be(desiredLength);
                if (shoudldHaveUppercaseLetter)
                {
                    IsRegexMatch(ApplicationConstants.RegexUppercase, generatedString).Should().BeTrue();
                }
                if (shoudldHaveDigits)
                {
                    IsRegexMatch(ApplicationConstants.RegexNumbers, generatedString).Should().BeTrue();
                }
                if (shoudldHaveSpecialCharacters)
                {
                    IsRegexMatch(ApplicationConstants.RegexSpecialChars, generatedString).Should().BeTrue();
                }
            }
        }

        private static bool IsRegexMatch(string regex, string stringToTest) => new Regex(regex).IsMatch(stringToTest);
    }
}
