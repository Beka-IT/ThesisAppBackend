﻿using AutoMapper;
using ThesisApp.Entities;
using ThesisApp.Models;

namespace ThesisApp.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<SignUpRequest, User>();
        }
    }
}