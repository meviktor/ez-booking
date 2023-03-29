namespace BookingWebAPI.DAL.Interfaces
{
    public interface IReadRepository<TEntity> where TEntity : class
    {
        IQueryable<TEntity> GetAll();

        Task<TEntity?> GetAsync(Guid id);

        Task<bool> ExistsAsync(Guid id);
    }
}
