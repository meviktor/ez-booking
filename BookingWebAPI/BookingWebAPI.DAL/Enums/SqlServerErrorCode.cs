namespace BookingWebAPI.DAL.Enums
{
    internal enum SqlServerErrorCode
    {
        CannotInsertDuplicate = 2601,
        CannotInsertNull = 515,
        ConstraintViolated = 547,
        StringOrBinaryTruncated = 2628
    }
}
