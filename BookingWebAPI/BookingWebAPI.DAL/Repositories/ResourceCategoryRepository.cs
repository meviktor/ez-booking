using BookingWebAPI.Common.Constants;
using BookingWebAPI.Common.ErrorCodes;
using BookingWebAPI.Common.Models;
using BookingWebAPI.DAL.Enums;
using BookingWebAPI.DAL.Infrastructure;
using BookingWebAPI.DAL.Interfaces;

namespace BookingWebAPI.DAL.Repositories
{
    internal class ResourceCategoryRepository : CRUDRepository<ResourceCategory>, IResourceCategoryRepository
    {
        public ResourceCategoryRepository(BookingWebAPIDbContext dbContext)
            : base(dbContext)
        {}

        public override IEnumerable<ErrorCodeAssociation> ErrorCodeAssosications => new ErrorCodeAssociation[]
        {
            new ErrorCodeAssociation(DatabaseConstraintNames.ResourceCategory_Name_UQ, SqlServerErrorCode.CannotInsertDuplicate, ApplicationErrorCodes.ResourceCategoryNameMustBeUnique),
            new ErrorCodeAssociation(nameof(Resource.Name), SqlServerErrorCode.CannotInsertNull, ApplicationErrorCodes.ResourceCategoryNameRequired),
            new ErrorCodeAssociation(nameof(Resource.Name), SqlServerErrorCode.StringOrBinaryTruncated, ApplicationErrorCodes.ResourceCategoryNameTooLong),
            new ErrorCodeAssociation(nameof(Resource.Description), SqlServerErrorCode.StringOrBinaryTruncated, ApplicationErrorCodes.ResourceCategoryDescriptionTooLong)
        };
    }
}
