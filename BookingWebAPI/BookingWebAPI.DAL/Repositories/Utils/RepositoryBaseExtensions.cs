using BookingWebAPI.Common.ErrorCodes;
using BookingWebAPI.Common.Exceptions;
using BookingWebAPI.Common.Interfaces;
using BookingWebAPI.Common.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace BookingWebAPI.DAL.Repositories.Utils
{
    internal static class RepositoryBaseExtensions
    {
        // TODO: handling concurrency? Tested?
        /// <summary>
        /// Providing database write for all kinds of repository classes - those, which deal with descandants of <see cref="ModelBase"/> and those ones who don't (dealing with entities which can be physically deleted).
        /// </summary>
        /// <typeparam name="TEntity">The entity type which the repository deals with.</typeparam>
        /// <param name="repository">The repositpry instance.</param>
        /// <param name="entity">The actual entity to save/modify.</param>
        /// <returns></returns>
        /// <exception cref="DALException">When something went wrong during the database operation.</exception>
        internal static async Task<TEntity> CreateOrUpdateInternalAsync<TEntity>(this RepositoryBase<TEntity> repository, TEntity entity) where TEntity : class, IEntity
        {
            if (entity.Id != default && !await repository.ExistsAsync(entity.Id))
            {
                throw new DALException(ApplicationErrorCodes.EntityNotFound);
            }

            if (entity.Id == default)
            {
                entity.Id = Guid.NewGuid();
                repository.Set.Add(entity);
            }
            else
            {
                repository.Set.Update(entity);
            }

            try
            {
                await repository.DbContext.SaveChangesAsync();
            }
            catch (DbUpdateException e) when (e.InnerException is SqlException sqlEx)
            {
                var errorCode = repository.ErrorCodeAssosications
                    .SingleOrDefault(association => sqlEx.Message.Contains(association.DatabaseObject) && sqlEx.Number == (int)association.ErrorCode)?
                    .ApplicationErrorCode;
                if (errorCode != default)
                {
                    throw new DALException(errorCode);
                }
                else throw e;
            }

            // If SaveChanges operation succeeds there must be an entity in the database having this specific id
            return (await repository.GetAsync(entity.Id))!;
        }
    }
}
