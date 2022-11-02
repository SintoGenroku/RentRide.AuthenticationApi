using Authentication.Api.Models.Requests.Users;
using Authentication.Api.Models.Responses.Errors;
using Authentication.Common;
using Authentication.Common.Exceptions;
using Authentication.Services.Abstracts;
using AutoMapper;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using RentRide.AuthenticationApi.Models;

namespace Authentication.Api.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthenticationController : Controller
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IMapper _mapper;
    private readonly IBus _bus;
    private readonly ILogger _logger;
    
    public AuthenticationController(IAuthenticationService authenticationService, IMapper mapper,  IBus bus, ILogger<AuthenticationController> logger)
    {
        _authenticationService = authenticationService;
        _mapper = mapper;
        _bus = bus;
        _logger = logger;
    }

    /// <summary>
    ///     Registration endpoint
    /// </summary>
    /// <param name="userModel">Create user model</param>
    /// <exception cref="BadRequestException"></exception>
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status200OK)]
    [HttpPost("registration")]
    public async Task<IActionResult> RegisterAsync(UserRegistrationRequestModel userModel)
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
        
        
        return Ok();
    }
}