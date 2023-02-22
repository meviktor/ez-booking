using BookingWebAPI.Common.ErrorCodes;
using BookingWebAPI.Common.Exceptions;
using BookingWebAPI.Common.Models;
using BookingWebAPI.DAL.Interfaces;

namespace BookingWebAPI.DAL.Repositories
{
    internal class CRURepository<TEntity> : ReadRepository<TEntity>, ICRURepository<TEntity> where TEntity : ModelBase
    {
        public CRURepository(BookingWebAPIDbContext dbContext) 
            : base(dbContext) {}

        // TODO: handling concurrency? Tested?
        public async Task<TEntity> CreateOrUpdateAsync(TEntity entity)
        {
            if (entity.Id != default && !await ExistsAsync(entity.Id))
            {
                throw new DALException(ApplicationErrorCodes.EntityNotFound);
            }

            if (entity.Id == default)
            {
                entity.Id = Guid.NewGuid();
                Set.Add(entity);
            }
            else 
            {
                Set.Update(entity);
            }

            await _dbContext.SaveChangesAsync();

            // If SaveChanges operation succeeds there must be an entity in the database having this specific id
            return await GetAsync(entity.Id);
        }
    }
}

