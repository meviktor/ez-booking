using BookingWebAPI.Common.Models;
using BookingWebAPI.DAL.Interfaces;
using BookingWebAPI.DAL.Repositories.Utils;

namespace BookingWebAPI.DAL.Repositories
{
    internal class CRURepository<TEntity> : ReadRepository<TEntity>, ICRURepository<TEntity> where TEntity : ModelBase
    {
        public CRURepository(BookingWebAPIDbContext dbContext) 
            : base(dbContext) {}

        public virtual Task<TEntity> CreateOrUpdateAsync(TEntity entity) => this.CreateOrUpdateInternalAsync(entity);
    }
}
