using BookingWebAPI.Common.ErrorCodes;
using BookingWebAPI.Common.Exceptions;
using BookingWebAPI.Common.Models;
using BookingWebAPI.DAL.Infrastructure;
using BookingWebAPI.DAL.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace BookingWebAPI.DAL.Repositories
{
    internal abstract class CRUDRepository<TEntity> : CRURepository<TEntity>, ICRUDRepository<TEntity> where TEntity : ModelBase
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

            try
            {
                await DbContext.SaveChangesAsync();
            }
            catch (DbUpdateException e) when (e.InnerException is SqlException sqlEx)
            {
                var errorCode = ErrorCodeAssosications
                    .SingleOrDefault(association => sqlEx.Message.Contains(association.DatabaseObject) && sqlEx.Number == (int)association.ErrorCode)?
                    .ApplicationErrorCode;
                if (errorCode != default)
                {
                    throw new DALException(errorCode);
                }
                throw;
            }

            return entityToDelete.Id;
        }
    }
}
