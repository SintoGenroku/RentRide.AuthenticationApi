using Authentication.Common;
using Authentication.Services.Abstracts;
using AutoMapper;
using MassTransit;
using RentRide.AuthenticationApi.Models;

namespace Authentication.Api.Consumers;

public class UserConsumer : IConsumer<UserQueue>
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;
    //private readonly ILogger _logger;
    
    public UserConsumer(IUserService userService, IMapper mapper/*, ILogger logger*/)
    {
        _userService = userService;
        _mapper = mapper;
        /*_logger = logger;*/
    }
    
    public async Task Consume(ConsumeContext<UserQueue> context)
    {
        var userQueue = context.Message;
        var user = _mapper.Map<UserQueue, User>(userQueue);
        if (user.IsDeleted)
        {
            user.Roles.Clear();
            user.Username = null;
            user.PasswordHash = null;
            
            await _userService.UpdateAsync(user);
            //_logger.LogInformation($"UserApi set user status IsActive to: {user.IsActive}");
            
            return;
        }
        await _userService.UpdateUserActivityAsync(user);
        //_logger.LogInformation($"UserApi set user status IsActive to: {user.IsActive}");
    }
}