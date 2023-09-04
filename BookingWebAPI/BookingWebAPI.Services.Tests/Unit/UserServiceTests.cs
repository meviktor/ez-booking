using BookingWebAPI.Common.Exceptions;
using BookingWebAPI.Common.Models;
using BookingWebAPI.DAL.Interfaces;
using BookingWebAPI.Services.Interfaces;
using BookingWebAPI.Testing.Common;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using NUnit.Framework;
using System.Linq.Expressions;

namespace BookingWebAPI.Services.Tests.Unit
{
    internal class UserServiceTests : UnitTestBase
    {
        private IUserService _userService;
        private Mock<ISettingService> _settingServiceMock;
        private Mock<IBackgroundJobClient> _jobClientMock;
        private Mock<IUserRepository> _userRepositoryMock;

        [SetUp]
        public void SetUp()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _settingServiceMock = new Mock<ISettingService>();
            _jobClientMock = new Mock<IBackgroundJobClient>();
            
            _userRepositoryMock.Setup(ur => ur.CreateOrUpdateAsync(It.IsAny<BookingWebAPIUser>())).ReturnsAsync((BookingWebAPIUser user) => user);
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task Register_Test(bool emailAlreadyRegistered)
        {
            // prepare
            _userRepositoryMock.Setup(ur => ur.ExistsByEmail(It.IsAny<string>())).ReturnsAsync(emailAlreadyRegistered);
            _userRepositoryMock.Setup(ur => ur.GetAll()).Returns(
                (IQueryable<BookingWebAPIUser>)Enumerable.Empty<BookingWebAPIUser>().AsEnumerableMockWithAsyncQueryableSetup().Object
            );
            _userService = new UserService(JwtConfigMock, _userRepositoryMock.Object, _settingServiceMock.Object, _jobClientMock.Object);

            var registrationEmail = "testuser@testmailprovider.com   ";
            var registrationSiteId = Guid.NewGuid();
            var firstName = " Test";
            var lastName = "  User ";

            // action
            var exceptionOccurred = false;
            BookingWebAPIUser? registeredUser = null;
            try
            {
                registeredUser = await _userService.Register(registrationEmail, registrationSiteId, firstName, lastName);
            }
            catch (BookingWebAPIException)
            {
                exceptionOccurred = true;
            }

            // assert
            Assert.That(exceptionOccurred, emailAlreadyRegistered ? Is.True : Is.False);
            if(!emailAlreadyRegistered)
            {
                Assert.That(registeredUser!.Email, Is.EqualTo(registrationEmail.Trim()));
                Assert.That(registeredUser!.EmailConfirmed, Is.False);
                Assert.That(registeredUser!.Token, Is.Not.Null);
                Assert.That(registeredUser!.UserName, Is.Not.Null);
                Assert.That(registeredUser!.FirstName, Is.EqualTo(firstName.Trim()));
                Assert.That(registeredUser!.LastName, Is.EqualTo(lastName.Trim()));
                Assert.That(registeredUser!.SiteId, Is.EqualTo(registrationSiteId));
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
            _userService = new UserService(JwtConfigMock, _userRepositoryMock.Object, _settingServiceMock.Object, _jobClientMock.Object);

            // action
            var exceptionOccurred = false;
            BookingWebAPIUser? registeredUser = null;
            try
            {
                registeredUser = await _userService.Register("testuser@testmailprovider.com", Guid.NewGuid(), "Test", "User");
            }
            catch (BookingWebAPIException)
            {
                exceptionOccurred = true;
            }

            // assert
            Assert.That(exceptionOccurred, Is.EqualTo(emailAlreadyRegistered));
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
    }
}
