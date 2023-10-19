using BookingWebAPI.Common.Constants;
using BookingWebAPI.Common.Enums;
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

        private IUserService _userService;
        private Mock<ISettingService> _settingServiceMock;
        private Mock<IBackgroundJobClient> _jobClientMock;
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<ISiteRepository> _siteRepositoryMock;

        [SetUp]
        public void SetUp()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _settingServiceMock = new Mock<ISettingService>();
            _jobClientMock = new Mock<IBackgroundJobClient>();
            _siteRepositoryMock = new Mock<ISiteRepository>();
            
            _userRepositoryMock.Setup(ur => ur.CreateOrUpdateAsync(It.IsAny<BookingWebAPIUser>())).ReturnsAsync((BookingWebAPIUser user) => user);
            _settingServiceMock.Setup(ss => ss.ExtractValueFromSetting<int>(It.IsAny<BookingWebAPISetting>())).Returns<BookingWebAPISetting>(s => Convert.ToInt32(s.RawValue));
            _settingServiceMock.Setup(ss => ss.ExtractValueFromSetting<double>(It.IsAny<BookingWebAPISetting>())).Returns<BookingWebAPISetting>(s => Convert.ToDouble(s.RawValue));
            _settingServiceMock.Setup(ss => ss.ExtractValueFromSetting<bool>(It.IsAny<BookingWebAPISetting>())).Returns<BookingWebAPISetting>(s => Convert.ToBoolean(s.RawValue));
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
        public async Task Register_Test(string emailAddress, bool emailAlreadyRegistered, bool siteExists, string firstName, string lastName, bool successExpected, string errorCodeExpected)
        {
            // prepare
            _userRepositoryMock.Setup(ur => ur.ExistsByEmail(It.IsAny<string>())).ReturnsAsync(emailAlreadyRegistered);
            _userRepositoryMock.Setup(ur => ur.GetAll()).Returns(
                (IQueryable<BookingWebAPIUser>)Enumerable.Empty<BookingWebAPIUser>().AsEnumerableMockWithAsyncQueryableSetup().Object
            );
            _siteRepositoryMock.Setup(sr => sr.ExistsAsync(It.IsAny<Guid>())).ReturnsAsync(siteExists);
            _userService = new UserService(JwtConfigMock, _userRepositoryMock.Object, _settingServiceMock.Object, _jobClientMock.Object, _siteRepositoryMock.Object);

            var siteId = Guid.NewGuid();

            // action & assert 
            var action = async () => await _userService.Register(emailAddress, siteId, firstName, lastName);
            if (successExpected)
            {
                var registeredUser = await action.Should().NotThrowAsync();
                registeredUser!.Which.Email.Should().Be(emailAddress.Trim());
                registeredUser!.Which.EmailConfirmed.Should().BeFalse();
                registeredUser!.Which.Token.Should().NotBeNull();
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
        public async Task Register_Test_EmailEnqueued(bool emailAlreadyRegistered)
        {
            // prepare
            _userRepositoryMock.Setup(ur => ur.ExistsByEmail(It.IsAny<string>())).ReturnsAsync(emailAlreadyRegistered);
            _userRepositoryMock.Setup(ur => ur.GetAll()).Returns(
                (IQueryable<BookingWebAPIUser>)Enumerable.Empty<BookingWebAPIUser>().AsEnumerableMockWithAsyncQueryableSetup().Object
            );
            _siteRepositoryMock.Setup(sr => sr.ExistsAsync(It.IsAny<Guid>())).ReturnsAsync(true);
            _userService = new UserService(JwtConfigMock, _userRepositoryMock.Object, _settingServiceMock.Object, _jobClientMock.Object, _siteRepositoryMock.Object);

            // action & assert
            var action = async () => await _userService.Register("testuser@testmailprovider.com", Guid.NewGuid(), "Test", "User");
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
                    _jobClientMock.Verify(jc => jc.Create(It.Is<Job>(job => job.Method.Name == "SendUserConfirmationEmail"), It.IsAny<IState>()), Times.Once());
                }
            }
            catch (MockException)
            {
                Assert.Fail("An email job has been enqueued for an existing user unexpectedly or an expected email job hasn't been enqued for a newly registered user.");
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task FindUserForEmailConfirmation_Test(bool userExists)
        {
            // prepare
            var userGuid = Guid.NewGuid();
            _userRepositoryMock.Setup(ur => ur.FindByEmailVerificationToken(userGuid)).ReturnsAsync(userExists ? new BookingWebAPIUser { Id = userGuid } : null);
            _userService = new UserService(JwtConfigMock, _userRepositoryMock.Object, _settingServiceMock.Object, _jobClientMock.Object, _siteRepositoryMock.Object);

            // action & assert
            var action = async () => await _userService.FindUserForEmailConfirmation(userGuid);
            if (userExists)
            {
              (await action.Should().NotThrowAsync()).Which.Id.Should().Be(userGuid);
            }
            else
            {
                await action.Should().ThrowAsync<BookingWebAPIException>();
            }
        }

        [TestCase(true, true, "Alma1234")]
        [TestCase(true, true, "")]
        [TestCase(true, false, "Alma1234")]
        [TestCase(true, false, "")]
        [TestCase(false, true, "Alma1234")]
        [TestCase(false, true, "")]
        [TestCase(false, false, "Alma1234")]
        [TestCase(false, false, "")]
        public async Task ConfirmRegistration_Test(bool userExists, bool tokenValid, string password)
        {
            // prepare
            var testUser = new BookingWebAPIUser
            {
                Id = Guid.NewGuid(), Email = "testUser@test.com", EmailConfirmed = false, AccessFailedCount = 0, FirstName = "Test", LastName = "Test", IsDeleted = false,
                LockoutEnabled = false, PasswordHash = null, SiteId = Guid.NewGuid(), Token = Guid.NewGuid(), UserName = "test", WorkHoursWeekly = 40
            };
            int minLength = 8, maxLength = 16;
            bool upperCase = true, specialChars = false, digits = true;
            _userRepositoryMock.Setup(ur => ur.ExistsAsync(testUser.Id)).ReturnsAsync(userExists);
            _userRepositoryMock.Setup(ur => ur.ExistsByEmailVerificationToken(testUser.Token.Value)).ReturnsAsync(tokenValid);
            _userRepositoryMock.Setup(ur => ur.GetAsync(testUser.Id)).ReturnsAsync(testUser);
            _settingServiceMock.Setup(ss => ss.GetSettingsForCategory(SettingCategory.PasswordPolicy)).ReturnsAsync(new BookingWebAPISetting[]
            {
                new BookingWebAPISetting { Name = ApplicationConstants.PasswordPolicyMinLength, Category = SettingCategory.PasswordPolicy, RawValue = $"{minLength}", ValueType = SettingValueType.Integer },
                new BookingWebAPISetting { Name = ApplicationConstants.PasswordPolicyMaxLength, Category = SettingCategory.PasswordPolicy, RawValue = $"{maxLength}", ValueType = SettingValueType.Integer },
                new BookingWebAPISetting { Name = ApplicationConstants.PasswordPolicyUppercaseLetter, Category = SettingCategory.PasswordPolicy, RawValue = $"{upperCase}", ValueType = SettingValueType.Boolean },
                new BookingWebAPISetting { Name = ApplicationConstants.PasswordPolicySpecialCharacters, Category = SettingCategory.PasswordPolicy, RawValue = $"{specialChars}", ValueType = SettingValueType.Boolean },
                new BookingWebAPISetting { Name = ApplicationConstants.PasswordPolicyDigits, Category = SettingCategory.PasswordPolicy, RawValue = $"{digits}", ValueType = SettingValueType.Boolean }
            });
           
            _userService = new UserService(JwtConfigMock, _userRepositoryMock.Object, _settingServiceMock.Object, _jobClientMock.Object, _siteRepositoryMock.Object);

            // action & assert
            var action = async () => await _userService.ConfirmRegistration(testUser.Id, testUser.Token.Value, password);

            var confirmRegistrationShouldFail = !userExists || !tokenValid || !PasswordCompliesToPolicy(password, minLength, maxLength, upperCase, specialChars, digits);
            if(confirmRegistrationShouldFail)
            {
                await action.Should().ThrowAsync<BookingWebAPIException>();
            }
            else
            {
                var foundUser = await action.Should().NotThrowAsync();

                var passwordCorrect = BCrypt.Net.BCrypt.Verify(password, foundUser!.Which.PasswordHash);
                passwordCorrect.Should().BeTrue();

                foundUser!.Which.Should().NotBeNull();
                foundUser!.Which.Id.Should().Be(testUser.Id);
                foundUser!.Which.EmailConfirmed.Should().BeTrue();
                foundUser!.Which.Token.Should().BeNull();
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
        public async Task Authenticate_Test(string emailAddress, bool emailRegistered, bool emailLockedOut, string password, bool passwordMatch, bool emailConfirmed, bool successExpected, string? errorCodeExpected)
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
            _userRepositoryMock.Setup(ur => ur.FindByUserEmail(emailAddress)).ReturnsAsync(emailRegistered ? mockUser : null);
            _userService = new UserService(JwtConfigMock, _userRepositoryMock.Object, _settingServiceMock.Object, _jobClientMock.Object, _siteRepositoryMock.Object);

            // action & assert
            var action = async () => await _userService.Authenticate(emailAddress, password);

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
                await action.Should().ThrowAsync<BookingWebAPIException>();
            }
        }

        [Test]
        public async Task Authenticate_Test_AccessFailedCount_Increment()
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
            _userRepositoryMock.Setup(ur => ur.FindByUserEmail(emailAddress)).ReturnsAsync(mockUser);
            _userService = new UserService(JwtConfigMock, _userRepositoryMock.Object, _settingServiceMock.Object, _jobClientMock.Object, _siteRepositoryMock.Object);

            // action
            string? errorCode = null;
            try
            {
                (_, _) = await _userService.Authenticate(emailAddress, password);
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
