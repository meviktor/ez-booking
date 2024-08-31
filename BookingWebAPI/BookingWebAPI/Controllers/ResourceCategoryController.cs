using AutoMapper;
using BookingWebAPI.Attributes;
using BookingWebAPI.Common.Models;
using BookingWebAPI.Infrastructure.ViewModels;
using BookingWebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookingWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResourceCategoryController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IResourceCategoryService _resourceCategoryService;

        public ResourceCategoryController(IMapper mapper, IResourceCategoryService resourceCategoryService)
        {
            _mapper = mapper;
            _resourceCategoryService = resourceCategoryService;
        }

        //[Authorized]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResourceCategoryViewModel>>> GetResourceCategories()
        {
            var foundCategories = await _resourceCategoryService.GetResourceCategoriesAsync();
            //return NotFound(new { message = $"Reasons" });
            return _mapper.Map<IEnumerable<ResourceCategory>, List<ResourceCategoryViewModel>>(foundCategories);
        }

        [Authorized]
        [HttpGet("{id}")]
        public async Task<ActionResult<ResourceCategoryViewModel>> GetResourceCategory(Guid id)
        {
            var foundCategory = await _resourceCategoryService.GetResourceCategoryAsync(id);
            return foundCategory != null ? _mapper.Map<ResourceCategoryViewModel>(foundCategory) : NotFound(new { message = $"There is no item with the id {id}" });
        }

        //[Authorized]
        [HttpPut]
        public async Task<ActionResult<ResourceCategoryViewModel>> CreateOrUpdateResourceCategory(ResourceCategoryViewModel resourceCategory)
        {
            var foundCategory = await _resourceCategoryService.GetResourceCategoryAsync(resourceCategory.Id);
            return _mapper.Map<ResourceCategoryViewModel>(
                await _resourceCategoryService.CreateOrUpdateResourceCategoryAsync(_mapper.Map(resourceCategory, foundCategory ?? new ResourceCategory()))
            );
        }

        //[Authorized]
        [HttpDelete]
        public async Task<ActionResult<Guid>> DeleteResourceCategory(ResourceCategoryViewModel resourceCategory)
        {
            return await _resourceCategoryService.DeleteResourceCategoryAsync(resourceCategory.Id);
        }
    }
}
