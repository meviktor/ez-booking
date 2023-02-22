namespace BookingWebAPI.DAL.Interfaces
{
    public interface ICRUDRepository<TEntity> : ICRURepository<TEntity> where TEntity : class
    {
        public Task DeleteAsync(Guid id);
    }
}
