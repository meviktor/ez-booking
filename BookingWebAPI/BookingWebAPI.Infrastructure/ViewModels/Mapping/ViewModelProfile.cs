using AutoMapper;
using BookingWebAPI.Common.Models;

namespace BookingWebAPI.Infrastructure.ViewModels.Mapping
{
    public class ViewModelProfile : Profile
    {
        public ViewModelProfile()
        {
            CreateMap<ResourceCategory, ResourceCategoryViewModel>();
        }
    }
}
