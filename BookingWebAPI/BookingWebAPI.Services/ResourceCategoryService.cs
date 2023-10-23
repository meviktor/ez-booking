using BookingWebAPI.Common.Models;
using BookingWebAPI.DAL.Interfaces;
using BookingWebAPI.Services.Interfaces;

namespace BookingWebAPI.Services
{
    public class ResourceCategoryService : IResourceCategoryService
    {
        private readonly IResourceCategoryRepository _repository;

        public ResourceCategoryService(IResourceCategoryRepository repository) 
        {
            _repository = repository;
        }

        public async Task<ResourceCategory?> GetResourceCategoryAsync(Guid id) => await _repository.GetAsync(id);   
    }
}
