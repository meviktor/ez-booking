using BookingWebAPI.Common.ErrorCodes;
using BookingWebAPI.Common.Exceptions;
using BookingWebAPI.Common.Models;
using BookingWebAPI.DAL.Infrastructure;
using BookingWebAPI.DAL.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace BookingWebAPI.DAL.Repositories
{
    internal class CRURepository<TEntity> : ReadRepository<TEntity>, ICRURepository<TEntity> where TEntity : ModelBase
    {
        public CRURepository(BookingWebAPIDbContext dbContext) 
            : base(dbContext) {}

        protected virtual IEnumerable<ErrorCodeAssociation> ErrorCodeAssosications => Enumerable.Empty<ErrorCodeAssociation>();

        // TODO: handling concurrency? Tested?
        public virtual async Task<TEntity> CreateOrUpdateAsync(TEntity entity)
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

            try
            {
                await _dbContext.SaveChangesAsync();
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
                else throw e;
            }

            // If SaveChanges operation succeeds there must be an entity in the database having this specific id
            return (await GetAsync(entity.Id))!;
        }
    }
}

