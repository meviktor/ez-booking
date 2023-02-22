using BookingWebAPI.Common.Models;

namespace BookingWebAPI.Services.Interfaces
{
    public interface IResourceCategoryService
    {
        Task<ResourceCategory?> GetResourceCategory(Guid id);
    }
}
