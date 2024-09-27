using BookingWebAPI.Common.Models;
using BookingWebAPI.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookingWebAPI.DAL.Repositories
{
    internal abstract class ReadRepository<TEntity> : RepositoryBase<TEntity>, IReadRepository<TEntity> where TEntity : ModelBase
    {
        public ReadRepository(BookingWebAPIDbContext dbContext) : base(dbContext) 
        {
        }

        public virtual async Task<TEntity?> GetAsync(Guid id) => await Set.SingleOrDefaultAsync(e => e.Id == id && !e.IsDeleted);

        public virtual async Task<bool> ExistsAsync(Guid id) => await Set.AnyAsync(e => e.Id == id && !e.IsDeleted);

        public virtual IQueryable<TEntity> GetAll() => Set.Where(e => !e.IsDeleted);
    }
}
