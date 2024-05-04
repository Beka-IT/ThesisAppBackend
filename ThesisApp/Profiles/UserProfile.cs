using AutoMapper;
using ThesisApp.Entities;
using ThesisApp.Models;

namespace ThesisApp.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<SignUpRequest, User>();
            CreateMap<User, UserRequest>();
            CreateMap<CreateDeadlineRequest, Deadline>();
            CreateMap<CreateThesisRequest, Thesis>();
            CreateMap<CreateRequestToThesisRequest, RequestToThesis>();
            CreateMap<RequestToThesis, GetRequestResponse>();
        }
    }
}
