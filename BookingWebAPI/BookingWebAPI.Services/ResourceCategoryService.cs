using BookingWebAPI.Common.Constants;
using BookingWebAPI.Common.ErrorCodes;
using BookingWebAPI.Common.Exceptions;
using BookingWebAPI.Common.Models;
using BookingWebAPI.DAL.Interfaces;
using BookingWebAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

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
        
        public async Task<IEnumerable<ResourceCategory>> GetResourceCategoriesAsync() => await _repository.GetAll().ToListAsync();

        public async Task<ResourceCategory> CreateOrUpdateResourceCategoryAsync(ResourceCategory resourceCategory) => await _repository.CreateOrUpdateAsync(resourceCategory);

        public async Task<Guid> DeleteResourceCategoryAsync(Guid id) => await _repository.DeleteAsync(id);
    }
}
