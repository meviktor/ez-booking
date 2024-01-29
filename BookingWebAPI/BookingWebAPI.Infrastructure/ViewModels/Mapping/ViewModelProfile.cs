using AutoMapper;
using BookingWebAPI.Common.Models;
using BookingWebAPI.Common.ViewModels;

namespace BookingWebAPI.Infrastructure.ViewModels.Mapping
{
    public class ViewModelProfile : Profile
    {
        public ViewModelProfile()
        {
            CreateMap<ResourceCategory, ResourceCategoryViewModel>();

            CreateMap<BookingWebAPIUser, BookingWebAPIUserViewModel>();
            CreateMap<(BookingWebAPIUser, string), BookingWebAPIAuthenticationViewModel>()
                .ForMember(vm => vm.User, o => o.MapFrom(authInfo => authInfo.Item1))
                .ForMember(vm => vm.Token, o => o.MapFrom(authInfo => authInfo.Item2));
            CreateMap<BookingWebAPIUserViewModel, BookingWebAPIUserConfirmationViewModel>()
                .ForMember(cvm => cvm.User, o => o.MapFrom(uvm => uvm));
            CreateMap<IEnumerable<BookingWebAPISetting>, BookingWebAPIUserConfirmationViewModel>()
                .ForMember(cvm => cvm.PasswordSettings, o => o.MapFrom(s => s));
            CreateMap<EmailConfirmationAttempt, BookingWebAPIUserConfirmationViewModel>();
            CreateMap<BookingWebAPISetting, BookingWebAPISettingViewModel>();
        }
    }
}
