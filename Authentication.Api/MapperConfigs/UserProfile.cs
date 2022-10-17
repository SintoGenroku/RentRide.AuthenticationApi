using Authentication.Api.Models.Requests.Users;
using Authentication.Api.Models.Responses.Users;
using Authentication.Common;
using AutoMapper;

namespace Authentication.Api.MapperConfigs;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<UserRegistrationRequestModel, User>()
            .ForMember(request => request.Username, o => o.MapFrom(user => user.Login));
        CreateMap<UserLoginRequestModel, User>();
        CreateMap<UserEditRequestModel, User>();
        CreateMap<User, UserResponseModel>();
    }
}