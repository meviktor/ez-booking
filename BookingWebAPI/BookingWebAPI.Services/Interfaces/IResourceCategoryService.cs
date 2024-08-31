using BookingWebAPI.Common.Models;

namespace BookingWebAPI.Services.Interfaces
{
    public interface IResourceCategoryService
    {
        Task<ResourceCategory?> GetResourceCategoryAsync(Guid id);

        Task<IEnumerable<ResourceCategory>> GetResourceCategoriesAsync();

        Task<ResourceCategory> CreateOrUpdateResourceCategoryAsync(ResourceCategory resourceCategory);

        Task<Guid> DeleteResourceCategoryAsync(Guid id);
    }
}
