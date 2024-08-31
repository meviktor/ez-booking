using AutoMapper;
using BookingWebAPI.Attributes;
using BookingWebAPI.Common.Constants;
using BookingWebAPI.Common.Enums;
using BookingWebAPI.Common.ErrorCodes;
using BookingWebAPI.Common.Exceptions;
using BookingWebAPI.Common.Models.Config;
using BookingWebAPI.Common.ViewModels;
using BookingWebAPI.Infrastructure.ViewModels;
using BookingWebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BookingWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorized]
    public class UsersController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ISettingService _settingService;
        private readonly IEmailConfirmationAttemptService _emailConfirmationService;
        private readonly IUserService _userService;
        private readonly IOptions<FrontEndConfiguration> _frontEndOptions;

        public UsersController(IMapper mapper, IUserService userService, ISettingService settingService, IEmailConfirmationAttemptService emailConfirmationAttemptService, IOptions<FrontEndConfiguration> frontEndOptions)
        {
            _mapper = mapper;
            _settingService = settingService;
            _userService = userService;
            _emailConfirmationService = emailConfirmationAttemptService;
            _frontEndOptions = frontEndOptions;
        }

        // TODO: restrict access to this methods only to users with admin privileges!
        [AllowAnonymous]
        [HttpPost(nameof(Register))]
        public async Task<CreatedResult> Register(RegisterViewModel registerViewModel)
        {
            // users will be created by admin. usernames are auto calculated via format <lastname>.<firstname><# of users having the same first & last name>.
            return Created(nameof(Authenticate), _mapper.Map<BookingWebAPIUserViewModel>(await _userService.RegisterAsync(registerViewModel.EmailAddress, registerViewModel.SiteId, registerViewModel.FirstName, registerViewModel.LastName)));
        }

        [AllowAnonymous]
        [HttpGet]
        [Route($"{nameof(ConfirmEmailAddress)}/{{confirmationAttemptId}}")]
        public async Task<IActionResult> ConfirmEmailAddress(Guid confirmationAttemptId)
        {
            await _userService.ConfirmEmailRegistrationAsync(confirmationAttemptId);
            return Redirect($"{_frontEndOptions.Value.Address}/{string.Format(_frontEndOptions.Value.PathEmailAddressConfirmationResult, confirmationAttemptId)}");
        }

        [AllowAnonymous]
        [HttpGet(nameof(ConfirmEmailAddressResult))]
        public async Task<EmailConfirmationResultViewModel> ConfirmEmailAddressResult(Guid confirmationAttemptId)
        {
            var attempt = await _emailConfirmationService.GetInStatusAsync(confirmationAttemptId, new[] { EmailConfirmationStatus.Succeeded, EmailConfirmationStatus.Failed });
            return _mapper.Map<EmailConfirmationResultViewModel>(attempt);
        }

        [AllowAnonymous]
        [HttpPost(nameof(Authenticate))]
        public async Task<BookingWebAPIAuthenticationViewModel> Authenticate([FromBody] LoginViewModel loginViewModel)
        {
            var authModel = _mapper.Map<BookingWebAPIAuthenticationViewModel>(await _userService.AuthenticateAsync(loginViewModel.Email, loginViewModel.Password));
            HttpContext.Response.Cookies.Append(ApplicationConstants.JwtToken, authModel.Token, new CookieOptions { Domain = "ezbooking.com", Secure = true, HttpOnly = true, SameSite = SameSiteMode.None, Expires = DateTimeOffset.Now.AddDays(1), Path = "/" });
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
