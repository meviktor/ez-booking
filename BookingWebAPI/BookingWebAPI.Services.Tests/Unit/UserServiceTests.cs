using BookingWebAPI.Common.Constants;
using BookingWebAPI.Common.ErrorCodes;
using BookingWebAPI.Common.Exceptions;
using BookingWebAPI.Common.Models;
using BookingWebAPI.DAL.Interfaces;
using BookingWebAPI.Services.Interfaces;
using BookingWebAPI.Testing.Common;
using FluentAssertions;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Moq;
using NUnit.Framework;

namespace BookingWebAPI.Services.Tests.Unit
{
    internal class UserServiceTests : UnitTestBase
    {
        private const string _emailAddressLongerThan320Chars = "thisEmailAddressIsLongerThanTheMaximum320CharactersAaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa@b.com";
        private const int _mockPasswordLength = 8;

        private IUserService _userService;
        private Mock<ISettingService> _settingServiceMock;
        private Mock<IBackgroundJobClient> _jobClientMock;
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<ISiteRepository> _siteRepositoryMock;
        private Mock<IEmailConfirmationAttemptService> _emailConfirmationAttemptService;

        [SetUp]
        public void SetUp()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _settingServiceMock = new Mock<ISettingService>();
            _siteRepositoryMock = new Mock<ISiteRepository>();
            _emailConfirmationAttemptService = new Mock<IEmailConfirmationAttemptService>();
            _jobClientMock = new Mock<IBackgroundJobClient>();
            _emailConfirmationAttemptService = new Mock<IEmailConfirmationAttemptService>();
            
            _userRepositoryMock.Setup(ur => ur.CreateOrUpdateAsync(It.IsAny<BookingWebAPIUser>())).ReturnsAsync((BookingWebAPIUser user) => user);
            _settingServiceMock.Setup(ss => ss.ExtractValueFromSetting<int>(It.IsAny<BookingWebAPISetting>())).Returns<BookingWebAPISetting>(s => Convert.ToInt32(s.RawValue));
            _settingServiceMock.Setup(ss => ss.ExtractValueFromSetting<double>(It.IsAny<BookingWebAPISetting>())).Returns<BookingWebAPISetting>(s => Convert.ToDouble(s.RawValue));
            _settingServiceMock.Setup(ss => ss.ExtractValueFromSetting<bool>(It.IsAny<BookingWebAPISetting>())).Returns<BookingWebAPISetting>(s => Convert.ToBoolean(s.RawValue));
            _settingServiceMock.Setup(ss => ss.GetValueBySettingNameAsync<int>(ApplicationConstants.PasswordPolicyMinLength)).ReturnsAsync(_mockPasswordLength);
        }

        #region Register_Test test cases
        [TestCase("no@whitespaces.com", false, true, "John", "Doe", true, null)]
        [TestCase(" trailing@andleadingwhitespace.com  ", true, true, "John", "Doe", false, ApplicationErrorCodes.UserEmailInvalidFormat)]
        [TestCase("no@whitespaces.com", true, true, "John", "Doe", false, ApplicationErrorCodes.UserEmailMustBeUnique)]
        [TestCase(" ", false, true, "John", "Doe", false, ApplicationErrorCodes.UserEmailInvalidFormat)]
        [TestCase("/t  ", false, true, "John", "Doe", false, ApplicationErrorCodes.UserEmailInvalidFormat)]
        [TestCase(null, false, true, "John", "Doe", false, ApplicationErrorCodes.UserEmailInvalidFormat)]
        [TestCase("withoutatanddomain", false, true, "John", "Doe", false, ApplicationErrorCodes.UserEmailInvalidFormat)]
        [TestCase("withoutat.domain", false, true, "John", "Doe", false, ApplicationErrorCodes.UserEmailInvalidFormat)]
        [TestCase(_emailAddressLongerThan320Chars, false, true, "John", "Doe", false, ApplicationErrorCodes.UserEmailInvalidFormat)]
        [TestCase("no@whitespaces.com", true, false, "John", "Doe", false, ApplicationErrorCodes.UserEmailMustBeUnique)]
        [TestCase("no@whitespaces.com", false, false, "John", "Doe", false, ApplicationErrorCodes.SiteDoesNotExist)]
        [TestCase(" ", false, false, "John", "Doe", false, ApplicationErrorCodes.UserEmailInvalidFormat)]
        [TestCase("/t  ", false, false, "John", "Doe", false, ApplicationErrorCodes.UserEmailInvalidFormat)]
        [TestCase("withoutatanddomain", false, false, "John", "Doe", false, ApplicationErrorCodes.UserEmailInvalidFormat)]
        [TestCase("withoutat.domain", false, false, "John", "Doe", false, ApplicationErrorCodes.UserEmailInvalidFormat)]
        [TestCase(_emailAddressLongerThan320Chars, false, false, "John", "Doe", false, ApplicationErrorCodes.UserEmailInvalidFormat)]
        [TestCase("no@whitespaces.com", true, true, "", "Doe", false, ApplicationErrorCodes.UserFirstNameRequired)]
        [TestCase(" trailing@andleadingwhitespace.com  ", true, true, " ", "Doe", false, ApplicationErrorCodes.UserFirstNameRequired)]
        [TestCase("no@whitespaces.com", false, true, "\t", "Doe", false, ApplicationErrorCodes.UserFirstNameRequired)]
        [TestCase(" trailing@andleadingwhitespace.com  ", false, true, "\v", "Doe", false, ApplicationErrorCodes.UserFirstNameRequired)]
        [TestCase(" ", false, true, null, "Doe", false, ApplicationErrorCodes.UserFirstNameRequired)]
        [TestCase("/t  ", false, true, "\v", "Doe", false, ApplicationErrorCodes.UserFirstNameRequired)]
        [TestCase(null, false, true, "", "Doe", false, ApplicationErrorCodes.UserFirstNameRequired)]
        [TestCase("withoutatanddomain", false, true, "", "Doe", false, ApplicationErrorCodes.UserFirstNameRequired)]
        [TestCase("withoutat.domain", false, true, "", "Doe", false, ApplicationErrorCodes.UserFirstNameRequired)]
        [TestCase(_emailAddressLongerThan320Chars, false, true, "\t\v ", "Doe", false, ApplicationErrorCodes.UserFirstNameRequired)]
        [TestCase("no@whitespaces.com", true, false, null, "Doe", false, ApplicationErrorCodes.UserFirstNameRequired)]
        [TestCase(" trailing@andleadingwhitespace.com  ", true, false, "", "Doe", false, ApplicationErrorCodes.UserFirstNameRequired)]
        [TestCase(" ", false, false, "", "Doe", false, ApplicationErrorCodes.UserFirstNameRequired)]
        [TestCase("/t  ", false, false, null, "Doe", false, ApplicationErrorCodes.UserFirstNameRequired)]
        [TestCase(null, false, false, "", "Doe", false, ApplicationErrorCodes.UserFirstNameRequired)]
        [TestCase("withoutatanddomain", false, false, " ", "Doe", false, ApplicationErrorCodes.UserFirstNameRequired)]
        [TestCase("withoutat.domain", false, false, "", "Doe", false, ApplicationErrorCodes.UserFirstNameRequired)]
        [TestCase(_emailAddressLongerThan320Chars, false, false, null, "Doe", false, ApplicationErrorCodes.UserFirstNameRequired)]
        [TestCase("no@whitespaces.com", true, true, "John", "", false, ApplicationErrorCodes.UserLastNameRequired)]
        [TestCase(" trailing@andleadingwhitespace.com  ", true, true, "John", " ", false, ApplicationErrorCodes.UserLastNameRequired)]
        [TestCase("no@whitespaces.com", false, true, "John", "\t", false, ApplicationErrorCodes.UserLastNameRequired)]
        [TestCase(" trailing@andleadingwhitespace.com  ", false, true, "John", "\v", false, ApplicationErrorCodes.UserLastNameRequired)]
        [TestCase(" ", false, true, "John", null, false, ApplicationErrorCodes.UserLastNameRequired)]
        [TestCase("/t  ", false, true, "John", "", false, ApplicationErrorCodes.UserLastNameRequired)]
        [TestCase(null, false, true, "John", " \t \v", false, ApplicationErrorCodes.UserLastNameRequired)]
        [TestCase("withoutatanddomain", false, true, "John", "\f", false, ApplicationErrorCodes.UserLastNameRequired)]
        [TestCase("withoutat.domain", false, true, "John", null, false, ApplicationErrorCodes.UserLastNameRequired)]
        [TestCase(_emailAddressLongerThan320Chars, false, true, "John", "", false, ApplicationErrorCodes.UserLastNameRequired)]
        [TestCase("no@whitespaces.com", true, false, "John", null, false, ApplicationErrorCodes.UserLastNameRequired)]
        [TestCase(" trailing@andleadingwhitespace.com  ", true, false, "John", "", false, ApplicationErrorCodes.UserLastNameRequired)]
        [TestCase(" ", false, false, "John", "", false, ApplicationErrorCodes.UserLastNameRequired)]
        [TestCase("/t  ", false, false, "John", null, false, ApplicationErrorCodes.UserLastNameRequired)]
        [TestCase(null, false, false, "John", null, false, ApplicationErrorCodes.UserLastNameRequired)]
        [TestCase("withoutatanddomain", false, false, "John", "", false, ApplicationErrorCodes.UserLastNameRequired)]
        [TestCase("withoutat.domain", false, false, "John", " \v \f", false, ApplicationErrorCodes.UserLastNameRequired)]
        [TestCase(_emailAddressLongerThan320Chars, false, false, "John", "", false, ApplicationErrorCodes.UserLastNameRequired)]
        [TestCase("no@whitespaces.com", true, true, "", "", false, ApplicationErrorCodes.UserFirstNameRequired)]
        [TestCase(" trailing@andleadingwhitespace.com  ", true, true, " ", "\t", false, ApplicationErrorCodes.UserFirstNameRequired)]
        [TestCase("no@whitespaces.com", false, true, "\t", "", false, ApplicationErrorCodes.UserFirstNameRequired)]
        [TestCase(" trailing@andleadingwhitespace.com  ", false, true, "\v", "", false, ApplicationErrorCodes.UserFirstNameRequired)]
        [TestCase(" ", false, true, null, null, false, ApplicationErrorCodes.UserFirstNameRequired)]
        [TestCase("/t  ", false, true, " ", "", false, ApplicationErrorCodes.UserFirstNameRequired)]
        [TestCase(null, false, true, "", "", false, ApplicationErrorCodes.UserFirstNameRequired)]
        [TestCase("withoutatanddomain", false, true, "", "\n", false, ApplicationErrorCodes.UserFirstNameRequired)]
        [TestCase("withoutat.domain", false, true, "", "\r\n", false, ApplicationErrorCodes.UserFirstNameRequired)]
        [TestCase(_emailAddressLongerThan320Chars, false, true, "\t\v \t", "\r", false, ApplicationErrorCodes.UserFirstNameRequired)]
        [TestCase("no@whitespaces.com", true, false, "\n", null, false, ApplicationErrorCodes.UserFirstNameRequired)]
        [TestCase(" trailing@andleadingwhitespace.com  ", true, false, "", "\r", false, ApplicationErrorCodes.UserFirstNameRequired)]
        [TestCase(" ", false, false, "", "\r\n", false, ApplicationErrorCodes.UserFirstNameRequired)]
        [TestCase("/t  ", false, false, null, " ", false, ApplicationErrorCodes.UserFirstNameRequired)]
        [TestCase(null, false, false, "", "", false, ApplicationErrorCodes.UserFirstNameRequired)]
        [TestCase("withoutatanddomain", false, false, " ", "\t\v \n", false, ApplicationErrorCodes.UserFirstNameRequired)]
        [TestCase("withoutat.domain", false, false, "", null, false, ApplicationErrorCodes.UserFirstNameRequired)]
        [TestCase(_emailAddressLongerThan320Chars, false, false, null, "", false, ApplicationErrorCodes.UserFirstNameRequired)]
        #endregion
        public async Task RegisterAsync_Test(string emailAddress, bool emailAlreadyRegistered, bool siteExists, string firstName, string lastName, bool successExpected, string errorCodeExpected)
        {
            // prepare
            _userRepositoryMock.Setup(ur => ur.ExistsByEmailAsync(It.IsAny<string>())).ReturnsAsync(emailAlreadyRegistered);
            _userRepositoryMock.Setup(ur => ur.GetAll()).Returns(
                (IQueryable<BookingWebAPIUser>)Enumerable.Empty<BookingWebAPIUser>().AsEnumerableMockWithAsyncQueryableSetup().Object
            );
            _siteRepositoryMock.Setup(sr => sr.ExistsAsync(It.IsAny<Guid>())).ReturnsAsync(siteExists);
            _userService = new UserService(JwtConfigMock, _userRepositoryMock.Object, _settingServiceMock.Object, _jobClientMock.Object, _siteRepositoryMock.Object, _emailConfirmationAttemptService.Object, TransactionManagerMock);

            var siteId = Guid.NewGuid();

            // action & assert 
            var action = async () => await _userService.RegisterAsync(emailAddress, siteId, firstName, lastName);
            if (successExpected)
            {
                var registeredUser = await action.Should().NotThrowAsync();
                registeredUser!.Which.Email.Should().Be(emailAddress.Trim());
                registeredUser!.Which.EmailConfirmed.Should().BeFalse();
                registeredUser!.Which.UserName.Should().NotBeNull();
                registeredUser!.Which.FirstName.Should().Be(firstName.Trim());
                registeredUser!.Which.LastName.Should().Be(lastName.Trim());
                registeredUser!.Which.SiteId.Should().Be(siteId);
            }
            else
            {
                (await action.Should().ThrowAsync<BookingWebAPIException>()).Which.ErrorCode.Should().Be(errorCodeExpected);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task RegisterAsync_Test_EmailEnqueued(bool emailAlreadyRegistered)
        {
            // prepare
            _userRepositoryMock.Setup(ur => ur.ExistsByEmailAsync(It.IsAny<string>())).ReturnsAsync(emailAlreadyRegistered);
            _userRepositoryMock.Setup(ur => ur.GetAll()).Returns(
                (IQueryable<BookingWebAPIUser>)Enumerable.Empty<BookingWebAPIUser>().AsEnumerableMockWithAsyncQueryableSetup().Object
            );
            _siteRepositoryMock.Setup(sr => sr.ExistsAsync(It.IsAny<Guid>())).ReturnsAsync(true);
            _userService = new UserService(JwtConfigMock, _userRepositoryMock.Object, _settingServiceMock.Object, _jobClientMock.Object, _siteRepositoryMock.Object, _emailConfirmationAttemptService.Object, TransactionManagerMock);

            // action & assert
            var action = async () => await _userService.RegisterAsync("testuser@testmailprovider.com", Guid.NewGuid(), "Test", "User");
            if (emailAlreadyRegistered)
            {
                await action.Should().ThrowAsync<BookingWebAPIException>();
            }
            else
            {
                await action.Should().NotThrowAsync();
            }

            try
            {
                if (emailAlreadyRegistered)
                {
                    _jobClientMock.Verify(jc => jc.Create(It.IsAny<Job>(), It.IsAny<IState>()), Times.Never());
                }
                else
                {
                    _jobClientMock.Verify(jc => jc.Create(It.Is<Job>(job => job.Method.Name == "SendUserConfirmationEmailAsync"), It.IsAny<IState>()), Times.Once());
                }
            }
            catch (MockException)
            {
                Assert.Fail("An email job has been enqueued for an existing user unexpectedly or an expected email job hasn't been enqued for a newly registered user.");
            }
        }

        [TestCase(null, false, false, null, false, false, false, ApplicationErrorCodes.UserEmailRequired)]
        [TestCase("", false, false, "", false, false, false, ApplicationErrorCodes.UserEmailRequired)]
        [TestCase(" ", false, false, " ", false, false, false, ApplicationErrorCodes.UserEmailRequired)]
        [TestCase("noatinit.mailprovider.com", false, false, "\t", false, false, false, ApplicationErrorCodes.UserEmailInvalidFormat)]
        [TestCase("goodmail@mailprovider.com", false, false, "\v", false, false, false, ApplicationErrorCodes.LoginPasswordRequired)]
        [TestCase("goodmail@mailprovider.com", true, false, "\f", false, false, false, ApplicationErrorCodes.LoginPasswordRequired)]
        [TestCase("goodmail@mailprovider.com", true, false, "", false, false, false, ApplicationErrorCodes.LoginPasswordRequired)]
        [TestCase("goodmail@mailprovider.com", true, false, null, false, false, false, ApplicationErrorCodes.LoginPasswordRequired)]
        [TestCase("goodmail@mailprovider.com", true, false, "doesntMatterWhatIsHereAsUserDoesntHaveAPasswordYet!", false, false, false, ApplicationErrorCodes.LoginEmailNotConfirmed)]
        [TestCase("goodmail@mailprovider.com", true, false, "matchingPassword!", false, true, false, ApplicationErrorCodes.LoginInvalidUserNameOrPassword)]
        [TestCase("goodmail@mailprovider.com", true, true, "matchingPassword!", true, true, false, ApplicationErrorCodes.UserLockedOut)]
        [TestCase("goodmail@mailprovider.com", true, true, "notMatchingPassword!", false, true, false, ApplicationErrorCodes.UserLockedOut)]
        [TestCase("goodmail@mailprovider.com", true, false, "matchingPassword!", true, true, true, null)]
        public async Task AuthenticateAsync_Test(string emailAddress, bool emailRegistered, bool emailLockedOut, string password, bool passwordMatch, bool emailConfirmed, bool successExpected, string? errorCodeExpected)
        {
            // prepare
            var mockUserId = Guid.NewGuid();
            var mockUser = new BookingWebAPIUser
            {
                Id = mockUserId,
                Email = emailAddress,
                LockoutEnabled = emailLockedOut,
                AccessFailedCount = 0,
                EmailConfirmed = emailConfirmed,
                PasswordHash = emailConfirmed ?
                    (passwordMatch ? BCrypt.Net.BCrypt.HashPassword(password) : BCrypt.Net.BCrypt.HashPassword($"{password}ruinPassword")) :
                    null
            };
            _userRepositoryMock.Setup(ur => ur.FindByUserEmailAsync(emailAddress)).ReturnsAsync(emailRegistered ? mockUser : null);
            _userService = new UserService(JwtConfigMock, _userRepositoryMock.Object, _settingServiceMock.Object, _jobClientMock.Object, _siteRepositoryMock.Object, _emailConfirmationAttemptService.Object, TransactionManagerMock);

            // action & assert
            var action = async () => await _userService.AuthenticateAsync(emailAddress, password);

            if (successExpected)
            {
                var userTokenTuple = await action.Should().NotThrowAsync();
                // found user
                userTokenTuple.Which.Item1.Id.Should().Be(mockUserId);
                userTokenTuple.Which.Item1.Email.Should().Be(emailAddress);
                // JWT
                userTokenTuple.Which.Item2.Should().NotBeNull();
            }
            else
            {
                (await action.Should().ThrowAsync<BookingWebAPIException>()).Which.ErrorCode.Should().Be(errorCodeExpected);
            }
        }

        [Test]
        public async Task AuthenticateAsync_Test_AccessFailedCount_Increment()
        {
            // prepare
            var emailAddress = "test@test.com";
            var password = "niceTry!";
            var originalAccessFailedCount = 0;
            var mockUser = new BookingWebAPIUser
            {
                Id = Guid.NewGuid(),
                Email = emailAddress,
                LockoutEnabled = false,
                AccessFailedCount = originalAccessFailedCount,
                EmailConfirmed = true,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword($"{password}ruinPassword")
            };
            _userRepositoryMock.Setup(ur => ur.FindByUserEmailAsync(emailAddress)).ReturnsAsync(mockUser);
            _userService = new UserService(JwtConfigMock, _userRepositoryMock.Object, _settingServiceMock.Object, _jobClientMock.Object, _siteRepositoryMock.Object, _emailConfirmationAttemptService.Object, TransactionManagerMock);

            // action
            string? errorCode = null;
            try
            {
                (_, _) = await _userService.AuthenticateAsync(emailAddress, password);
            }
            catch (BookingWebAPIException ex)
            {
                errorCode = ex.ErrorCode;
            }

            // assert
            errorCode.Should().Be(ApplicationErrorCodes.LoginInvalidUserNameOrPassword);
            mockUser.AccessFailedCount.Should().Be(originalAccessFailedCount + 1);
        }

        private static bool PasswordCompliesToPolicy(string password, int minLength, int maxLength, bool upperCase, bool specialChar, bool digits)
        {
            var lengthCriteria = password.Length >= minLength && password.Length <= maxLength;
            var upperCaseCriteria = (!upperCase || !password.Equals(password.ToLower()));
            var specialCharCriteria = (!specialChar || password.Any(letter => // a special character, so:
                    (letter < '0' || letter > '9') && // not a number
                    (letter < 'a' || letter > 'z') && // not a lower case letter
                    (letter < 'A' || letter > 'Z') && // not an upper case letter
                    !new char[] { ' ', '\n', '\t', '\r', '\f' }.Contains(letter) // not a space character 
                ));
            var digitCriteria = (!digits || password.Any(letter => letter >= '0' && letter <= '9'));
            return lengthCriteria && upperCaseCriteria && specialCharCriteria && digitCriteria;
        }
    }
}
