namespace BookingWebAPI.Testing.Common
{
    /// <summary>
    /// Used to indicate an issue while doing the necessary setup for integration tests.
    /// </summary>
    internal class IntegrationTestSetupException : Exception
    {
        public IntegrationTestSetupException() : base() { }

        public IntegrationTestSetupException(string? message) : base(message) { }

        public IntegrationTestSetupException(string? message, Exception? innerException) : base(message, innerException) { }
    }
}
