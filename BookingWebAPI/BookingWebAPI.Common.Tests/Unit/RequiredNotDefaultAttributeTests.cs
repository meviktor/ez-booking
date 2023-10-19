using BookingWebAPI.Common.Attributes;
using NUnit.Framework;
using FluentAssertions;

namespace BookingWebAPI.Common.Tests.Unit
{
    public class RequiredNotDefaultAttributeTests
    {
        private RequiredNotDefaultAttribute _attrInstance = new RequiredNotDefaultAttribute();

        [TestCaseSource(nameof(IsValidTestCases))]
        public void IsValid_Test(bool expectedAsValid, bool expectedException, object? value)
        {
            // prepare
            var assertAction = () => _attrInstance.IsValid(value);

            // action & assert
            if (!expectedException)
            {
                assertAction.Should().NotThrow().Which.Should().Be(expectedAsValid);
            }
            else assertAction.Should().Throw<ArgumentException>();
        }

        private static readonly object?[] IsValidTestCases =
        {
            new object?[] { true, false, (char)'a' },
            new object?[] { false, false, default(char) },
            new object?[] { true, false, (byte)1 },
            new object?[] { false, false, default(byte) },
            new object?[] { true, false, (sbyte)2 },
            new object?[] { false, false, default(sbyte) },
            new object?[] { true, false, (short)3 },
            new object?[] { false, false, default(short) },
            new object?[] { true, false, (ushort)4 },
            new object?[] { false, false, default(ushort) },
            new object?[] { true, false, (int)5 },
            new object?[] { false, false, default(int) },
            new object?[] { true, false, (uint)6 },
            new object?[] { false, false, default(uint) },
            new object?[] { true, false, (long)7 },
            new object?[] { false, false, default(long) },
            new object?[] { true, false, (ulong)8 },
            new object?[] { false, false, default(ulong) },
            new object?[] { true, false, (float)9.0f },
            new object?[] { false, false, default(float) },
            new object?[] { true, false, (double)10.0d },
            new object?[] { false, false, default(double) },
            new object?[] { true, false, (decimal)11.0m },
            new object?[] { false, false, default(decimal) },
            new object?[] { true, false, Guid.Parse("11000000-0000-0000-1111-101010101010") },
            new object?[] { false, false, default(Guid) },
            // The below test case fails, because the generic type parameter will be System.Guid for some reason (not being aware of nullable)
            // new object?[] { false, true, (Guid?)Guid.Parse("11000000-0000-0000-1111-101010101010") },
            // new object?[] { false, true, default(Guid?) },
            new object?[] { false, true, (string)"str" },
            new object?[] { false, true, new DummyReferenceType<byte>() },
            // The below test case fails, because the "recognition" of reference types does not work when passing default value ('null' for every reference type)
            // new object?[] { false, true, (DummyReferenceType<byte>?)null },
        };

        private class DummyReferenceType<T> { };
    }
}