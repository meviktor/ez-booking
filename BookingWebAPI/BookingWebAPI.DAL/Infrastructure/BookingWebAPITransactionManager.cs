using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace BookingWebAPI.DAL.Infrastructure
{
    internal class BookingWebAPITransactionManager : IDbContextTransactionManager
    {
        private readonly BookingWebAPIDbContext _dbContext;
        private int _transactionDepthLevel;

        public int DepthLevel => _transactionDepthLevel;

        public BookingWebAPITransactionManager(BookingWebAPIDbContext dbContext)
        {
            _dbContext = dbContext;
            _transactionDepthLevel = 0;
        }

        public IDbContextTransaction? CurrentTransaction => _dbContext.Database.CurrentTransaction;

        public IDbContextTransaction BeginTransaction()
        {
            if(_transactionDepthLevel == 0)
            {
                _dbContext.Database.BeginTransaction();
            }
            _transactionDepthLevel++;
            // If _transactionNestingLevel was zero, there will be a current transaction because of the BeginTransaction() call.
            // If _transactionNestingLevel is not zero, we already have a transaction which is in progress.
            // Eventually, it should not be null at this point.
            return new BookingWebAPITransaction(this, _transactionDepthLevel);
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transactionDepthLevel == 0)
            {
                await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            }
            _transactionDepthLevel++;
            // See the BeginTransaction() method for explanation.
            return new BookingWebAPITransaction(this, _transactionDepthLevel);
        }

        public void CommitTransaction()
        {
            if(_transactionDepthLevel == 1)
            {
                _dbContext.Database.CommitTransaction();
            }
            _transactionDepthLevel--;
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transactionDepthLevel == 1)
            {
                await _dbContext.Database.CommitTransactionAsync(cancellationToken);
            }
            _transactionDepthLevel--;
        }

        public void ResetState() => ((IResettableService)_dbContext).ResetState();
       
        public async Task ResetStateAsync(CancellationToken cancellationToken = default) => await ((IResettableService)_dbContext).ResetStateAsync(cancellationToken);

        public void RollbackTransaction()
        {
            _dbContext.Database.RollbackTransaction();
            _transactionDepthLevel = 0;
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            await _dbContext.Database.RollbackTransactionAsync(cancellationToken);
            _transactionDepthLevel = 0;
        }
    }
}
