using BookingWebAPI.Common.Interfaces;
using BookingWebAPI.DAL.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BookingWebAPI.DAL.Repositories
{
    internal abstract class RepositoryBase<TEntity> where TEntity : class, IEntity
    {
        private readonly BookingWebAPIDbContext _dbContext;

        protected RepositoryBase(BookingWebAPIDbContext dbContext) => _dbContext = dbContext;

        protected BookingWebAPIDbContext DbContext => _dbContext;
        protected DbSet<TEntity> Set => _dbContext.Set<TEntity>();

        public abstract IEnumerable<ErrorCodeAssociation> ErrorCodeAssosications { get; }
    }
}
