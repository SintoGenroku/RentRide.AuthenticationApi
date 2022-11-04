using Authentication.Api.Models.Requests.Users;
using Authentication.Api.Models.Responses.Users;
using Authentication.Api.Models.ViewModels.Users;
using Authentication.Common;
using AutoMapper;
using RentRide.AuthenticationApi.Models;
using RentRide.AuthenticationApi.Models.Requests;

namespace Authentication.Api.MapperConfigs;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<UserRegistrationRequestModel, User>()
            .ForMember(request => request.Username, o => o.MapFrom(user => user.Login));
        CreateMap<UserEditRequestModel, User>();
        CreateMap<User, UserResponseModel>();
        CreateMap<UserRegistrationRequestModel, UserCreated>();
        CreateMap<UserCreated, User>();
        CreateMap<UserLoginViewModel, LoginRequestModel>()
            .ForMember(viewModel => viewModel.username, o => o.MapFrom(user => user.Login))
            .ForMember(viewModel => viewModel.password, o => o.MapFrom(user => user.Password));
    }
}