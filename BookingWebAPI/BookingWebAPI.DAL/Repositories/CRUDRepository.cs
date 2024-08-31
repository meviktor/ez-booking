using BookingWebAPI.Common.ErrorCodes;
using BookingWebAPI.Common.Exceptions;
using BookingWebAPI.Common.Models;
using BookingWebAPI.DAL.Interfaces;

namespace BookingWebAPI.DAL.Repositories
{
    internal class CRUDRepository<TEntity> : CRURepository<TEntity>, ICRUDRepository<TEntity> where TEntity : ModelBase
    {
        public CRUDRepository(BookingWebAPIDbContext dbContext) 
            : base(dbContext)
        {}

        // TODO: handling concurrency? Tested?
        public async Task<Guid> DeleteAsync(Guid id)
        {
            if(id == default || !await ExistsAsync(id))
            {
                throw new DALException(ApplicationErrorCodes.EntityNotFound);
            }
            var entityToDelete = await GetAsync(id);

            // If the item exists in the database by its id, GetAsync must retrive the corresponding entity
            entityToDelete!.IsDeleted = true;
            await DbContext.SaveChangesAsync();

            return entityToDelete.Id;
        }
    }
}
