using Authentication.Api.Models.Requests.Users;
using Authentication.Api.Models.Responses.Errors;
using Authentication.Api.Models.ViewModels.Users;
using Authentication.Common;
using Authentication.Common.Exceptions;
using AutoMapper;
using IdentityModel.Client;
using IdentityServer4.Extensions;
using IdentityServer4.Services;
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using RentRide.AuthenticationApi.Models;
using RentRide.AuthenticationApi.Models.Requests;
using IAuthenticationService = Authentication.Services.Abstracts.IAuthenticationService;

namespace Authentication.Api.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthenticationController : Controller
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IMapper _mapper;
    private readonly IBus _bus;
    private readonly ILogger _logger;
    private readonly IIdentityServerInteractionService _interactionService;
    
    public AuthenticationController(IAuthenticationService authenticationService, IMapper mapper,  IBus bus, ILogger<AuthenticationController> logger, IIdentityServerInteractionService interactionService)
    {
        _authenticationService = authenticationService;
        _mapper = mapper;
        _bus = bus;
        _logger = logger;
        _interactionService = interactionService;
    }

    /// <summary>
    ///     Registration endpoint
    /// </summary>
    /// <param name="userModel">Create user model</param>
    /// <exception cref="BadRequestException"></exception>
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status200OK)]
    [HttpPost("registration")]
    public async Task<IActionResult> RegisterAsync([FromForm]UserRegistrationRequestModel userModel)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.ToDictionary(
                error => error.Key,
                error => error.Value?.Errors.Select(e => e.ErrorMessage).ToArray()
            );
            
            throw new BadRequestException("Invalid data", errors);
        }

        var user = _mapper.Map<User>(userModel);

        var result = await _authenticationService.RegisterAsync(user, userModel.Password);

        if (!result.IsSuccessfull)
        {
            var error = new Dictionary<string, string[]>
            {
                { "Error", result.ErrorMessages.ToArray() }
            };
            
            throw new BadRequestException("Registration error", error);
        }

        var createdUser = _mapper.Map<UserRegistrationRequestModel, UserCreated>(userModel);
        createdUser.Id = user.Id;
        
        _logger.LogInformation($"Created User - Login: {user.Username}, Id: {user.Id}");
        await _bus.Publish(createdUser);
        _logger.LogInformation($"Publish created user");
        
        return  RedirectToAction("Login", "Authentication");;
    }

    [HttpGet("registration")]
    public IActionResult RegistrationAsync()
    {
        
        return View();
    }
    
    /// <summary>
    ///     login endpoint
    /// </summary>
    /// <param name="userModel">login user model</param>
    /// <exception cref="BadRequestException"></exception>
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status200OK)]
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromForm]UserLoginViewModel userModel)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.ToDictionary(
                error => error.Key,
                error => error.Value?.Errors.Select(e => e.ErrorMessage).ToArray()
            );
            
            throw new BadRequestException("Invalid data", errors);
        }
        
        var user = _mapper.Map<LoginRequestModel>(userModel);
    
        var result = await  _authenticationService.SignInAsync(user);

        if (result.IsSuccessfull)
        {
            var token = await HttpContext.GetTokenAsync("access_token"); //returns null
            var client = new HttpClient();
            client.SetBearerToken(token);
            return Redirect(userModel.ReturnUrl);
        }
        ModelState.AddModelError(String.Empty, "Incorrect login credentials");
        return View(userModel);
    }

    [HttpGet("login")]
    public IActionResult LoginAsync(string returnUrlll)
    {
        var loginViewModel = new UserLoginViewModel()
        {
            ReturnUrl = returnUrlll
        };
        return View(loginViewModel);
    }

    [HttpGet]
    public async Task<IActionResult> LogoutAsync(string logoutId)
    {
        await _authenticationService.SignOutAsync();
        var logoutRequest = await _interactionService.GetLogoutContextAsync(logoutId);
        return Redirect(logoutRequest.PostLogoutRedirectUri);
    }
}