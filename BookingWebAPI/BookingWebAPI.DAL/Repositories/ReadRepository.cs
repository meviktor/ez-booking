using BookingWebAPI.Common.Models;
using BookingWebAPI.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookingWebAPI.DAL.Repositories
{
    internal class ReadRepository<TEntity> : RepositoryBase<TEntity>, IReadRepository<TEntity> where TEntity : ModelBase
    {
        public ReadRepository(BookingWebAPIDbContext dbContext) : base(dbContext) 
        {
        }

        public override async Task<TEntity?> GetAsync(Guid id) => await Set.SingleOrDefaultAsync(e => e.Id == id && !e.IsDeleted);

        public override async Task<bool> ExistsAsync(Guid id) => await Set.AnyAsync(e => e.Id == id && !e.IsDeleted);

        public override IQueryable<TEntity> GetAll() => Set.Where(e => !e.IsDeleted).AsQueryable();
    }
}
