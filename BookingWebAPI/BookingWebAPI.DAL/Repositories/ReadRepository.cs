using BookingWebAPI.Common.Models;
using BookingWebAPI.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookingWebAPI.DAL.Repositories
{
    internal class ReadRepository<TEntity> : IReadRepository<TEntity> where TEntity : ModelBase
    {
        protected readonly BookingWebAPIDbContext _dbContext;
        protected DbSet<TEntity> Set => _dbContext.Set<TEntity>();

        public ReadRepository(BookingWebAPIDbContext dbContext) => _dbContext = dbContext;

        public async Task<TEntity?> GetAsync(Guid id) => await Set.SingleOrDefaultAsync(e => e.Id == id && !e.IsDeleted);

        public async Task<bool> ExistsAsync(Guid id) => await Set.AnyAsync(e => e.Id == id && !e.IsDeleted);

        public IQueryable<TEntity> GetAll() => Set.Where(e => !e.IsDeleted).AsQueryable();
    }
}
