using AutoMapper;
using BookingWebAPI.Attributes;
using BookingWebAPI.Common.Constants;
using BookingWebAPI.Common.ViewModels;
using BookingWebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorized]
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
            // users will be created by admin. usernames are auto calculated via format <lastname>.<firstname><# of users having the same first & last name>.
            return Created(nameof(Authenticate), _mapper.Map<BookingWebAPIUserViewModel>(await _userService.RegisterAsync(registerViewModel.EmailAddress, registerViewModel.SiteId, registerViewModel.FirstName, registerViewModel.LastName)));
        }

        [HttpGet(nameof(ConfirmUser))]
        public async Task<BookingWebAPIUserViewModel> ConfirmUser([FromQuery] Guid token)
        {
            return _mapper.Map<BookingWebAPIUserViewModel>(await _userService.FindUserForEmailConfirmationAsync(token));
        }

        [HttpPost("ConfirmUser")]
        public async Task<BookingWebAPIUserViewModel> ConfirmUserPost(ConfirmRegistrationViewModel confirmRegistrationViewModel)
        {
            return _mapper.Map<BookingWebAPIUserViewModel>(await _userService.ConfirmRegistrationAsync(confirmRegistrationViewModel.UserId, confirmRegistrationViewModel.Token, confirmRegistrationViewModel.Password));
        }

        [AllowAnonymous]
        [HttpPost(nameof(Authenticate))]
        public async Task<BookingWebAPIAuthenticationViewModel> Authenticate([FromBody] LoginViewModel loginViewModel)
        {
            var authModel = _mapper.Map<BookingWebAPIAuthenticationViewModel>(await _userService.AuthenticateAsync(loginViewModel.Email, loginViewModel.Password));
            HttpContext.Response.Cookies.Append(ApplicationConstants.JwtToken, authModel.Token, new CookieOptions { Domain = "ezbooking.com", Secure = true, HttpOnly = true, SameSite = SameSiteMode.None, Expires = DateTime.Now.AddDays(1), Path = "/" });
            return authModel;
        }

        [HttpGet(nameof(LoggedInUser))]
        public async Task<BookingWebAPIUserViewModel> LoggedInUser()
        {
            var userIdClaim = HttpContext.User.Claims.Single(claim => claim.Type.Equals(ApplicationConstants.JwtClaimId));
            var loggedInUser = await _userService.GetAsync(Guid.Parse(userIdClaim.Value));
            return _mapper.Map<BookingWebAPIUserViewModel>(loggedInUser);
        }

        [HttpPost(nameof(Logout))]
        public IActionResult Logout()
        {
            HttpContext.Response.Cookies.Delete(ApplicationConstants.JwtToken, new CookieOptions { Domain = "ezbooking.com", Secure = true, HttpOnly = true, SameSite = SameSiteMode.None, Path = "/" });
            return Ok();
        }
    }
}
