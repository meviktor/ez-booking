namespace BookingWebAPI.DAL.Interfaces
{
    public interface ICRURepository<TEntity> : IReadRepository<TEntity> where TEntity : class
    {
        Task<TEntity> CreateOrUpdateAsync(TEntity entity);
    }
}
