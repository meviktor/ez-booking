using Moq;

namespace BookingWebAPI.Testing.Common
{
    public static class UnitTestExtensions
    {
        /// <summary>
        /// Returns a properly set up mock for an <see cref="IEnumerable<T>"/> collection, which returns an appropriate <see cref="IQueryable<T>"/> setup, for async use as well.
        /// </summary>
        /// <typeparam name="T">The entity type of the collection.</typeparam>
        /// <param name="collection">The bae collection of the mock setup.</param>
        /// <returns>The properly set up mock.</returns>
        public static Mock<IEnumerable<T>> AsEnumerableMockWithAsyncQueryableSetup<T>(this IEnumerable<T> collection)
        {
            var mock = new Mock<IEnumerable<T>>();
            mock.Setup(m => m.GetEnumerator()).Returns(collection.GetEnumerator());

            mock.As<IAsyncEnumerable<T>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns(new TestAsyncEnumerator<T>(collection.GetEnumerator()));

            var collAsQuery = collection.AsQueryable();
            mock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(collAsQuery.Expression);
            mock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(collAsQuery.ElementType);
            mock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<T>(collAsQuery.Provider));
            mock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(collection.GetEnumerator());

            return mock;
        }
    }
}
