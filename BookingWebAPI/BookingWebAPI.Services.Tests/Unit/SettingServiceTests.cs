using BookingWebAPI.Common.Enums;
using BookingWebAPI.Common.ErrorCodes;
using BookingWebAPI.Common.Exceptions;
using BookingWebAPI.Common.Models;
using BookingWebAPI.DAL.Interfaces;
using BookingWebAPI.Testing.Common;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System.Reflection;

namespace BookingWebAPI.Services.Tests.Unit
{
    internal class SettingServiceTests : UnitTestBase
    {
        private Mock<ISettingRepository> _settingRepositoryMock;
        private SettingService _settingService;

        [SetUp]
        public void SetUp()
        {
            _settingRepositoryMock = new Mock<ISettingRepository>();
            _settingService = new SettingService(_settingRepositoryMock.Object);
        }

        [TestCase(SettingValueType.Boolean, "True")]
        [TestCase(SettingValueType.Boolean, "False")]
        [TestCase(SettingValueType.Boolean, "true")]
        [TestCase(SettingValueType.Boolean, "false")]
        [TestCase(SettingValueType.Boolean, "1", ApplicationErrorCodes.SettingWrongSettingType)]
        [TestCase(SettingValueType.Boolean, "0", ApplicationErrorCodes.SettingWrongSettingType)]
        [TestCase(SettingValueType.Boolean, "", ApplicationErrorCodes.SettingWrongSettingType)]
        [TestCase(SettingValueType.Boolean, null, ApplicationErrorCodes.SettingWrongSettingType)]
        [TestCase(SettingValueType.Integer, "795")]
        [TestCase(SettingValueType.Integer, "795.211", ApplicationErrorCodes.SettingWrongSettingType)]
        [TestCase(SettingValueType.Integer, "six", ApplicationErrorCodes.SettingWrongSettingType)]
        [TestCase(SettingValueType.Integer, "", ApplicationErrorCodes.SettingWrongSettingType)]
        [TestCase(SettingValueType.Integer, null, ApplicationErrorCodes.SettingWrongSettingType)]
        [TestCase(SettingValueType.Float, "950")]
        [TestCase(SettingValueType.Float, "950.775")]
        [TestCase(SettingValueType.Float, "fifty", ApplicationErrorCodes.SettingWrongSettingType)]
        [TestCase(SettingValueType.Float, "", ApplicationErrorCodes.SettingWrongSettingType)]
        [TestCase(SettingValueType.Float, null, ApplicationErrorCodes.SettingWrongSettingType)]
        [TestCase(SettingValueType.String, "apples")]
        [TestCase(SettingValueType.String, "")]
        [TestCase(SettingValueType.String, null)]
        //[TestCase(0, "", false, ApplicationErrorCodes.SettingNotKnownSettingType)]
        public void ExtractValueFromSetting_Test(SettingValueType valueType, string rawValue, string? errorCodeExpected = null)
        {
            // prepare - id, category and name are not relevant to this test thus they are hardcoded
            var settingToTest = new BookingWebAPISetting { Id = Guid.NewGuid(), Category = SettingCategory.Email, Name = "TestSetting", ValueType = valueType, RawValue = rawValue };

            var extractValueFromSettingMethod = typeof(SettingService).GetMethods()
                .SingleOrDefault(m => m.Name == nameof(SettingService.ExtractValueFromSetting) && m.IsGenericMethodDefinition);

            if(extractValueFromSettingMethod == null)
            {
                Assert.Fail($"Could not prepare test. Method {nameof(SettingService.ExtractValueFromSetting)} could not been found.");
            }

            var genericMethod = extractValueFromSettingMethod!.MakeGenericMethod(DotNetType(valueType));

            // action
            string? errorCode = null;
            try
            {
                 genericMethod.Invoke(_settingService, new[] { settingToTest } );
            }
            catch (TargetInvocationException t) when (t.InnerException != null && t.InnerException is BookingWebAPIException)
            {
                errorCode = (t.InnerException as BookingWebAPIException)!.ErrorCode;
            }

            // assert
            errorCode.Should().Be(errorCodeExpected);
        }

        [Test]
        public void ExtractValueFromSetting_Test_WrongGenericType()
        {
            // prepare
            var settingToTest = new BookingWebAPISetting { Id = Guid.NewGuid(), Category = SettingCategory.Email, Name = "TestSetting", ValueType = SettingValueType.Integer, RawValue = "75" };

            // action
            string? errorCode = null;
            try
            {
                _settingService.ExtractValueFromSetting<bool>(settingToTest);
            }
            catch (BookingWebAPIException e)
            {
                errorCode = e.ErrorCode;
            }

            // assert
            errorCode.Should().Be(ApplicationErrorCodes.SettingWrongSettingType);
        }

        [Test]
        public void ExtractValueFromSetting_Test_NotExistingSettingValueType()
        {
            // prepare
            var settingToTest = new BookingWebAPISetting { Id = Guid.NewGuid(), Category = SettingCategory.Email, Name = "TestSetting", ValueType = (SettingValueType)0, RawValue = "75" };

            // action
            string? errorCode = null;
            try
            {
                _settingService.ExtractValueFromSetting<int>(settingToTest);
            }
            catch (BookingWebAPIException e)
            {
                errorCode = e.ErrorCode;
            }

            // assert
            errorCode.Should().Be(ApplicationErrorCodes.SettingNotKnownSettingType);
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task GetValueBySettingNameAsync_Test(bool existingSettingName)
        {
            // prepare
            var settingName = "TestSetting";
            var settingValueType = SettingValueType.Float;
            var settingValue = 75.386f;
            var settingToTest = new BookingWebAPISetting { Id = Guid.NewGuid(), Category = SettingCategory.Email, Name = settingName, ValueType = settingValueType, RawValue = $"{settingValue}" };

            _settingRepositoryMock.Setup(sr => sr.GetSettingByNameAsync(settingName)).ReturnsAsync(settingToTest);
            _settingService = new SettingService(_settingRepositoryMock.Object);

            // action
            string? errorCode = null;
            float? extractedValue = null;
            try
            {
                extractedValue = await _settingService.GetValueBySettingNameAsync<float>($"{settingName}{(existingSettingName ? string.Empty : "Ruined")}");
            }
            catch (BookingWebAPIException e)
            {
                errorCode = e.ErrorCode;
            }

            // assert
            errorCode.Should().Be(existingSettingName ? null : ApplicationErrorCodes.EntityNotFound);
            if (existingSettingName) 
            {
                extractedValue.Should().Be(settingValue);
            }
        }

        private static Type DotNetType(SettingValueType settingValueType) => settingValueType switch
        {
            SettingValueType.Boolean => typeof(bool),
            SettingValueType.Integer => typeof(int),
            SettingValueType.Float => typeof(float),
            SettingValueType.String => typeof(string),
            _ => throw new ArgumentException($"Not a known element of type '{nameof(SettingValueType)}'.", nameof(settingValueType))
        };
    }
}
