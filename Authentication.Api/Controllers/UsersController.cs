using Authentication.Api.Models.Requests.Users;
using Authentication.Api.Models.Responses.Errors;
using Authentication.Api.Models.Responses.Users;
using Authentication.Common;
using Authentication.Common.Exceptions;
using Authentication.Services.Abstracts;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Authentication.Api.Controllers;

 /// <summary>
    ///     Controller for working with users
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class UsersController :Controller
    {
        private readonly IUserService _userService;
        private readonly IAuthenticationService _authenticationService; 
        private readonly IMapper _mapper;

        public UsersController(IUserService userService, IAuthenticationService authenticationService, IMapper mapper)
        {
            _userService = userService;
            _authenticationService = authenticationService;
            _mapper = mapper;
        }
        
        /// <summary>
        ///     Allows you get all users
        /// </summary>
        // <returns>Users collection</returns>
        [ProducesResponseType(typeof(IReadOnlyCollection<UserResponseModel>), StatusCodes.Status200OK)]
        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        public async Task<ActionResult> GetUsersAsync()
        {
            var users = await _userService.GetUsersAsync();
            
            var userResponseModels =
                _mapper.Map<IReadOnlyCollection<User>, IReadOnlyCollection<UserResponseModel>>(users); 
            
            return Ok(userResponseModels);
        }
        
        /// <summary>
        ///     Allows get current user with his contracts by his ID
        /// </summary>
        /// <param name="id">GUID user identifier</param>
        // <returns>User</returns>
        [ProducesResponseType(typeof(UserResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [Authorize]
        [HttpGet("{id:guid}")]
        public async Task<ActionResult> GetUserAsync(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                throw new NotFoundException("Such user doesn't exists");
            }
            var userResponseModel = _mapper.Map<UserResponseModel>(user);
            
            return Ok(userResponseModel);
        }
        
        /// <summary>
        ///     Allows delete current user by his ID
        /// </summary>
        /// <param name="id">GUID user identifier</param>
        // <returns>Operation status code</returns>
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "ADMIN")]
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> DeleteUserAsync(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                throw new NotFoundException("Such user doesn't exists");
            }
            if (user.Username == HttpContext.User.Identity?.Name)
            {
                throw new ForbiddenException("You can't delete yourself");
            }
            await _userService.DeleteUsersAsync(user);
            
            return NoContent();
        }
        
        /// <summary>
        ///     Allows update current user 
        /// </summary>
        /// <param name="userEditRequestModel">special user model with updated data</param>
        /// <param name="id">GUID user identifier</param>
        // <returns>Operation status code</returns>
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [Authorize]
        [HttpPatch("{id:guid}")]
        public async Task<IActionResult> EditUserAsync(UserEditRequestModel userEditRequestModel, Guid id)
        {
            if (userEditRequestModel.Id != id)
            {
                throw new BadRequestException("Id do not match");
            }

            var user = await _userService.GetUserByIdAsync(userEditRequestModel.Id); 
            
            var result = await _authenticationService.SignInAsync(user, userEditRequestModel.Password);
            if (!result.IsSuccessfull)
            {
                throw new ForbiddenException("invalid password");
            }
            
            user.PhoneNumber = userEditRequestModel.PhoneNumber;
            user.MailAddress = userEditRequestModel.MailAddress;
            await _userService.UpdateAsync(user);
            return Ok();
        }  
}