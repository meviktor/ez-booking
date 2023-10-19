using BookingWebAPI.Common.Attributes;
using FluentAssertions;
using NUnit.Framework;

namespace BookingWebAPI.Common.Tests.Unit
{
    internal class BookingWebAPIEmailAddressAttributeTests
    {
        private BookingWebAPIEmailAddressAttribute _attrInstance = new BookingWebAPIEmailAddressAttribute();

        [TestCaseSource(nameof(IsValidTestCases))]
        public void IsValid_Test(bool expectedAsValid, bool expectedException, object? value)
        {
            var action = () => _attrInstance.IsValid(value);

            if (!expectedAsValid && expectedException)
            {
                action.Should().Throw<ArgumentException>();
            }
            else
            {
                action().Should().Be(expectedAsValid);
            }
        }

        private static readonly object?[] IsValidTestCases =
        {
            new object?[] { true, false, "testmail@provider.com" },
            new object?[] { false, false, "testmail@providercom" },
            new object?[] { false, false, "" },
            new object?[] { false, true, (string?)null },
            new object?[] { false, true, (byte)1 },
            new object?[] { false, true, 7.6f }
        };
    }
}
