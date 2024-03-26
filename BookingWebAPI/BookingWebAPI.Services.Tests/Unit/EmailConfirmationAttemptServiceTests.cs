using BookingWebAPI.Common.Enums;
using BookingWebAPI.Common.ErrorCodes;
using BookingWebAPI.Common.Exceptions;
using BookingWebAPI.Common.Models;
using BookingWebAPI.DAL.Interfaces;
using BookingWebAPI.Testing.Common;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace BookingWebAPI.Services.Tests.Unit
{
    internal class EmailConfirmationAttemptServiceTests : UnitTestBase
    {
        private const EmailConfirmationStatus _testStatus = EmailConfirmationStatus.Initiated;
        private const string _testAttemptId = "646bdd3b-18d3-4be7-ae0b-66c013951da4";
        private const string _invalidAttemptId = "25ecdf64-0434-4bfd-82e1-4cc35a0643cd";
        private static readonly IEnumerable<EmailConfirmationAttempt> _testAttempts = new EmailConfirmationAttempt[]
        {
            new EmailConfirmationAttempt { Id = Guid.Parse(_testAttemptId), CreatedAt = DateTimeOffset.UtcNow, Status = _testStatus, UserId = Guid.Parse(TestDatabaseSeeder.Constants.ActiveUserId) }
        };

        private Mock<IEmailConfirmationAttemptRepository> _emailConfirmationAttempRepositoryMock;
        private EmailConfirmationAttemptService _emailConfirmationAttemptService;

        [SetUp]
        public void SetUp()
        {
            _emailConfirmationAttempRepositoryMock = new Mock<IEmailConfirmationAttemptRepository>();
            _emailConfirmationAttempRepositoryMock.Setup(r => r.GetAsync(It.IsAny<Guid>())).ReturnsAsync((Guid id) => _testAttempts.SingleOrDefault(a => a.Id == id));

            _emailConfirmationAttemptService = new EmailConfirmationAttemptService(_emailConfirmationAttempRepositoryMock.Object);
        }

        [TestCase(_testAttemptId, new[] { _testStatus, EmailConfirmationStatus.Failed }, true)]
        [TestCase(_invalidAttemptId, new[] { _testStatus, EmailConfirmationStatus.Failed }, false)]
        [TestCase(_testAttemptId, new[] { EmailConfirmationStatus.Succeeded }, false)]
        [TestCase(_invalidAttemptId, new[] { EmailConfirmationStatus.Succeeded }, false)]
        public async Task GetInStatusAsync_Test(string idAsStr, IEnumerable<EmailConfirmationStatus> acceptableStatuses, bool attemptShouldBeFound)
        {
            var id = Guid.Parse(idAsStr);
            var action = async () => await _emailConfirmationAttemptService.GetInStatusAsync(id, acceptableStatuses);
           
            if (attemptShouldBeFound)
            {
                (await action()).Should().Match((EmailConfirmationAttempt a) => a != null && a.Id == id);
            }
            else
            {
                await action.Should().ThrowAsync<BookingWebAPIException>().Where(e => e.ErrorCode == ApplicationErrorCodes.EmailConfirmationInvalidAttempt);
            }
        }
    }
}
