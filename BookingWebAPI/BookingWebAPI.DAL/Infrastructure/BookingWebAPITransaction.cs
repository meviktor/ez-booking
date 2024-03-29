using Microsoft.EntityFrameworkCore.Storage;

namespace BookingWebAPI.DAL.Infrastructure
{
    internal class BookingWebAPITransaction : IDbContextTransaction
    {
        private readonly BookingWebAPITransactionManager _manager;
        private readonly int _depthLevel;

        public BookingWebAPITransaction(BookingWebAPITransactionManager manager, int depthLevel)
        {
            _manager = manager;
            _depthLevel = depthLevel;
        }

        bool Commited => _depthLevel > _manager.DepthLevel;

        public Guid TransactionId => Transaction?.TransactionId ?? default;

        IDbContextTransaction? Transaction => _manager.CurrentTransaction;

        public void Commit() => _manager.CommitTransaction();

        public Task CommitAsync(CancellationToken cancellationToken = default) => _manager.CommitTransactionAsync(cancellationToken);

        public void Dispose()
        {
            if (!Commited && Transaction != null)
                Transaction.Dispose();
        }

        public ValueTask DisposeAsync() => !Commited && Transaction != null ? Transaction.DisposeAsync() : default;

        public void Rollback() => Transaction?.Rollback();

        public Task RollbackAsync(CancellationToken cancellationToken = default) => Transaction?.RollbackAsync(cancellationToken) ?? Task.CompletedTask;
    }
}
