using BookingWebAPI.Common.ErrorCodes;
using BookingWebAPI.Common.Exceptions;
using BookingWebAPI.Common.Models;
using BookingWebAPI.DAL.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace BookingWebAPI.DAL.Repositories
{
    internal abstract class CRURepository<TEntity> : ReadRepository<TEntity>, ICRURepository<TEntity> where TEntity : ModelBase
    {
        public CRURepository(BookingWebAPIDbContext dbContext) 
            : base(dbContext) {}

        //public virtual Task<TEntity> CreateOrUpdateAsync(TEntity entity) => this.CreateOrUpdateInternalAsync(entity);

        // TODO: handling concurrency? Tested?
        /// <summary>
        /// Providing database write for all kinds of repository classes - those, which deal with descandants of <see cref="ModelBase"/> and those ones who don't (dealing with entities which can be physically deleted).
        /// </summary>
        /// <typeparam name="TEntity">The entity type which the repository deals with.</typeparam>
        /// <param name="repository">The repositpry instance.</param>
        /// <param name="entity">The actual entity to save/modify.</param>
        /// <returns></returns>
        /// <exception cref="DALException">When something went wrong during the database operation.</exception>
        public async Task<TEntity> CreateOrUpdateAsync(TEntity entity)
        {
            if (entity.Id != default && !await ExistsAsync(entity.Id))
            {
                throw new DALException(ApplicationErrorCodes.EntityNotFound);
            }

            try
            {
                Set.Update(entity);
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

            // If SaveChanges operation succeeded, there must be an entity in the database having this specific id
#pragma warning disable CS8603
            return await GetAsync(entity.Id);
#pragma warning restore
        }
    }
}
