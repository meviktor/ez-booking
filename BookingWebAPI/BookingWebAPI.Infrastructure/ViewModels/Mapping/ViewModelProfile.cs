using AutoMapper;
using BookingWebAPI.Common.Enums;
using BookingWebAPI.Common.Models;
using BookingWebAPI.Common.ViewModels;

namespace BookingWebAPI.Infrastructure.ViewModels.Mapping
{
    public class ViewModelProfile : Profile
    {
        public ViewModelProfile()
        {
            CreateMap<ResourceCategory, ResourceCategoryViewModel>().ReverseMap();

            CreateMap<BookingWebAPIUser, BookingWebAPIUserViewModel>();
            CreateMap<(BookingWebAPIUser, string), BookingWebAPIAuthenticationViewModel>()
                .ForMember(vm => vm.User, o => o.MapFrom(authInfo => authInfo.Item1))
                .ForMember(vm => vm.Token, o => o.MapFrom(authInfo => authInfo.Item2));
            CreateMap<EmailConfirmationAttempt, EmailConfirmationResultViewModel>()
                .ForMember(r => r.Success, o => o.MapFrom(a => a.Status == EmailConfirmationStatus.Succeeded));
            CreateMap<BookingWebAPISetting, BookingWebAPISettingViewModel>();
        }
    }
}
