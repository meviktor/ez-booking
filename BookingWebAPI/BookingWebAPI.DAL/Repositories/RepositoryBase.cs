using BookingWebAPI.Common.Interfaces;
using BookingWebAPI.DAL.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BookingWebAPI.DAL.Repositories
{
    internal class RepositoryBase<TEntity> where TEntity : class, IEntity
    {
        private readonly BookingWebAPIDbContext _dbContext;

        protected RepositoryBase(BookingWebAPIDbContext dbContext) => _dbContext = dbContext;

        public BookingWebAPIDbContext DbContext => _dbContext;
        public DbSet<TEntity> Set => _dbContext.Set<TEntity>();

        public virtual IEnumerable<ErrorCodeAssociation> ErrorCodeAssosications => Enumerable.Empty<ErrorCodeAssociation>();

        public virtual async Task<TEntity?> GetAsync(Guid id) => await Set.SingleOrDefaultAsync(e => e.Id == id);

        public virtual async Task<bool> ExistsAsync(Guid id) => await Set.AnyAsync(e => e.Id == id);

        public virtual IQueryable<TEntity> GetAll() => Set.AsQueryable();
    }
}
