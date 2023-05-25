using AutoMapper;
using BookingWebAPI.Common.ViewModels;
using BookingWebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public UsersController(IMapper mapper, IUserService userService)
        {
            _mapper = mapper;
            _userService = userService;
        }

        // TODO: restrict access to this methods only to users with admin privileges!
        [HttpPost(nameof(Register))]
        public async Task<CreatedResult> Register(RegisterViewModel registerViewModel)
        {
            // TODO: update DB schema with new user fields
            // users will be created by admin. usernames are auto calculated via format <lastname>.<firstname><# of users having the same first & last name>.
            return Created(nameof(Authenticate), _mapper.Map<BookingWebAPIUserViewModel>(await _userService.Register(registerViewModel.EmailAddress, registerViewModel.SiteId, registerViewModel.FirstName, registerViewModel.LastName)));
        }

        [HttpGet(nameof(ConfirmUser))]
        public async Task<BookingWebAPIUserViewModel> ConfirmUser([FromQuery] Guid token)
        {
            return _mapper.Map<BookingWebAPIUserViewModel>(await _userService.FindUserForEmailConfirmation(token));
        }

        [HttpPost("ConfirmUser")]
        public async Task<BookingWebAPIUserViewModel> ConfirmUserPost(ConfirmRegistrationViewModel confirmRegistrationViewModel)
        {
            return _mapper.Map<BookingWebAPIUserViewModel>(await _userService.ConfirmRegistration(confirmRegistrationViewModel.UserId, confirmRegistrationViewModel.Token, confirmRegistrationViewModel.Password));
        }

        [AllowAnonymous]
        [HttpPost(nameof(Authenticate))]
        public async Task<BookingWebAPIAuthenticationViewModel> Authenticate(LoginViewModel loginViewModel)
        {
            return _mapper.Map<BookingWebAPIAuthenticationViewModel>(await _userService.Authenticate(loginViewModel.Email, loginViewModel.Password));
        }
    }
}
