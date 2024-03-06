using BookingWebAPI.Common.Constants;
using BookingWebAPI.Common.ErrorCodes;
using BookingWebAPI.Common.Models;
using BookingWebAPI.DAL.Enums;
using BookingWebAPI.DAL.Infrastructure;
using BookingWebAPI.DAL.Interfaces;

namespace BookingWebAPI.DAL.Repositories
{
    internal class ResourceRepository : CRUDRepository<Resource>, IResourceRepository
    {
        public ResourceRepository(BookingWebAPIDbContext dbContext) 
            : base(dbContext)
        {}

        public override IEnumerable<ErrorCodeAssociation> ErrorCodeAssosications => new ErrorCodeAssociation[]
        {
            new ErrorCodeAssociation(DatabaseConstraintNames.Resource_Name_UQ, SqlServerErrorCode.CannotInsertDuplicate, ApplicationErrorCodes.ResourceNameMustBeUnique),
            new ErrorCodeAssociation(nameof(Resource.Name), SqlServerErrorCode.CannotInsertNull, ApplicationErrorCodes.ResourceNameRequired),
            new ErrorCodeAssociation(nameof(Resource.Name), SqlServerErrorCode.StringOrBinaryTruncated, ApplicationErrorCodes.ResourceNameTooLong),
            new ErrorCodeAssociation(nameof(Resource.Description), SqlServerErrorCode.StringOrBinaryTruncated, ApplicationErrorCodes.ResourceDescriptionTooLong)
        };
    }
}
