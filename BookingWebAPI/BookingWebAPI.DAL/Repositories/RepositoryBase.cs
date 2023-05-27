using BookingWebAPI.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingWebAPI.DAL.Repositories
{
    internal class RepositoryBase<TEntity> where TEntity : ModelBase
    {
        protected RepositoryBase(BookingWebAPIDbContext dbContext) => _dbContext = dbContext;

        protected readonly BookingWebAPIDbContext _dbContext;
        protected DbSet<TEntity> Set => _dbContext.Set<TEntity>();
    }
}
