﻿using BookingWebAPI.Common.Constants;
using BookingWebAPI.Common.ErrorCodes;
using BookingWebAPI.Common.Models;
using BookingWebAPI.DAL.Enums;
using BookingWebAPI.DAL.Infrastructure;
using BookingWebAPI.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookingWebAPI.DAL.Repositories
{
    internal class UserRepository : CRUDRepository<BookingWebAPIUser>, IUserRepository
    {
        public UserRepository(BookingWebAPIDbContext dbContext) 
            : base(dbContext)
        {}

        protected override IEnumerable<ErrorCodeAssociation> ErrorCodeAssosications => new ErrorCodeAssociation[]
        {
            new ErrorCodeAssociation(DatabaseConstraintNames.User_UserName_UQ, SqlServerErrorCode.CannotInsertDuplicate, ApplicationErrorCodes.UserUserNameMustBeUnique),
            new ErrorCodeAssociation(DatabaseConstraintNames.User_Email_UQ, SqlServerErrorCode.CannotInsertDuplicate, ApplicationErrorCodes.UserEmailMustBeUnique),
            new ErrorCodeAssociation(nameof(BookingWebAPIUser.Email), SqlServerErrorCode.CannotInsertNull, ApplicationErrorCodes.UserEmailRequired),
            new ErrorCodeAssociation(nameof(BookingWebAPIUser.Email), SqlServerErrorCode.StringOrBinaryTruncated, ApplicationErrorCodes.UserEmailTooLong),
            new ErrorCodeAssociation(nameof(BookingWebAPIUser.UserName), SqlServerErrorCode.CannotInsertNull, ApplicationErrorCodes.UserUserNameRequired),
            new ErrorCodeAssociation(nameof(BookingWebAPIUser.UserName), SqlServerErrorCode.StringOrBinaryTruncated, ApplicationErrorCodes.UserUserNameTooLong)
        };

        public async Task<BookingWebAPIUser?> FindByUserEmail(string emailAddress) => await Set.SingleOrDefaultAsync(user => !user.IsDeleted && user.Email == emailAddress);

        public async Task<BookingWebAPIUser?> FindByEmailVerificationToken(Guid token) => await Set.SingleOrDefaultAsync(user => !user.IsDeleted && !user.EmailConfirmed && user.Token == token);

        public async Task<bool> ExistsByEmailVerificationToken(Guid token) => await Set.AnyAsync(user => !user.IsDeleted && !user.EmailConfirmed && user.Token == token);

        public async Task<bool> ExistsByUserName(string userName) => await Set.AnyAsync(user => !user.IsDeleted && user.UserName == userName);

        public async Task<bool> ExistsByEmail(string emailAddress) => await Set.AnyAsync(user => !user.IsDeleted && user.Email == emailAddress);
    }
}
